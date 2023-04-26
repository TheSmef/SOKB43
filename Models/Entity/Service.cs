using ClosedXML.Attributes;
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
        [XLColumn(Ignore = true)]
        [Required]
        public virtual Equipment? Equipment { get; set; }
        [XLColumn(Header = "Тип обслуживания", Order = 1)]
        [Required]
        [StringLength(25)]
        public virtual string ServiceType { get; set; } = EnumUtility.GetStringsValues(typeof(ServiceTypeEnum)).ElementAt(0);
        [XLColumn(Header = "Описание работ", Order = 2)]
        [Required]
        [StringLength(150)]
        public string WorkContent { get; set; } = string.Empty;
        [XLColumn(Header = "Сумма работ", Order = 3)]
        [Required]
        public decimal Sum { get; set; }
        [XLColumn(Header = "Дата проведения", Order = 4)]
        [Required]
        public DateTime Date { get; set; }
        [XLColumn(Header = "Статус обслуживания", Order = 5)]
        [Required]
        [StringLength(25)]
        public string Status { get; set; } = EnumUtility.GetStringsValues(typeof(ServiceStatusEnum)).ElementAt(0);
        [XLColumn(Ignore = true)]
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
