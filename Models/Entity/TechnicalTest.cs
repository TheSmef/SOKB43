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
        [Required]
        public virtual User? User { get; set; }
        [Required]
        public virtual Equipment? Equipment { get; set; }
        [Required]
        [StringLength(50)]
        public string ExpectedConclusion { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string FactConclusion { get; set; } = string.Empty;
        [Required]
        [StringLength(150)]
        public string TestData { get; set; } = string.Empty;
        [Required]
        [StringLength(250)]
        public string Description { get; set; } = string.Empty;
        [StringLength(200)]
        public string? Comment { get; set; }
        [Required]
        [StringLength(20)]
        public string TestPriority { get; set; } = EnumUtility.GetStringsValues(typeof(TestPriorityEnum)).ElementAt(0);
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public bool Passed { get; set; } = true;
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
