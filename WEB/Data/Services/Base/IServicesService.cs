using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;

namespace WEB.Data.Services.Base
{
    public interface IServicesService
    {
        public Task<ServiceGetDtoModel> GetServices(QuerySupporter query);
        public Task<Service> GetServiceById(Guid id);
        public Task<Service> UpdateService(ServiceDto model, Guid id);
        public Task<Service> AddService(ServiceDto model);
        public Task DeleteService(Guid id);
        public Task ExportServices(QuerySupporter query);
        public Task ImportServices(byte[] data, Guid id);
    }
}
