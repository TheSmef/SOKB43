using Models.Entity.Base;
using Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class Equipment : BaseModel
    {
        [Required]
        public virtual Order? Order { get; set; }
        [Required]
        public virtual TechnicalTask? TechnicalTask { get; set; }
        [Required]
        [StringLength(20)]
        public virtual string Status { get; set; } = EnumUtility.GetStringsValues(typeof(EquipmentStatusEnum)).ElementAt(0);
        [Required]
        [StringLength(40)]
        public string EquipmentCode { get; set; } = string.Empty;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public bool Deleted { get; set; } = false;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual List<TechnicalTest>? TechicalTests { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual List<Service>? Services { get; set; }

        public enum EquipmentStatusEnum
        {
            [Description("В производстве")]
            DEVELOPMENT,
            [Description("Тестируется")]
            TESTING,
            [Description("Готово к передаче")]
            READY,
            [Description("У заказчика")]
            PRODUCTION
        }

    }
}
