using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class DateAttribute : RangeAttribute
    {
        public DateAttribute(int start, int end)
          : base(typeof(DateTime), DateTime.Now.AddYears(-start).ToShortDateString(), DateTime.Now.AddYears(-end).ToShortDateString()) { }
    }
}
