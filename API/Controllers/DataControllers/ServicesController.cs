using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Models.Entity;
using AutoMapper;
using Models.Mapper;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Models.QuerySupporter;
using API.Utility;
using Models.Dto.PostPutModels;
using Models.Dto.GetModels;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using ClosedXML.Excel;
using Models.Dto.FileModels;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers.DataControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор, Менеджер по работе с клиентами, Отдел обслуживания")]
    public class ServicesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public ServicesController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("Document")]
        public async Task<ActionResult<FileModel>> GetServiceWordDocument(
            [FromQuery] Guid serviceId)
        {
            if (!_context.Services.Where(x => x.Id == serviceId).Any())
            {
                return BadRequest("Данное обслуживание не существует");
            }
            Service service = await _context.Services.Where(x => x.Id == serviceId).
                Include(x => x.Equipment).ThenInclude(x => x!.TechnicalTask).ThenInclude(x =>x!.TypeEquipment).
                Include(x => x.Equipment).ThenInclude(x => x!.Order).ThenInclude(x => x!.Contractor).FirstAsync();
            MemoryStream ms = WordHelper.GetServiceDocument(service);

            FileModel response = new FileModel() { Name = $"Договор_обслуживания_{service.Date.ToShortDateString()}.docx", Data = ms.ToArray() };
            return Ok(response);
        }


        [HttpPost("Import")]
        public async Task<ActionResult> importServices(byte[] data, Guid id)
        {
            List<ServiceDto> services = ExcelExporter.getImportModel<ServiceDto>(data, "Обслуживание");
            List<Service> itemsToAdd = new List<Service>();
            if (!_context.Equipments.Where(x => x.Id == id).Any())
            {
                return BadRequest("Данное оборудование не существует!");
            }
            foreach (var service in services)
            {
                service.EquipmentId = id;
                ValidationContext validationContext = new ValidationContext(service);
                if (!Validator.TryValidateObject(service, validationContext, null, true))
                {
                    return BadRequest($"Ошибка валидации внутри файла импортирования, исправте ошибку валидации и повторите попытку (Ошибка на строке {services.IndexOf(service) + 2})");
                }
                Service item = _mapper.Map<Service>(service);
                item.Equipment = _context.Equipments.Where(x => x.Id == service.EquipmentId).First();
                itemsToAdd.Add(item);
            }
            await _context.Services.AddRangeAsync(itemsToAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Export")]
        public async Task<ActionResult<FileModel>> exportServices(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.Services.AsQueryable();
            if (query == null)
            {
                return BadRequest("Нет параметров для данных!");
            }
            if (!string.IsNullOrEmpty(query.Filter))
            {
                if (query.FilterParams != null)
                {
                    items = items.Where(query.Filter, query.FilterParams);
                }
                else
                {
                    items = items.Where(query.Filter);
                }
            }
            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                items = items.OrderBy(query.OrderBy);
            }
            using (MemoryStream ms = new MemoryStream())
            {
                List<ServiceDto> data = new List<ServiceDto>();
                foreach (var item in await items.ToListAsync())
                {
                    data.Add(_mapper.Map<ServiceDto>(item));
                }
                XLWorkbook wb = ExcelExporter.getExcelReport(data, "Обслуживание");
                wb.Worksheet("Обслуживание").Column(3).Style.NumberFormat.SetFormat("0.00 ₽");
                wb.SaveAs(ms);
                FileModel response = new FileModel() { Name = $"Обслуживание_{DateTime.Today.ToShortDateString()}.xlsx", Data = ms.ToArray() };
                return Ok(response);
            }
        }

        [HttpGet]
        public async Task<ActionResult<ServiceGetDtoModel>> getServices(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.Services.Include(x => x.Equipment)
                .ThenInclude(x => x!.Order).ThenInclude(x => x!.Contractor)
                .Include(x => x.Equipment).ThenInclude(x => x!.TechnicalTask)
                .ThenInclude(x => x!.TypeEquipment).AsQueryable();
            if (query == null)
            {
                return BadRequest("Нет параметров для данных!");
            }
            if (!string.IsNullOrEmpty(query.Filter))
            {
                if (query.FilterParams != null)
                {
                    items = items.Where(query.Filter, query.FilterParams);
                }
                else
                {
                    items = items.Where(query.Filter);
                }
            }

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                items = items.OrderBy(query.OrderBy);
            }
            ServiceGetDtoModel serviceGetDtoModel = new ServiceGetDtoModel();
            if (query.Skip <= -1 || query.Top <= 0)
            {
                return BadRequest("Неправильный формат строки запроса!");
            }
            serviceGetDtoModel.TotalPages = PageCounter.CountPages(items.Count(), query.Top);
            serviceGetDtoModel.ElementsCount = items.Count();
            items = items.Skip(query.Skip);
            serviceGetDtoModel.CurrentPageIndex = serviceGetDtoModel.TotalPages + 1 - PageCounter.CountPages(items.Count(), query.Top);
            items = items.Take(query.Top);
            serviceGetDtoModel.Collection = await items.ToListAsync();
            serviceGetDtoModel.Total = items.Sum(x => x.Sum);
            return Ok(serviceGetDtoModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<Service>> getServiceById(Guid id)
        {
            if (_context.Services.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.Services.Where(x => x.Id == id).Include(x => x.Equipment)
                .ThenInclude(x => x!.Order).ThenInclude(x => x!.Contractor)
                .Include(x => x.Equipment).ThenInclude(x => x!.TechnicalTask)
                .ThenInclude(x => x!.TypeEquipment).FirstAsync());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Service>> postService(ServiceDto serviceDto)
        {
            if (!_context.Equipments.Where(x => x.Id == serviceDto.EquipmentId).Any())
            {
                return BadRequest("Данное оборудование не существует!");
            }
            Service service = _mapper.Map<Service>(serviceDto);
            service.Equipment = _context.Equipments.Where(x => x.Id == serviceDto.EquipmentId).First();
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            return Ok(service);
        }

        [HttpPut]
        public async Task<ActionResult<Service>> putService([FromQuery] Guid id, ServiceDto serviceDto)
        {
            Service? serviceCheck = await _context.Services.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (serviceCheck == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (!_context.Equipments.Where(x => x.Id == serviceDto.EquipmentId).Any())
            {
                return BadRequest("Данное оборудование не существует!");
            }
            SafeMapper.MapServiceFromServiceDto(serviceDto, serviceCheck);
            serviceCheck.Equipment = _context.Equipments.Where(x => x.Id == serviceDto.EquipmentId).First();
            await _context.SaveChangesAsync();
            return Ok(serviceCheck);
        }


        [HttpDelete]
        public async Task<ActionResult> deleteService([FromQuery] Guid id)
        {
            Service? delete = await _context.Services.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Запись не существует!");
            }
            _context.Services.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }
    }
}
