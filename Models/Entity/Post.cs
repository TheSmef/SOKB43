using Models.Entity.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models.Entity
{
    public class Post : BaseModel
    {
        [Required(ErrorMessage = "Название должности обязательно для ввода!")]
        [StringLength(50)]
        [MinLength(3, ErrorMessage = "Название должности не может быть меньше 3 символов!")]
        [MaxLength(50, ErrorMessage = "Название должности не может быть более 50 символов!")]
        [RegularExpression(pattern: "^[а-яА-Я0-9 ]+$",
            ErrorMessage = "Название должности должно содержать в себе только буквы кириллицы и цифры!")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Зарплата обязательна для ввода!")]
        [Range(0.01, 999999999999.99, ErrorMessage = "Значение зарплаты должно быть больше 0 и меньше 15 символов до запятой и 2 символов после запятой")]
        public decimal Salary { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual List<UserPost>? UserPosts { get; set; }
    }
}
