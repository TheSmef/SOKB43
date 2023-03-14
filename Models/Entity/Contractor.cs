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
    public class Contractor : BaseModel
    {
        [Required(ErrorMessage = "Название контрагента обязательно для ввода!")]
        [StringLength(50)]
        [MinLength(3, ErrorMessage = "Название контрагента не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Название контрагента не может быть более 50 символов!")]
        [RegularExpression(pattern: "^[а-яёЁА-Я0-9\"«»' ]+$",
            ErrorMessage = "Название контрагента должно содержать в себе только буквы кириллицы и цифры!")]
        public string Name { get; set; } = string.Empty;
        [StringLength(150)]
        [Required(ErrorMessage = "Описание контрагента обязательно для ввода!")]
        [MinLength(3, ErrorMessage = "Описание контрагента не может быть меньше 3 символов!")]
        [MaxLength(150, ErrorMessage = "Описание контрагента не может быть более 150 символов!")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Электронная почта обязательна для ввода!")]
        [EmailAddress(ErrorMessage = "Неправильный формат электронной почты!")]
        [MaxLength(255, ErrorMessage = "Электронная почта не может быть более 255 символов!")]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        [StringLength(11)]
        [Required(ErrorMessage = "Номер телефона обязателен для ввода!")]
        [RegularExpression(pattern: "^(8)[0-9]{10}",
            ErrorMessage = "Неправильный формат номера телефона! (Пример: 88888888888)")]
        public string PhoneNumber { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual List<Order>? Orders { get; set; }
    }
}
