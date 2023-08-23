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
using Microsoft.IdentityModel.Tokens;
using Models.Dto.StatsModels.GetModels;
using Models.Dto.StatsModels.ParamModels;
using ClosedXML.Excel;
using Models.Dto.FileModels;
using Models.ExportModels;

namespace API.Controllers.DataControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор, Менеджер по работе с клиентами, Отдел тестирования, Отдел обслуживания")]
    public class EquipmentController : Controller
    {
        private readonly IMapper _mapper;

        private readonly DataContext _context;

        public EquipmentController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("Export")]
        [Authorize(Roles = "Администратор, Менеджер по работе с клиентами")]
        public async Task<ActionResult<FileModel>> exportEquipment(
            [FromQuery] QuerySupporter query, CancellationToken ct)
        {
            var items = _context.Equipments.AsNoTracking().Include(x => x.TechnicalTask)
                .ThenInclude(x => x!.TypeEquipment).AsQueryable();
            if (query == null)
            {
                return BadRequest("Нет параметров для данных!");
            }
            items = QueryParamHelper.SetParams(items, query);
            List<EquipmentExportModel> export = new List<EquipmentExportModel>();
            foreach(var item in await items.ToListAsync(ct))
            {
                export.Add(SafeMapper.MapEquipmentForExport(item));
            }
            using (MemoryStream ms = new MemoryStream())
            {
                XLWorkbook wb = ExcelExporter.getExcelReport(export, "Оборудование");
                wb.SaveAs(ms);
                FileModel response = new FileModel() { Name = $"Оборудование_{DateTime.Today.ToShortDateString()}.xlsx", Data = ms.ToArray() };
                return Ok(response);
            }
        }

        [HttpGet("Stats")]
        [Authorize(Roles = "Администратор, Менеджер по работе с клиентами")]
        public async Task<ActionResult<List<EquipmentTypesStatsModel>>> GetTypesStats(
            [FromQuery] DateQuery query, [FromQuery] Guid? id, CancellationToken ct)
        {
            if (query.StartDate.AddMonths(3) < query.EndDate)
            {
                return BadRequest("Период просмотра статистики не должен превышать 3 месяцев");
            }
            var items = _context.Equipments.Where(x => x.Order!.Date >= query.StartDate && x.Order!.Date <= query.EndDate && x.Deleted == false);
            if (!_context.Contractors.Where(x => x.Id == id).Any() && id != null)
            {
                return BadRequest("Данного контрагента не существует");
            }
            else if (id != null)
            {
                items = items.Where(x => x.Order!.Contractor!.Id == id);
            }
            List<TypeEquipment> types = await items.Select(x => x.TechnicalTask!.TypeEquipment!).GroupBy(x => x.Name).Select(x => x.First())
                .AsNoTracking()
                .ToListAsync(ct);
            List<EquipmentTypesStatsModel> responce = new List<EquipmentTypesStatsModel>();
            foreach (var type in types)
            {
                responce.Add(new EquipmentTypesStatsModel()
                {
                    TypeName = type.Name,
                    Amount = items.Where(x => x.TechnicalTask!.TypeEquipment!.Id == type.Id).Count()
                });
            }
            return Ok(responce);
        }


        [HttpGet]
        public async Task<ActionResult<EquipmentDtoGetModel>> getEquipments(
            [FromQuery] QuerySupporter query, CancellationToken ct)
        {
            var items = _context.Equipments.AsNoTracking().Include(x => x.Order)
                .ThenInclude(x => x!.Contractor).Include(x => x.TechnicalTask).ThenInclude(x => x!.TypeEquipment).AsQueryable();
            if (query == null)
            {
                return BadRequest("Нет параметров для данных!");
            }
            items = QueryParamHelper.SetParams(items, query);
            EquipmentDtoGetModel equipmentDtoGetModel = new EquipmentDtoGetModel();
            if (query.Skip <= -1 || query.Top <= 0)
            {
                return BadRequest("Неправильный формат строки запроса!");
            }
            equipmentDtoGetModel.TotalPages = PageCounter.CountPages(items.Count(), query.Top);
            equipmentDtoGetModel.ElementsCount = items.Count();
            items = items.Skip(query.Skip);
            equipmentDtoGetModel.CurrentPageIndex = equipmentDtoGetModel.TotalPages + 1 - PageCounter.CountPages(items.Count(), query.Top);
            items = items.Take(query.Top);
            equipmentDtoGetModel.Collection = await items.ToListAsync(ct);
            return Ok(equipmentDtoGetModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<Equipment>> getEquipmentById(Guid id, CancellationToken ct)
        {
            if (_context.Equipments.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.Equipments.Where(x => x.Id == id)
                    .Include(x => x.Order).ThenInclude(x => x!.Contractor)
                    .Include(x => x.TechnicalTask).ThenInclude(x => x!.TypeEquipment)
                    .AsNoTracking()
                    .FirstAsync(ct));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Администратор, Менеджер по работе с клиентами")]
        public async Task<ActionResult<Equipment>> postEquiment(EquipmentDto eqDto)
        {
            Equipment equipment = _mapper.Map<Equipment>(eqDto);
            if (_context.Equipments.Where(x => x.EquipmentCode == equipment.EquipmentCode).Any())
            {
                return BadRequest("Данный код оборудования занят!");
            }
            if (!_context.TechnicalTasks.Where(x => x.Id == eqDto.TechnicalTaskId).Any())
            {
                return BadRequest("Данного технического задания не существует!");
            }
            if (!_context.Orders.Where(x => x.Id == eqDto.OrderId).Any())
            {
                return BadRequest("Данного заказа не существует!");
            }
            equipment.TechnicalTask = _context.TechnicalTasks.Where(x => x.Id == eqDto.TechnicalTaskId).First();
            equipment.Order = _context.Orders.Where(x => x.Id == eqDto.OrderId).First();
            await _context.Equipments.AddAsync(equipment);
            await _context.SaveChangesAsync();
            return Ok(equipment);
        }

        [HttpPut]
        [Authorize(Roles = "Администратор, Менеджер по работе с клиентами, Отдел тестирования")]
        public async Task<ActionResult<Equipment>> putEquipment([FromQuery] Guid id, EquipmentDto eqDto)
        {
            Equipment? equipment = await _context.Equipments.Where(x => x.Id == id).Include(x => x.Order)
                .ThenInclude(x => x!.Contractor).Include(x => x.TechnicalTask).ThenInclude(x => x!.TypeEquipment).FirstOrDefaultAsync();
            if (equipment == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (_context.Equipments.Where(x => x.EquipmentCode == eqDto.EquipmentCode).Any() && eqDto.EquipmentCode != equipment.EquipmentCode)
            {
                return BadRequest("Данный код оборудования занят!");
            }
            if (!_context.TechnicalTasks.Where(x => x.Id == eqDto.TechnicalTaskId).Any())
            {
                return BadRequest("Данного технического задания не существует!");
            }
            if (!_context.Orders.Where(x => x.Id == eqDto.OrderId).Any())
            {
                return BadRequest("Данного заказа не существует!");
            }
            SafeMapper.MapEquipmentFromEquipmentDto(eqDto, equipment);
            equipment.TechnicalTask = _context.TechnicalTasks.Where(x => x.Id == eqDto.TechnicalTaskId).First();
            equipment.Order = _context.Orders.Where(x => x.Id == eqDto.OrderId).First();
            await _context.SaveChangesAsync();
            return Ok(equipment);
        }


        [HttpDelete]
        [Authorize(Roles = "Администратор, Менеджер по работе с клиентами")]
        public async Task<ActionResult> deleteEquipment([FromQuery] Guid id)
        {
            Equipment? delete = await _context.Equipments.Where(x => x.Id == id).Include(x => x.Services).Include(x => x.TechnicalTests).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (delete.Services!.Any() || delete.TechnicalTests!.Any())
            {
                return BadRequest("Невозможно удалить запись, у неё присутствуют дочерние записи!");
            }
            _context.Equipments.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }
    }
}
