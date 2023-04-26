using ClosedXML.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entity.Base
{
    public class BaseModel
    {
        [Key]
        [XLColumn(Ignore = true)]
        public Guid Id { get; set; } = new Guid();
    }
}
