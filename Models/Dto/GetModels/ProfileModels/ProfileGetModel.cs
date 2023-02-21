using Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.GetModels.ProfileModels
{
    public class ProfileGetModel
    {
        public User? User { get; set; }
        public string? RefreshToken { get; set; }
    }
}
