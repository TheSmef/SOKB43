using Models.Dto.GetModels.BaseDtoGetModels;
using Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.GetModels
{
    public class OrderGetDtoModel : BaseDtoGetModel
    {
        public ICollection<Order>? Collection { get; set; }
        public decimal Total { get; set; }
    }
}
