﻿using Models.Dto.GetModels.BaseDtoGetModels;
using Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.GetModels
{
    public class EquipmentDtoGetModel : BaseDtoGetModel
    {
        public ICollection<Equipment>? Collection { get; set; }
    }
}
