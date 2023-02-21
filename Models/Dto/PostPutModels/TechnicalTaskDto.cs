using Models.Attributes;
using Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.PostPutModels
{
    public class TechnicalTaskDto
    {
        [Required(ErrorMessage = "Тип оборудования обязателен для ввода!")]
        public virtual Guid? TypeEquipmentId { get; set; }
        [Required(ErrorMessage = "Описаение технического задания обязательно для ввода!")]
        [MinLength(3, ErrorMessage = "Описание технического задания не может быть меньше 3 символов!")]
        [MaxLength(1000, ErrorMessage = "Описание технического задания не может быть более 1000 символов!")]
        [RegularExpression(pattern: "^[а-яА-Я0-9a-zA-Z ]+$",
            ErrorMessage = "Описание технического задания должно содержать в себе только буквы кириллицы, латиницы и цифры!")]
        public string Content { get; set; } = string.Empty;
        [Required(ErrorMessage = "Дата создания обязательна для ввода!")]
        [Date(30, 0, ErrorMessage = "Дата создания должна быть между {1} и {2}")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Название оборудования обязательно для ввода!")]
        [MinLength(3, ErrorMessage = "Название оборудования не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Название оборудования не может быть более 50 символов!")]
        [RegularExpression(pattern: "^[а-яА-Я0-9a-zA-Z ]+$",
            ErrorMessage = "Название оборудования должно содержать в себе только буквы кириллицы, латиницы и цифры!")]
        public string NameEquipment { get; set; } = string.Empty;
    }
}
