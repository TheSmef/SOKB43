using Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.PostPutModels
{
    public class UserPostDto
    {
        [Required(ErrorMessage = "Пользователь обязателен для заполенения!")]
        [GuidNotNull(ErrorMessage = "Пользователь обязателен для заполенения!")]
        public Guid UserId { get; set; } = Guid.Empty;
        [Required(ErrorMessage = "Должность обязательна для заполенения!")]
        [GuidNotNull(ErrorMessage = "Должность обязательна для заполенения!")]
        public Guid PostId { get; set; } = Guid.Empty;
        [Required(ErrorMessage = "Ставка обязательна для заполенения!")]
        [Range(0.01, 1.00, ErrorMessage = "Ставка является значением от 0 (не включительно) до 1")]
        public decimal Share { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
