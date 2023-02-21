using Models.Dto.GetModels.BaseDtoGetModels;
using Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.GetModels
{
    public class UsersGetDtoModel : BaseDtoGetModel
    {
        public ICollection<User>? Collection { get; set; }
    }
}
