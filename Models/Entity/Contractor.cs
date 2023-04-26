using ClosedXML.Attributes;
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
        [XLColumn(Header = "Название контрагента")]
        [Required(ErrorMessage = "Название контрагента обязательно для ввода!")]
        [StringLength(50)]
        [MinLength(3, ErrorMessage = "Название контрагента не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Название контрагента не может быть более 50 символов!")]
        [RegularExpression(pattern: "^[а-яёЁА-Я0-9\"«»' ]+$",
            ErrorMessage = "Название контрагента должно содержать в себе только буквы кириллицы и цифры!")]
        public string Name { get; set; } = string.Empty;
        [XLColumn(Header = "Описание контрагента")]
        [StringLength(150)]
        [Required(ErrorMessage = "Описание контрагента обязательно для ввода!")]
        [MinLength(3, ErrorMessage = "Описание контрагента не может быть меньше 3 символов!")]
        [MaxLength(150, ErrorMessage = "Описание контрагента не может быть более 150 символов!")]
        public string Description { get; set; } = string.Empty;
        [XLColumn(Header = "Электронная почта контрагента")]
        [Required(ErrorMessage = "Электронная почта обязательна для ввода!")]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Неправильный формат электронной почты!")]
        [MaxLength(255, ErrorMessage = "Электронная почта не может быть более 255 символов!")]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        [XLColumn(Header = "Контактный номер контрагента")]
        [StringLength(11)]
        [Required(ErrorMessage = "Номер телефона обязателен для ввода!")]
        [RegularExpression(pattern: "^(8)[0-9]{10}",
            ErrorMessage = "Неправильный формат номера телефона! (Пример: 88888888888)")]
        public string PhoneNumber { get; set; } = string.Empty;
        [XLColumn(Ignore = true)]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual List<Order>? Orders { get; set; }
    }
}
