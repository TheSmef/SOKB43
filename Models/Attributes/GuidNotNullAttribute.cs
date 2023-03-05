using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Attributes
{
    public class GuidNotNullAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is Guid)
            {
                Guid guid = (Guid)value;
                return guid != Guid.Empty;
            }
            return false;
            
        }
    }
}
