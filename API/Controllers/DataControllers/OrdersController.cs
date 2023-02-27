using API.Data;
using API.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Dto.GetModels;
using Models.Entity;
using Models.QuerySupporter;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Models.Dto.PostPutModels;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace API.Controllers.DataControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор, Менеджер по работе с клиентами, Отдел обслуживания")]
    public class OrdersController : Controller
    {
        private readonly DataContext _context;

        public OrdersController(DataContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<OrderGetDtoModel>> getOrders(
            [FromQuery] QuerySupporter query)
        {

            var items = _context.Orders.Include(x => x.Contractor)
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
            OrderGetDtoModel ordersGetDtoModel = new OrderGetDtoModel();
            if (query.Skip <= -1 || query.Top <= 0)
            {
                return BadRequest("Неправильный формат строки запроса!");
            }
            ordersGetDtoModel.TotalPages = PageCounter.CountPages(items.Count(), query.Top);
            ordersGetDtoModel.ElementsCount = items.Count();
            items = items.Skip(query.Skip);
            ordersGetDtoModel.CurrentPageIndex = ordersGetDtoModel.TotalPages + 1 - PageCounter.CountPages(items.Count(), query.Top);
            items = items.Take(query.Top);
            ordersGetDtoModel.Collection = await items.ToListAsync();
            return Ok(ordersGetDtoModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<UserPost>> getOrderId(Guid id)
        {
            if (_context.Orders.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.Orders.Include(x => x.Contractor).FirstAsync());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> postOrder(OrderDto order)
        {
            Order orderPost = new Order();
            orderPost.Date = order.Date;
            orderPost.Contractor = _context.Conctractors.Where(x => x.Id == order.ContractorId).First();
            orderPost.Sum = order.Sum;
            await _context.Orders.AddAsync(orderPost);
            await _context.SaveChangesAsync();
            return Ok(orderPost);
        }

        [HttpPut]
        public async Task<ActionResult<Order>> putOrder([FromQuery] Guid id, OrderDto orderDto)
        {
            Order? orderCheck = await _context.Orders.Where(x => x.Id == id).Include(x => x.Contractor).FirstOrDefaultAsync();
            if (orderCheck == null)
            {
                return BadRequest("Такого заказа не существует");
            }
            orderCheck.Date = orderDto.Date;
            orderCheck.Contractor = _context.Conctractors.Where(x => x.Id == orderDto.ContractorId).First();
            orderCheck.Sum = orderDto.Sum;
            await _context.SaveChangesAsync();
            return Ok(orderCheck);
        }


        [HttpDelete]
        public async Task<ActionResult> deleteOrder([FromQuery] Guid id)
        {
            Order? delete = await _context.Orders.Where(x => x.Id == id).Include(x => x.Equipments).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Записи не существует!");
            }
            if (delete.Equipments!.Any())
            {
                return BadRequest("Невозможно удалить запись, у неё присутствуют дочерние записи!");
            }
            _context.Orders.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }
    }
}
