using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;

namespace WEB.Data.Services.Base
{
    public interface ITechnicalTaskService
    {
        public Task<TechnicalTasksGetDtoModel> GetTechnicalTasks(QuerySupporter query);
        public Task<TechnicalTask> GetTechnicalTaskById(Guid id);
        public Task<TechnicalTask> UpdateTechnicalTask(TechnicalTaskDto model, Guid id);
        public Task<TechnicalTask> AddTechnicalTask(TechnicalTaskDto model);
        public Task DeleteTechnicalTask(Guid id);
    }
}
