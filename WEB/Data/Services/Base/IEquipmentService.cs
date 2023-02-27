using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;

namespace WEB.Data.Services.Base
{
    public interface IEquipmentService
    {
        public Task<EquipmentDtoGetModel> GetEquipment(QuerySupporter query);
        public Task<Equipment> GetEquipmentById(Guid id);
        public Task<Equipment> UpdateEquipment(EquipmentDto model, Guid id);
        public Task<Equipment> AddEquipment(EquipmentDto model);
        public Task DeleteEquipment(Guid id);
    }
}
