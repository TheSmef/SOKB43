using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.StatsModels.GetModels
{
    public class EquipmentTypesStatsModel
    {
        public string TypeName { get; set; } = string.Empty;
        public int Amount { get; set; }
    }
}
