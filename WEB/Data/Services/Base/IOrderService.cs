using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Dto.StatsModels.GetModels;
using Models.Dto.StatsModels.ParamModels;
using Models.Entity;
using Models.QuerySupporter;

namespace WEB.Data.Services.Base
{
    public interface IOrderService
    {
        public Task<OrderGetDtoModel> GetOrders(QuerySupporter query);
        public Task<Order> GetOrderById(Guid id);
        public Task<Order> UpdateOrder(OrderDto model, Guid id);
        public Task<Order> AddOrder(OrderDto model);
        public Task DeleteOrder(Guid id);
        public Task<List<IncomeStatsModel>> GetIncomeStats(DateQuery query, Guid? id = null);
        public Task GetWordDocument(Guid id);
    }
}
