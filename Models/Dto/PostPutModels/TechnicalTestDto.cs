using ClosedXML.Attributes;
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
using static Models.Entity.TechnicalTest;

namespace Models.Dto.PostPutModels
{
    public class TechnicalTestDto
    {
        [XLColumn(Ignore = true)]
        [Required(ErrorMessage = "Оборудование обязательно для ввода!")]
        [GuidNotNull(ErrorMessage = "Оборудование обязательно для ввода!")]
        public virtual Guid EquipmentId { get; set; }
        [XLColumn(Header = "Ожидаемый результат")]
        [Required(ErrorMessage = "Ожидаемый результат обязателен для ввода!")]
        [MinLength(3, ErrorMessage = "Ожидаемый результат не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Ожидаемый результат не может быть более 50 символов!")]
        public string ExpectedConclusion { get; set; } = string.Empty;
        [XLColumn(Header = "Фактический результат")]
        [Required(ErrorMessage = "Фактический результат обязателен для ввода!")]
        [MinLength(3, ErrorMessage = "Фактический результат не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Фактический результат не может быть более 50 символов!")]
        public string FactConclusion { get; set; } = string.Empty;
        [XLColumn(Header = "Тестовые данные")]
        [Required(ErrorMessage = "Данные тестирования обязательны для ввода!")]
        [MinLength(3, ErrorMessage = "Данные тестирования не могут быть меньше 3 символов!")]
        [MaxLength(150, ErrorMessage = "Данные тестирования не могут быть более 150 символов!")]
        public string TestData { get; set; } = string.Empty;
        [XLColumn(Header = "Описание тестирования")]
        [Required(ErrorMessage = "Описание тестирования обязательно для ввода!")]
        [MinLength(3, ErrorMessage = "Описание тестирования не может быть меньше 3 символов!")]
        [MaxLength(250, ErrorMessage = "Описание тестирования не может быть более 250 символов!")]
        public string Description { get; set; } = string.Empty;
        [XLColumn(Header = "Комментарий тестировщика")]
        [Nullable(3, ErrorMessage = "Комментарий к тестированию не может быть меньше 3 символов!")]
        [MaxLength(200, ErrorMessage = "Комментарий к тестированию не может быть более 200 символов!")]
        public string? Comment { get; set; }
        [XLColumn(Header = "Приоритет тестирования")]
        [Required(ErrorMessage = "Приоритет тестирования обязателен для ввода!")]
        [RegularExpression(pattern: "Высокий|Средний|Низкий", ErrorMessage = "Неверный приоритет тестирования!")]
        public string TestPriority { get; set; } = EnumUtility.GetStringsValues(typeof(TestPriorityEnum)).ElementAt(0);
        [XLColumn(Header = "Дата тестирования")]
        [Required(ErrorMessage = "Дата тестирования обязательна для ввода!")]
        [Date(10, 0, ErrorMessage = "Дата тестирования должна быть между {1} и {2}")]
        public DateTime Date { get; set; } = DateTime.Today;
        [XLColumn(Header = "Статус прохождения")]
        [Required(ErrorMessage = "Статус тестирования обязателен для ввода!")]
        public bool Passed { get; set; } = false;
        [XLColumn(Ignore = true)]
        [Required(ErrorMessage = "Статус удаления обязателен для ввода!")]
        public bool Deleted { get; set; } = false;
    }
}
