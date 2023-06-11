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
using Models.Dto.StatsModels.ParamModels;
using Models.Dto.StatsModels.GetModels;
using Models.Dto.FileModels;


namespace API.Controllers.DataControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор, Менеджер по работе с клиентами")]
    public class OrdersController : Controller
    {
        private readonly DataContext _context;

        public OrdersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("Document")]
        public async Task<ActionResult<FileModel>> GetOrderWordDocument(
            [FromQuery] Guid orderId)
        {
            if (!_context.Orders.Where(x => x.Id == orderId).Any())
            {
                return BadRequest("Данный заказ не существует");
            }
            if (!_context.Equipments.Where(x => x.Order!.Id == orderId && x.Deleted == false).Any())
            {
                return BadRequest("Данный заказ не содержит оборудования");
            }
            Order order = await _context.Orders.Where(x => x.Id == orderId)
                .Include(x => x.Contractor).FirstAsync();
            List<Equipment> equipments = await _context.Equipments.Where(x => x.Order!.Id == orderId && x.Deleted == false)
                .Include(x => x.TechnicalTask).ThenInclude(x => x!.TypeEquipment).ToListAsync();
            MemoryStream ms = WordHelper.GetOrderDocument(order, equipments);

            FileModel response = new FileModel() { Name = $"Договор_{order.Date.ToShortDateString()}.docx", Data = ms.ToArray() };
            return Ok(response);
        }

            [HttpGet("Stats")]
        public async Task<ActionResult<List<IncomeStatsModel>>> GetIncomeStats(
            [FromQuery] DateQuery query, [FromQuery] Guid? id)
        {
            if (query.StartDate.AddMonths(3) < query.EndDate)
            {
                return BadRequest("Период просмотра статистики не должен превышать 3 месяцев");
            }
            var orders = _context.Orders.Where(x => x.Date >= query.StartDate && x.Date <= query.EndDate);
            var services = _context.Services.Where(x => x.Date >= query.StartDate && x.Date <= query.EndDate && x.Deleted == false);
            if (!_context.Contractors.Where(x => x.Id == id).Any() && id != null)
            {
                return BadRequest("Данного контрагента не существует");
            }
            else if (id != null)
            {
                orders = orders.Where(x => x.Contractor!.Id == id);
                services = _context.Services.Where(x => x.Date >= query.StartDate && x.Date <= query.EndDate && x.Deleted == false 
                    && x.Equipment!.Order!.Contractor!.Id == id);
            }
            List<DateTime> dates = await orders.Select(x => x.Date).GroupBy(x => x.Date).Select(x => x.First()).ToListAsync();
            dates.AddRange(await services.Select(x => x.Date)
                .GroupBy(x => x.Date).Select(x => x.First()).ToListAsync());
            dates = dates.GroupBy(x => x).Select(x => x.First()).OrderBy(x => x).ToList();
            List<IncomeStatsModel> responce = new List<IncomeStatsModel>();
            foreach (var date in dates)
            {
                responce.Add(new IncomeStatsModel()
                {
                    Date = date.ToLongDateString(),
                    Total = orders.Where(x => x.Date == date).Sum(x => x.Sum) + services.Where(x => x.Date == date).Sum(x => x.Sum)
                });
            }
            return Ok(responce);
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
            ordersGetDtoModel.Total = items.Sum(x => x.Sum);
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
                return Ok(await _context.Orders.Where(x => x.Id == id).Include(x => x.Contractor).FirstAsync());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> postOrder(OrderDto order)
        {
            if (!_context.Contractors.Where(x => x.Id == order.ContractorId).Any())
            {
                return BadRequest("Данный контрагент не существует!");
            }
            Order orderPost = new Order();
            orderPost.Date = order.Date;
            orderPost.Contractor = _context.Contractors.Where(x => x.Id == order.ContractorId).First();
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
                return BadRequest("Запись не существует!");
            }
            if (!_context.Contractors.Where(x => x.Id == orderDto.ContractorId).Any())
            {
                return BadRequest("Данный контрагент не существует!");
            }
            orderCheck.Date = orderDto.Date;
            orderCheck.Contractor = _context.Contractors.Where(x => x.Id == orderDto.ContractorId).First();
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
                return BadRequest("Запись не существует!");
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
