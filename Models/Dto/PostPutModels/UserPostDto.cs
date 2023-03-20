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
        [Required(ErrorMessage = "Пользователь обязателен для заполнения!")]
        [GuidNotNull(ErrorMessage = "Пользователь обязателен для заполнения!")]
        public Guid UserId { get; set; } = Guid.Empty;
        [Required(ErrorMessage = "Должность обязательна для заполнения!")]
        [GuidNotNull(ErrorMessage = "Должность обязательна для заполнения!")]
        public Guid PostId { get; set; } = Guid.Empty;
        [Required(ErrorMessage = "Ставка обязательна для заполнения!")]
        [Range(0.01, 1.00, ErrorMessage = "Ставка является значением от 0 (не включительно) до 1")]
        public decimal Share { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
