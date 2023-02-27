using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
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
    }
}
