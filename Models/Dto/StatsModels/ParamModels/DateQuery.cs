using Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.StatsModels.ParamModels
{
    public class DateQuery
    {
        [Required(ErrorMessage = "Дата начала периода не может быть пустой")]
        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-1);
        [Required(ErrorMessage = "Дата конца периода не может быть пустой")]
        public DateTime EndDate { get; set; } = DateTime.Today;
    }
}
