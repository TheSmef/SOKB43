using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.PostPutModels.AccountModels
{
    public class AuthModel
    {
        [Required(ErrorMessage = "Логин/Электронная почта - необходимое поле")]
        public string Login { get; set; } = string.Empty;
        [Required(ErrorMessage = "Пароль - необходимое поле")]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = true;
    }
}
