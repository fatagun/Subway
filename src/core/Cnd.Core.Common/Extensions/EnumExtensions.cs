using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Cnd.Core.Common
{
    [DebuggerStepThrough]
    public static class EnumExtensions
    {
        public static string GetEnumDisplayName(this Enum val)
        {
            return val.GetType().GetMember(val.ToString()).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>(false)?.Name ?? val.ToString();
        }
    }
}
