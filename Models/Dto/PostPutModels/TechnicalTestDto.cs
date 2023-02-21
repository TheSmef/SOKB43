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
        [Required(ErrorMessage = "Пользователь обязателен для ввода!")]
        public virtual Guid UserId { get; set; }
        [Required(ErrorMessage = "Оборудование обязательно для ввода!")]
        public virtual Guid EquipmentId { get; set; }
        [Required(ErrorMessage = "Ожидаемый результат обязателен для ввода!")]
        [MinLength(3, ErrorMessage = "Ожидаемый результат не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Ожидаемый результат не может быть более 50 символов!")]
        [RegularExpression(pattern: "^[0-9a-zA-Zа-яА-Я ]+$",
            ErrorMessage = "Ожидаемый результат должен содержать в себе только буквы кириллицы, латиницы и цифры!")]
        public string ExpectedConclusion { get; set; } = string.Empty;
        [Required(ErrorMessage = "Фактический результат обязателен для ввода!")]
        [MinLength(3, ErrorMessage = "Фактический результат не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Фактический результат не может быть более 50 символов!")]
        [RegularExpression(pattern: "^[0-9a-zA-Zа-яА-Я ]+$",
            ErrorMessage = "Фактический результат должен содержать в себе только буквы кириллицы, латиницы и цифры!")]
        public string FactCoclusion { get; set; } = string.Empty;
        [Required(ErrorMessage = "Данные тестирования обязательны для ввода!")]
        [MinLength(3, ErrorMessage = "Данные тестирования не могут быть меньше 3 символов!")]
        [MaxLength(150, ErrorMessage = "Данные тестирования не могут быть более 150 символов!")]
        [RegularExpression(pattern: "^[0-9a-zA-Zа-яА-Я ]+$",
            ErrorMessage = "Тестовые данные должны содержать в себе только буквы кириллицы, латиницы и цифры!")]
        public string TestData { get; set; } = string.Empty;
        [Required(ErrorMessage = "Описание тестирования обязательно для ввода!")]
        [MinLength(3, ErrorMessage = "Описание тестирования не может быть меньше 3 символов!")]
        [MaxLength(250, ErrorMessage = "Описание тестирования не может быть более 250 символов!")]
        [RegularExpression(pattern: "^[0-9a-zA-Zа-яА-Я ]+$",
            ErrorMessage = "Описание тестирования должно содержать в себе только буквы кириллицы, латиницы и цифры!")]
        public string Description { get; set; } = string.Empty;
        [Nullable(3, ErrorMessage = "Комментарий к тестированию не может быть меньше 3 символов!")]
        [MaxLength(200, ErrorMessage = "Комментарий к тестированию не может быть более 250 символов!")]
        [RegularExpression(pattern: "^[0-9a-zA-Zа-яА-Я ]+$",
            ErrorMessage = "Комментарий к тестированию должен содержать в себе только буквы кириллицы, латиницы и цифры!")]
        public string? Comment { get; set; }
        [Required(ErrorMessage = "Приоритет тестирования обязателен для ввода!")]
        [RegularExpression(pattern: "Высокий|Средний|Низкий", ErrorMessage = "Неверный приоритет тестирования!")]
        public string TestPriority { get; set; } = EnumUtility.GetStringsValues(typeof(TestPriorityEnum)).ElementAt(0);
        [Required(ErrorMessage = "Дата тестирования обязательна для ввода!")]
        [Date(10, 0, ErrorMessage = "Дата тестирования должна быть между {1} и {2}")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Статус удаления обязателен для ввода!")]
        public bool Deleted { get; set; } = false;
    }
}
