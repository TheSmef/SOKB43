using Models.Attributes;
using Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.PostPutModels
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Электронная почта обязательна для ввода!")]
        [EmailAddress(ErrorMessage = "Неправильный формат электронной почты!")]
        [MinLength(5, ErrorMessage = "Неправильный формат электронной почты!")]
        [MaxLength(254, ErrorMessage = "Электронная почта не может быть более 254 символов!")]
        public string Email { get; set; } = string.Empty;
        [AllowNull]
        [MaxLength(30, ErrorMessage = "Пароль не может быть более 30 символов!")]
        [RegularExpression(pattern: "^(?=.*[0-9])(?=.*[!@#$%^&*])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&*]{8,30}$",
            ErrorMessage = "Пароль должен быть 8-30 символов, содержать в себе как минимум одну букву, как минимум 1 цифру и как минимум 1 символ (!@#$%^&*)")]
        public string Password { get; set; } = string.Empty;
        [AllowNull]
        [Compare(nameof(Password), ErrorMessage = "Введённые пароли не совпадают!")]
        public string PasswordConfirm { get; set; } = string.Empty;
        [Required(ErrorMessage = "Логин обязателен для ввода!")]
        [MinLength(3, ErrorMessage = "Логин не может быть меньше 3 символов!")]
        [MaxLength(40, ErrorMessage = "Логин не может быть более 40 символов!")]
        [RegularExpression(pattern: "^[a-zA-Z0-9]+$",
            ErrorMessage = "Логин должен содержать в себе только буквы латинского алфавита и цифры!")]
        public string Login { get; set; } = string.Empty;
        public virtual List<Role>? Roles { get; set; }
        [Required(ErrorMessage = "Фамилия обязательна для ввода!")]
        [MinLength(3, ErrorMessage = "Фамилия не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Фамилия не может быть более 50 символов!")]
        [RegularExpression(pattern: "^[а-яёЁА-Я]+$",
            ErrorMessage = "Фамилия должна содержать в себе только буквы кириллицы!")]
        public string Last_name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Имя обязательно для ввода!")]
        [MinLength(3, ErrorMessage = "Имя не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Имя не может быть более 50 символов!")]
        [RegularExpression(pattern: "^[а-яёЁА-Я]+$",
            ErrorMessage = "Имя должно содержать в себе только буквы кириллицы!")]
        public string First_name { get; set; } = string.Empty;
        [AllowNull]
        [Nullable(3, ErrorMessage = "Отчество не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Отчество не может быть более 50 символов!")]
        [RegularExpression(pattern: "^[а-яёЁА-Я]+$",
            ErrorMessage = "Отчество должно содержать в себе только буквы кириллицы!")]
        public string? Otch { get; set; }
        [Required(ErrorMessage = "Дата рождения обязательна для ввода")]
        [Date(80, 18, ErrorMessage = "Дата рождения должна быть между {1} и {2}")]
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "Серия паспорта обязательна для ввода!")]
        [MaxLength(4, ErrorMessage = "Длинна серии паспорта должна быть равной 4 символам!")]
        [MinLength(4, ErrorMessage = "Длинна серии паспорта должна быть равной 4 символам!")]
        [RegularExpression(pattern: "^\\d+$",
            ErrorMessage = "Серия паспорта должна содержать в себе только цифры!")]
        public string PassportSeries { get; set; } = string.Empty;
        [Required(ErrorMessage = "Серия паспорта обязательна для ввода!")]
        [MaxLength(6, ErrorMessage = "Длинна серии паспорта должна быть равной 6 символам!")]
        [MinLength(6, ErrorMessage = "Длинна серии паспорта должна быть равной 6 символам!")]
        [RegularExpression(pattern: "^\\d+$",
            ErrorMessage = "Номер паспорта должна содержать в себе только цифры!")]
        public string PassportNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Номер телефона обязателен для ввода!")]
        [MaxLength(12, ErrorMessage = "Неправильный формат номера телефона!")]
        [MinLength(11, ErrorMessage = "Неправильный формат номера телефона!")]
        [RegularExpression(pattern: "^([8])[0-9]{10}",
            ErrorMessage = "Неправильный формат номера телефона! (Пример: 88888888888)")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
