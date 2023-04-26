namespace WEB.Utility
{
    public static class Formater
    {
        public static string FormatSum(object value)
        {
            return ((double)value).ToString("C0");
        }

        public static string FormatDate(object value)
        {
            if (value != null)
            {
                return Convert.ToDateTime(value).ToShortDateString();
            }

            return string.Empty;
        }
    }
}
