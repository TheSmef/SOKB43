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
    public class TypeEquipment : BaseModel
    {
        [Required(ErrorMessage = "Название типа оборудования обязательно для ввода!")]
        [StringLength(50)]
        [MinLength(3, ErrorMessage = "Название типа оборудования не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Название типа оборудования не может быть более 50 символов!")]
        [RegularExpression(pattern: "^[а-яёЁА-Я0-9 ]+$",
            ErrorMessage = "Название типа оборудования должно содержать в себе только буквы кириллицы и цифры!")]
        public string Name { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual List<TechnicalTask>? TechnicalTasks { get; set; }
    }
}
