using Models.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class Token : BaseModel
    {
        [Required]
        public virtual Account? Account { get; set; }
        [Required]
        [StringLength(250)]
        public string TokenStr { get; set; } = string.Empty;
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
