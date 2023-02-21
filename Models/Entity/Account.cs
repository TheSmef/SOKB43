using Models.Entity.Base;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Models.Entity
{
    public class Account
    {
        [Key]
        public Guid UserId { get; set; }
        [Required]
        [JsonIgnore]
        public virtual User? User { get; set; }
        [Required]
        [StringLength(255)]
        public string Email { get; set; } =  string.Empty;
        [Required]
        [StringLength(128)]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [StringLength(40)]
        public string Login { get; set; } = string.Empty;
        public virtual List<Role>? Roles { get; set; }
    }
}
