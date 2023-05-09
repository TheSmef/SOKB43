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
            foreach (var name in Enum.GetNames(type))
            {
                var memberInfos = type.GetMember(name);
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == type);
                var valueAttributes = enumValueMemberInfo!.GetCustomAttributes(typeof(DescriptionAttribute), false);
                var description = ((DescriptionAttribute)valueAttributes[0]).Description;
                values.Add(description);
            }
            return values;
        }
    }
}
