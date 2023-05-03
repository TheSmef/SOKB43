using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;

namespace WEB.Data.Services.Base
{
    public interface ITechnicalTestsService
    {
        public Task<TechnicalTestsGetDtoModel> GetTests(QuerySupporter query);
        public Task<TechnicalTest> GetTestById(Guid id);
        public Task<TechnicalTest> UpdateTest(TechnicalTestDto model, Guid id);
        public Task<TechnicalTest> AddTest(TechnicalTestDto model);
        public Task DeleteTest(Guid id);
        public Task ExportTests(QuerySupporter query);
        public Task ImportTests(byte[] data, Guid id);
    }
}
