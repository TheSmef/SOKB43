using Models.Entity.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models.Entity
{
    public class UserPost : BaseModel
    {
        [Required]
        public virtual Post? Post { get; set; }
        [Required]
        public virtual User? User { get; set; }
        [Required]
        public decimal Share { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
