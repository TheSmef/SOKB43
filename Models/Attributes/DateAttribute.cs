using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class DateAttribute : RangeAttribute
    {
        public DateAttribute(int start, int end)
          : base(typeof(DateTime), DateTime.Today.AddYears(-start).ToShortDateString(), DateTime.Today.AddYears(-end).ToShortDateString()) { }
    }
}
