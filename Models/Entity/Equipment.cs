using Models.Attributes;
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
        [Required(ErrorMessage = "Заказ обязателен для ввода!")]
        public virtual Order? Order { get; set; }
        [Required(ErrorMessage = "Техническое задание обязательно для ввода!")]
        public virtual TechnicalTask? TechnicalTask { get; set; }
        [Required(ErrorMessage = "Статус оборудование обязателен для ввода!")]
        [RegularExpression(pattern: "В производстве|Тестируется|Готово к передаче|У заказчика", ErrorMessage = "Неверный тип статуса оборудования!")]
        [StringLength(20)]
        public virtual string Status { get; set; } = EnumUtility.GetStringsValues(typeof(EquipmentStatusEnum)).ElementAt(0);
        [Required(ErrorMessage = "Код оборудования обязателен для ввода!")]
        [MinLength(3, ErrorMessage = "Код оборудования не может быть меньше 3 символов!")]
        [StringLength(40, ErrorMessage = "Код оборудования не может быть более 40 символов!")]
        [RegularExpression(pattern: "^[0-9a-zA-Z]+$",
            ErrorMessage = "Код оборудования должен содержать в себе только буквы латиницы и цифры!")]
        public string EquipmentCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Дата сборки обязательна для ввода!")]
        [Date(30, -15, ErrorMessage = "Дата сборки должна быть между {1} и {2}")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Статус удаления обязателен для ввода!")]
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
