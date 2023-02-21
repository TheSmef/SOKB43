using Models.Entity.Base;
using Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class Service : BaseModel
    {
        [Required]
        public virtual Equipment? Equipment { get; set; }
        [Required]
        [StringLength(20)]
        public virtual string ServiceType { get; set; } = EnumUtility.GetStringsValues(typeof(ServiceTypeEnum)).ElementAt(0);
        [Required]
        [StringLength(150)]
        public string WorkContent { get; set; } = string.Empty;
        [Required]
        public decimal Sum { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = EnumUtility.GetStringsValues(typeof(ServiceStatusEnum)).ElementAt(0);
        [Required]
        public bool Deleted { get; set; } = false;





        public enum ServiceTypeEnum
        {
            [Description("Техническое обслуживание")]
            SERVICE,
            [Description("Ремонт")]
            REPAIRS,
            [Description("Модификация")]
            IMPROVEMENT
        }

        public enum ServiceStatusEnum
        {
            [Description("В очереди на исполнение")]
            INQUE,
            [Description("В процессе")]
            INPROCESS,
            [Description("Проведено")]
            DONE
        }
    }
}
