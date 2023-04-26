using ClosedXML.Attributes;
using Models.Entity.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Models.Entity
{
    public class User : BaseModel
    {
        [XLColumn(Header = "Фамилия")]
        [Required]
        [StringLength(50)]
        public string Last_name { get; set; } = string.Empty;
        [XLColumn(Header = "Имя")]
        [Required]
        [StringLength(50)]
        public string First_name { get; set; } = string.Empty;
        [XLColumn(Header = "Отчество")]
        [AllowNull]
        [StringLength(50)]
        public string? Otch { get; set; }
        [XLColumn(Header = "Дата рождения")]
        [Required]
        public DateTime BirthDate { get; set; }
        [XLColumn(Ignore = true)]
        [Required]
        public virtual Account? Account { get; set; }
        [XLColumn(Header = "Серия паспорта")]
        [Required]
        [StringLength(4)]
        public string PassportSeries { get; set; } = string.Empty;
        [XLColumn(Header = "Номер паспорта")]
        [Required]
        [StringLength(6)]
        public string PassportNumber { get; set; } = string.Empty;
        [XLColumn(Header = "Номер телефона")]
        [Required]
        [StringLength(11)]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        [XLColumn(Ignore = true)]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual List<UserPost>? UserPosts { get; set; }
    }

}
