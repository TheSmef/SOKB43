using Models.Entity.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models.Entity
{
    public class Role : BaseModel
    {
        [Required]
        [StringLength(30)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public Guid AccountUserId { get; set; }
        [JsonIgnore]
        public virtual Account? AccountUser { get; set; }

        public enum NameRole
        {
            [Description("Администратор")]
            ADMIN,
            [Description("Отдел кадров")]
            CHAR,
            [Description("Менеджер по работе с клиентами")]
            MANAGER,
            [Description("Технический писатель")]
            TECHWRITTER,
            [Description("Отдел тестирования")]
            TESTER,
            [Description("Отдел обслуживания")]
            SERVICE,
        }
    }
}
