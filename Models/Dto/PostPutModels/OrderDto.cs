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
    public class OrderDto
    {
        [Required(ErrorMessage = "Контрагент обязателен для ввода!")]
        public virtual Guid? ConctractorId { get; set; }
        [Required(ErrorMessage = "Дата заказа обязательна для ввода!")]
        [Date(10, 0, ErrorMessage = "Дата заказа должна быть между {1} и {2}")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Сумма заказа обязательна для ввода!")]
        [Range(0.01, 999999999999.99, ErrorMessage = "Значение суммы заказа должно быть больше 0 и меньше 15 символов до запятой и 2 символов после запятой")]
        public decimal Sum { get; set; }
    }
}
