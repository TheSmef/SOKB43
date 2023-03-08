using Models.Attributes;
using Models.Entity;
using Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Models.Entity.Service;

namespace Models.Dto.PostPutModels
{
    public class ServiceDto
    {
        [Required(ErrorMessage = "Оборудование обязательно для ввода!")]
        [GuidNotNull(ErrorMessage = "Оборудование обязательно для ввода!")]
        public virtual Guid? EquipmentId { get; set; }
        [Required(ErrorMessage = "Тип работ обязателен для ввода!")]
        [RegularExpression(pattern: "Техническое обслуживание|Ремонт|Модификация", ErrorMessage = "Неверный тип работ!")]
        public virtual string ServiceType { get; set; } = EnumUtility.GetStringsValues(typeof(ServiceTypeEnum)).ElementAt(0);
        [Required(ErrorMessage = "Описание работ обязательно для ввода!")]
        [MinLength(3, ErrorMessage = "Описание работ не может быть меньше 3 символов!")]
        [MaxLength(150, ErrorMessage = "Описание работ не может быть более 150 символов!")]
        public string WorkContent { get; set; } = string.Empty;
        [Required(ErrorMessage = "Сумма оплаты работ обязательна для ввода!")]
        [Range(0.01, 999999999999.99, ErrorMessage = "Значение суммы оплаты работ должно быть больше 0 и меньше 15 символов до запятой и 2 символов после запятой")]
        public decimal Sum { get; set; }
        [Required(ErrorMessage = "Дата проведения обязательна для ввода!")]
        [Date(30, -5, ErrorMessage = "Дата проведения должна быть между {1} и {2}")]
        public DateTime Date { get; set; } = DateTime.Today;
        [Required(ErrorMessage = "Статус работ обязателен для ввода!")]
        [RegularExpression(pattern: "В очереди на исполнение|В процессе|Проведено", ErrorMessage = "Неверный статус работ!")]
        public string Status { get; set; } = EnumUtility.GetStringsValues(typeof(ServiceStatusEnum)).ElementAt(0);
        [Required(ErrorMessage = "Статус удаления обязателен для ввода!")]
        public bool Deleted { get; set; } = false;
    }
}
