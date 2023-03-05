using Models.Attributes;
using Models.Entity;
using Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Entity.Equipment;

namespace Models.Dto.PostPutModels
{
    public  class EquipmentDto
    {
        [Required(ErrorMessage = "Заказ обязателен для ввода!")]
        [GuidNotNull(ErrorMessage = "Заказ обязателен для ввода!")]
        public Guid OrderId { get; set; }
        [Required(ErrorMessage = "Техническое задание обязательно для ввода!")]
        [GuidNotNull(ErrorMessage = "Техническое задание обязательно для ввода!")]
        public Guid TechnicalTaskId { get; set; }
        [Required(ErrorMessage = "Статус оборудование обязателен для ввода!")]
        [RegularExpression(pattern: "В производстве|Тестируется|Готово к передаче|У заказчика", ErrorMessage = "Неверный тип статуса оборудования!")]
        public string Status { get; set; } = EnumUtility.GetStringsValues(typeof(EquipmentStatusEnum)).ElementAt(0);
        [Required(ErrorMessage = "Код оборудования обязателен для ввода!")]
        [MinLength(3, ErrorMessage = "Код оборудования не может быть меньше 3 символов!")]
        [MaxLength(40, ErrorMessage = "Код оборудования не может быть более 40 символов!")]
        public string EquipmentCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Дата сборки обязательна для ввода!")]
        [Date(30, -15, ErrorMessage = "Дата сборки должна быть между {1} и {2}")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Статус удаления обязателен для ввода!")]
        public bool Deleted { get; set; } = false;

    }
}
