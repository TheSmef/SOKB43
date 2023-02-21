using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.GetModels.BaseDtoGetModels
{
    public class BaseDtoGetModel
    {
        public int CurrentPageIndex { get; set; }
        public int TotalPages { get; set; }
        public int ElementsCount { get; set; }
    }
}
