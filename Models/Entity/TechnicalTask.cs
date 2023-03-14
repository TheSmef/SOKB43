using Models.Attributes;
using Models.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class TechnicalTask : BaseModel
    {
        [Required]
        public virtual TypeEquipment? TypeEquipment { get; set; }
        [Required(ErrorMessage = "Описаение технического задания обязательно для ввода!")]
        [MinLength(3, ErrorMessage = "Описание технического задания не может быть меньше 3 символов!")]
        [StringLength(6000, ErrorMessage = "Описание технического задания слишком большое, сократите количество символов! (6000 и менее)")]
        public string Content { get; set; } = string.Empty;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [StringLength(50)]
        public string NameEquipment { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual List<Equipment>? Equipments { get; set; }
    }
}
