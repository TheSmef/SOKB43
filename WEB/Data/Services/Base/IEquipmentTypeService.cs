using Models.Dto.GetModels;
using Models.Entity;
using Models.QuerySupporter;

namespace WEB.Data.Services.Base
{
    public interface IEquipmentTypeService
    {
        public Task<EquipmentTypesGetDtoModel> GetTypes(QuerySupporter query);
        public Task<TypeEquipment> GetTypeById(Guid id);
        public Task<TypeEquipment> UpdateType(TypeEquipment model);
        public Task<TypeEquipment> AddType(TypeEquipment model);
        public Task DeleteType(Guid id);
    }
}
