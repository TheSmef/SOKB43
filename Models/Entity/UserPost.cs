using Models.Entity.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models.Entity
{
    public class UserPost : BaseModel
    {
        [Required(ErrorMessage = "Должность обязательна для заполенения!")]
        public virtual Post? Post { get; set; }
        [Required(ErrorMessage = "Сотрудник обязателен для заполенения!")]
        public virtual User? User { get; set; }
        [Required(ErrorMessage = "Ставка обязательна для заполенения!")]
        [Range(0.01, 1.00, ErrorMessage = "Ставка является значением от 0 (не включительно) до 1")]
        public decimal Share { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
