using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class NullableAttribute : ValidationAttribute
    {
        int min = 0;
        public NullableAttribute(int min)
        {
            this.min = min; 
        }
        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }
            MinLengthAttribute min = new MinLengthAttribute(this.min);
            return min.IsValid(value);
        }

    }
}
