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


        [HttpGet]
        public async Task<ActionResult<EquipmentDtoGetModel>> getEquipments(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.Equipments.Include(x => x.Order).ThenInclude(x => x!.Contractor).Include(x => x.TechnicalTask).ThenInclude(x => x!.TypeEquipment).AsQueryable();
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
            equipmentDtoGetModel.Collection = await items.ToListAsync();
            return Ok(equipmentDtoGetModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<Equipment>> getEquipmentById(Guid id)
        {
            if (_context.Equipments.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.Equipments.Where(x => x.Id == id)
                    .Include(x => x.Order).ThenInclude(x => x!.Contractor)
                    .Include(x => x.TechnicalTask).ThenInclude(x => x!.TypeEquipment)
                    .FirstAsync());
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
            Equipment? delete = await _context.Equipments.Where(x => x.Id == id).Include(x => x.Services).Include(x => x.TechicalTests).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (delete.Services!.Any() || delete.TechicalTests!.Any())
            {
                return BadRequest("Невозможно удалить запись, у неё присутствуют дочерние записи!");
            }
            _context.Equipments.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }
    }
}
