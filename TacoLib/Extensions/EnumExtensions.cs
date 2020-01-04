using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TacoLib
{
    public static class Enum<TEnum>
    {
        private static KeyValuePair<string, object>[] _kvps;
        public static IEnumerable<KeyValuePair<string, object>> GetNameValuePairs() =>
            _kvps ??= Enum.GetNames(typeof(TEnum)).Select(value =>
            {

                var member = typeof(TEnum).GetMember(value)?.FirstOrDefault();
                var name =
                    ((IEnumerable<DisplayAttribute>)member.GetCustomAttributes(typeof(DisplayAttribute), false))?
                    .FirstOrDefault()?.Name ?? value;
                return new KeyValuePair<string, object>(name, value);
            }).ToArray();
    }
    public static class EnumExtensions
    {
    }
}
