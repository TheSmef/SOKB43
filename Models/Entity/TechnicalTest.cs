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
    public class TechnicalTest : BaseModel
    {
        [XLColumn(Ignore = true)]
        [Required]
        public virtual User? User { get; set; }
        [XLColumn(Ignore = true)]
        [Required]
        public virtual Equipment? Equipment { get; set; }
        [XLColumn(Header = "Ожидаемый результат")]
        [Required]
        [StringLength(50)]
        public string ExpectedConclusion { get; set; } = string.Empty;
        [XLColumn(Header = "Фактический результат")]
        [Required]
        [StringLength(50)]
        public string FactConclusion { get; set; } = string.Empty;
        [XLColumn(Header = "Тестовые данные")]
        [Required]
        [StringLength(150)]
        public string TestData { get; set; } = string.Empty;
        [XLColumn(Header = "Описание тестирования")]
        [Required]
        [StringLength(250)]
        public string Description { get; set; } = string.Empty;
        [XLColumn(Header = "Комментарий тестировщика")]
        [StringLength(200)]
        public string? Comment { get; set; }
        [XLColumn(Header = "Приоритет тестирования")]
        [Required]
        [StringLength(20)]
        public string TestPriority { get; set; } = EnumUtility.GetStringsValues(typeof(TestPriorityEnum)).ElementAt(0);
        [XLColumn(Header = "Дата тестирования")]
        [Required]
        public DateTime Date { get; set; }
        [XLColumn(Header = "Статус прохождения")]
        [Required]
        public bool Passed { get; set; } = true;
        [XLColumn(Ignore = true)]
        [Required]
        public bool Deleted { get; set; } = false;



        public enum TestPriorityEnum
        {
            [Description("Низкий")]
            LOW,
            [Description("Средний")]
            MEDIUM,
            [Description("Высокий")]
            HIGH,
        }

    }
}
