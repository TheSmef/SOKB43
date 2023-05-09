using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
using System.Security.Claims;
using ClosedXML.Excel;
using Models.Dto.FileModels;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers.DataControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор, Отдел тестирования")]
    public class TechnicalTestsController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly DataContext _context;

        public TechnicalTestsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("Import")]
        public async Task<ActionResult> importTechnicalTests(byte[] data, Guid id)
        {
            List<TechnicalTestDto> tests = new List<TechnicalTestDto>();
            try
            {
                tests = ExcelExporter.getImportModel<TechnicalTestDto>(data, "Тестирования");
            }
            catch
            {
                return BadRequest("Файл импортирования некорректен");
            }
            List<TechnicalTest> itemsToAdd = new List<TechnicalTest>();
            if (tests.Count == 0)
            {
                return BadRequest("Файл импортирования не содержит данные для импортирования");
            }
            if (!_context.Equipments.Where(x => x.Id == id).Any())
            {
                return BadRequest("Данное оборудование не существует!");
            }
            foreach (var test in tests)
            {
                test.EquipmentId = id;
                ValidationContext validationContext = new ValidationContext(test);
                var results = new List<ValidationResult>();
                if (!Validator.TryValidateObject(test, validationContext, results, true))
                {
                    return BadRequest($"Ошибка валидации внутри файла импортирования, исправьте ошибку валидации и повторите попытку (Ошибка на строке {tests.IndexOf(test) + 2})");
                }
                Guid idUser = Guid.Parse(User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).First().Value);
                TechnicalTest item = _mapper.Map<TechnicalTest>(test);
                item.User = _context.Users.Where(x => x.Id == idUser).First();
                item.Equipment = _context.Equipments.Where(x => x.Id == test.EquipmentId).First();
                itemsToAdd.Add(item);
            }
            await _context.TechnicalTests.AddRangeAsync(itemsToAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Export")]
        public async Task<ActionResult<FileModel>> exportTechnicalTests(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.TechnicalTests.AsQueryable();
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
                List<TechnicalTestDto> data = new List<TechnicalTestDto>();
                foreach (var item in await items.ToListAsync())
                {
                    data.Add(_mapper.Map<TechnicalTestDto>(item));
                }
                XLWorkbook wb = ExcelExporter.getExcelReport(data, "Тестирования");
                wb.SaveAs(ms);
                FileModel response = new FileModel() { Name = $"Тестирования_{DateTime.Today.ToShortDateString()}.xlsx", Data = ms.ToArray() };
                return Ok(response);
            }
        }


        [HttpGet]
        public async Task<ActionResult<TechnicalTestsGetDtoModel>> getTechnicalTests(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.TechnicalTests.Include(x => x.User)
                .Include(x => x.Equipment).ThenInclude(x => x!.TechnicalTask)
                .ThenInclude(x => x!.TypeEquipment)
                .Include(x => x.Equipment)
                .ThenInclude(x => x!.Order).ThenInclude(x => x!.Contractor)
                .AsQueryable();
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
            TechnicalTestsGetDtoModel technicalTestsGetDtoModel = new TechnicalTestsGetDtoModel();
            if (query.Skip <= -1 || query.Top <= 0)
            {
                return BadRequest("Неправильный формат строки запроса!");
            }
            technicalTestsGetDtoModel.TotalPages = PageCounter.CountPages(items.Count(), query.Top);
            technicalTestsGetDtoModel.ElementsCount = items.Count();
            items = items.Skip(query.Skip);
            technicalTestsGetDtoModel.CurrentPageIndex = technicalTestsGetDtoModel.TotalPages + 1 - PageCounter.CountPages(items.Count(), query.Top);
            items = items.Take(query.Top);
            technicalTestsGetDtoModel.Collection = await items.ToListAsync();
            return Ok(technicalTestsGetDtoModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<TechnicalTest>> getTechnicalTestById(Guid id)
        {
            if (_context.TechnicalTests.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.TechnicalTests.Where(x => x.Id == id)
                .Include(x => x.User)
                .Include(x => x.Equipment).ThenInclude(x => x!.TechnicalTask)
                .ThenInclude(x => x!.TypeEquipment)
                .Include(x => x.Equipment)
                .ThenInclude(x => x!.Order).ThenInclude(x => x!.Contractor).FirstAsync());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Администратор, Отдел тестирования")]
        public async Task<ActionResult<TechnicalTest>> postTechnicalTest(TechnicalTestDto testDto)
        {
            if (!_context.Equipments.Where(x => x.Id == testDto.EquipmentId).Any())
            {
                return BadRequest("Данное оборудование не существует!");
            }
            Guid id = Guid.Parse(User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).First().Value);
            TechnicalTest test = _mapper.Map<TechnicalTest>(testDto);
            test.User = _context.Users.Where(x => x.Id == id).First();
            test.Equipment = _context.Equipments.Where(x => x.Id == testDto.EquipmentId).First();
            await _context.TechnicalTests.AddAsync(test);
            await _context.SaveChangesAsync();
            return Ok(test);
        }

        [HttpPut]
        [Authorize(Roles = "Администратор, Отдел тестирования")]
        public async Task<ActionResult<TechnicalTest>> putTechnicalTest([FromQuery] Guid id, TechnicalTestDto testDto)
        {
            TechnicalTest? test = await _context.TechnicalTests.Where(x => x.Id == id).Include(x => x.User)
                .Include(x => x.Equipment).ThenInclude(x => x!.TechnicalTask)
                .ThenInclude(x => x!.TypeEquipment)
                .Include(x => x.Equipment)
                .ThenInclude(x => x!.Order).ThenInclude(x => x!.Contractor).FirstOrDefaultAsync();
            if (test == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (!_context.Equipments.Where(x => x.Id == testDto.EquipmentId).Any())
            {
                return BadRequest("Данное оборудование не существует!");
            }
            SafeMapper.MapTechnicalTestFromDto(testDto, test);
            test.Equipment = _context.Equipments.Where(x => x.Id == testDto.EquipmentId).First();
            await _context.SaveChangesAsync();
            return Ok(test);
        }


        [HttpDelete]
        [Authorize(Roles = "Администратор, Отдел тестирования")]
        public async Task<ActionResult> deleteTechnicalTest([FromQuery] Guid id)
        {
            TechnicalTest? delete = await _context.TechnicalTests.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Запись не существует!");
            }
            _context.TechnicalTests.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }
    }
}
