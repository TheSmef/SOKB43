using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Utility
{
    public static class EnumUtility
    {
        public static List<String> GetStringsValues(Type type)
        {
            List<String> values = new List<String>();
            var enumType = type;
            foreach (var name in Enum.GetNames(enumType))
            {
                var memberInfos = enumType.GetMember(name);
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                var valueAttributes = enumValueMemberInfo!.GetCustomAttributes(typeof(DescriptionAttribute), false);
                var description = ((DescriptionAttribute)valueAttributes[0]).Description;
                values.Add(description);
            }
            return values;
        }
    }
}
