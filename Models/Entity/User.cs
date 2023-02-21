using Models.Entity.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Models.Entity
{
    public class User : BaseModel
    {
        [Required]
        [StringLength(50)]
        public string Last_name { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string First_name { get; set; } = string.Empty;
        [AllowNull]
        [StringLength(50)]
        public string? Otch { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public virtual Account? Account { get; set; }
        [Required]
        [StringLength(4)]
        public string PassportSeries { get; set; } = string.Empty;
        [Required]
        [StringLength(6)]
        public string PassportNumber { get; set; } = string.Empty;
        [Required]
        [StringLength(11)]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual List<UserPost>? UserPosts { get; set; }
    }

}
