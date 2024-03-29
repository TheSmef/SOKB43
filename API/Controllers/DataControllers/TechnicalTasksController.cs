﻿using Microsoft.AspNetCore.Mvc;
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
            [FromQuery] QuerySupporter query, CancellationToken ct)
        {
            var items = _context.TechnicalTasks.AsNoTracking().Include(x => x.TypeEquipment).AsQueryable();
            if (query == null)
            {
                return BadRequest("Нет параметров для данных!");
            }
            items = QueryParamHelper.SetParams(items, query);
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
            tasksGetDtoModel.Collection = await items.ToListAsync(ct);
            return Ok(tasksGetDtoModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<TechnicalTask>> getTechnicalTaskById(Guid id, CancellationToken ct)
        {
            if (_context.TechnicalTasks.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.TechnicalTasks.Where(x => x.Id == id).Include(x => x.TypeEquipment).AsNoTracking().FirstAsync(ct));
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
                return BadRequest("Техническое задание с данным названием оборудования уже существует!");
            }
            if (!_context.TypesEquipment.Where(x => x.Id == taskDto.TypeEquipmentId).Any())
            {
                return BadRequest("Данный тип оборудования не существует!");
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
                return BadRequest("Запись не существует!");
            }
            if (_context.TechnicalTasks.Where(x => x.NameEquipment == taskDto.NameEquipment).Any() && taskCheck.NameEquipment != taskDto.NameEquipment)
            {
                return BadRequest("Техническое задание с данным названием оборудования уже существует!");
            }
            if (!_context.TypesEquipment.Where(x => x.Id == taskDto.TypeEquipmentId).Any())
            {
                return BadRequest("Данный тип оборудования не существует!");
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
                return BadRequest("Запись не существует!");
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
