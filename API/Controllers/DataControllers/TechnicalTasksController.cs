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
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers.DataControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор, Менеджер по работе с клиентами, Технический писатель")]
    public class TechnicalTasksController : Controller
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public TechnicalTasksController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<TechnicalTasksGetDtoModel>> getTechnicalTasks(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.TechnicalTasks.Include(x => x.TypeEquipment).AsQueryable();
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
            TechnicalTasksGetDtoModel tasksGetDtoModel = new TechnicalTasksGetDtoModel();
            if (query.Skip <= -1 || query.Top <= 0)
            {
                return BadRequest("Неправильный формат строки запроса!");
            }
            tasksGetDtoModel.TotalPages = PageCounter.CountPages(items.Count(), query.Top);
            tasksGetDtoModel.ElementsCount = items.Count();
            items = items.Skip(query.Skip);
            tasksGetDtoModel.CurrentPageIndex = tasksGetDtoModel.TotalPages + 1 - PageCounter.CountPages(items.Count(), query.Top);
            items = items.Take(query.Top);
            tasksGetDtoModel.Collection = await items.ToListAsync();
            return Ok(tasksGetDtoModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<TechnicalTask>> getTechnicalTaskById(Guid id)
        {
            if (_context.TechnicalTasks.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.TechnicalTasks.Where(x => x.Id == id).Include(x => x.TypeEquipment).FirstAsync());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Администратор, Технический писатель")]
        public async Task<ActionResult<TechnicalTask>> postTechnicalTask(TechnicalTaskDto taskDto)
        {
            if (_context.TechnicalTasks.Where(x => x.NameEquipment == taskDto.NameEquipment).Any())
            {
                return BadRequest("Контрагент с данным названием уже существует!");
            }
            TechnicalTask task = _mapper.Map<TechnicalTask>(taskDto);
            task.TypeEquipment = _context.TypesEquipment.Where(x => x.Id == taskDto.TypeEquipmentId).First();

            await _context.TechnicalTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return Ok(task);
        }

        [HttpPut]
        [Authorize(Roles = "Администратор, Технический писатель")]
        public async Task<ActionResult<TechnicalTask>> putTechnicalTask([FromQuery] Guid id, TechnicalTaskDto taskDto)
        {
            TechnicalTask? taskCheck = await _context.TechnicalTasks.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (taskCheck == null)
            {
                return BadRequest("Такого технического задания не существует");
            }
            if (_context.TechnicalTasks.Where(x => x.NameEquipment == taskDto.NameEquipment).Any())
            {
                return BadRequest("Контрагент с данным названием уже существует!");
            }
            SafeMapper.MapTechnicalTaskFromTechnicalTaskDto(taskDto, taskCheck);
            taskCheck.TypeEquipment = _context.TypesEquipment.Where(x => x.Id == taskDto.TypeEquipmentId).First();
            await _context.SaveChangesAsync();
            return Ok(taskCheck);
        }


        [HttpDelete]
        [Authorize(Roles = "Администратор, Технический писатель")]
        public async Task<ActionResult> deleteTechnicalTask([FromQuery] Guid id)
        {
            TechnicalTask? delete = await _context.TechnicalTasks.Where(x => x.Id == id).Include(x => x.Equipments).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Записи не существует!");
            }
            if (delete.Equipments!.Any())
            {
                return BadRequest("Невозможно удалить запись, у неё присутствуют дочерние записи!");
            }
            _context.TechnicalTasks.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }
    }
}
