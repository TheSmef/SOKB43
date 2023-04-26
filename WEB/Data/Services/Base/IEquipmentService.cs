using Models.Dto.FileModels;
using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Dto.StatsModels.GetModels;
using Models.Dto.StatsModels.ParamModels;
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
        public Task<List<EquipmentTypesStatsModel>> GetTypesStats(DateQuery query, Guid? id = null);
        public Task ExportEquipment(QuerySupporter query);
    }
}
