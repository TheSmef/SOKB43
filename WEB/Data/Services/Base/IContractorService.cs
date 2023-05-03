using Models.Dto.GetModels;
using Models.Entity;
using Models.QuerySupporter;

namespace WEB.Data.Services.Base
{
    public interface IContractorService
    {
        public Task<ContractorsGetDtoModel> GetContractors(QuerySupporter query);
        public Task<Contractor> GetContractorById(Guid id);
        public Task<Contractor> UpdateContractor(Contractor model);
        public Task<Contractor> AddContractor(Contractor model);
        public Task DeleteContractor(Guid id);
        public Task ExportContractors(QuerySupporter query);
        public Task ImportContractors(byte[] data);
    }
}
