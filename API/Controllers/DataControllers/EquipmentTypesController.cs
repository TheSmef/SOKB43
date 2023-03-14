using API.Data;
using API.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Dto.GetModels;
using Models.Entity;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Models.QuerySupporter;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace API.Controllers.DataControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор, Технический писатель")]
    public class EquipmentTypesController : Controller
    {
        private readonly DataContext _context;
        public EquipmentTypesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<EquipmentTypesGetDtoModel>> getEquipmentTypes(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.TypesEquipment.AsQueryable();
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
            EquipmentTypesGetDtoModel typesGetDtoModel = new EquipmentTypesGetDtoModel();
            if (query.Skip <= -1 || query.Top <= 0)
            {
                return BadRequest("Неправильный формат строки запроса!");
            }
            typesGetDtoModel.TotalPages = PageCounter.CountPages(items.Count(), query.Top);
            typesGetDtoModel.ElementsCount = items.Count();
            items = items.Skip(query.Skip);
            typesGetDtoModel.CurrentPageIndex = typesGetDtoModel.TotalPages + 1 - PageCounter.CountPages(items.Count(), query.Top);
            items = items.Take(query.Top);
            typesGetDtoModel.Collection = await items.ToListAsync();
            return Ok(typesGetDtoModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<TypeEquipment>> getTypeEquipmentById(Guid id)
        {
            if (_context.TypesEquipment.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.TypesEquipment.Where(x => x.Id == id).FirstAsync());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<TypeEquipment>> postType(TypeEquipment type)
        {
            if (_context.TypesEquipment.Where(x => x.Name == type.Name).Any())
            {
                return BadRequest("Тип оборудования с данным названием уже существует!");
            }
            await _context.TypesEquipment.AddAsync(type);
            await _context.SaveChangesAsync();
            return Ok(type);
        }

        [HttpPut]
        public async Task<ActionResult<Contractor>> putType([FromQuery] Guid id, TypeEquipment type)
        {
            TypeEquipment? typeCheck = await _context.TypesEquipment.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
            if (typeCheck == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (_context.TypesEquipment.Where(x => x.Name == type.Name).Any() && type.Name != typeCheck.Name)
            {
                return BadRequest("Тип оборудования с данным названием уже существует!");
            }
            type.Id = id;
            type.TechnicalTasks = typeCheck.TechnicalTasks;
            _context.Update(type);
            await _context.SaveChangesAsync();
            return Ok(type);
        }


        [HttpDelete]
        public async Task<ActionResult> deleteType([FromQuery] Guid id)
        {
            TypeEquipment? delete = await _context.TypesEquipment.Where(x => x.Id == id).Include(x => x.TechnicalTasks).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (delete.TechnicalTasks!.Any())
            {
                return BadRequest("Невозможно удалить запись, у неё присутствуют дочерние записи!");
            }
            _context.TypesEquipment.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }
    }
}
