using Models.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class Order : BaseModel
    {
        [Required]
        public virtual Contractor? Contractor { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public decimal Sum { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual List<Equipment>? Equipments { get; set; } 
    }
}
