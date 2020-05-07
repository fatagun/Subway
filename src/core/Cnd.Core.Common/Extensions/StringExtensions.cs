using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.Text;

namespace Cnd.Core.Common
{
    [DebuggerStepThrough]
    public static class StringExtensions
    {

        /// <summary>
        ///     Determines whether the specified text is empty.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        ///     <c>true</c> if the specified text is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(this string text)
        {
            return text.Length == 0;
        }

        /// <summary>
        ///     Determines whether the specified text is null.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        ///     <c>true</c> if the specified text is null; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNull(this string text)
        {
            return text == null;
        }

        /// <summary>
        ///     Determines whether the specified text is null or empty. Functionally, it's the same as
        ///     <see cref="string.IsNullOrEmpty" /> but this looks better as it's an extension method.
        /// </summary>
        /// <param name="text">String.</param>
        /// <returns>
        ///     <c>true</c> if the specified text is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        ///     Safely parses Integers.
        /// </summary>
        /// <returns>
        ///     <see cref="int" />
        /// </returns>
        /// <param name="value">String.</param>
        public static int SafeIntParse(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException();
            }

            if (int.TryParse(value, out var result))
            {
                return result;
            }

            return default;
        }

        /// <summary>
        ///     Safely parses Floats.
        /// </summary>
        /// <returns>
        ///     <see cref="float" />
        /// </returns>
        /// <param name="value">String.</param>
        public static float SafeFloatParse(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException();
            }

            if (float.TryParse(value, out var result))
            {
                return result;
            }

            return default;
        }


        /// <summary>
        ///     Safely parses Doubles.
        /// </summary>
        /// <returns>
        ///     <see cref="double" />
        /// </returns>
        /// <param name="value">String.</param>
        public static double SafeDoubleParse(this string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                throw new ArgumentNullException();
            }


            if (double.TryParse(number, out var parsed))
            {
                return parsed;
            }
            return default;
        }


        /// <summary>
        ///     Safely parses Decimals.
        /// </summary>
        /// <returns>
        ///     <see cref="decimal" />
        /// </returns>
        /// <param name="value">String.</param>
        public static decimal SafeDecimalParse(this string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                throw new ArgumentNullException();
            }

            if (decimal.TryParse(number, out var parsed))
            {
                return parsed;
            }
            return default;
        }


        /// <summary>
        ///     Safely parses Long .
        /// </summary>
        /// <returns>
        ///     <see cref="long" />
        /// </returns>
        /// <param name="value">String.</param>
        public static long SafeLongParse(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException();
            }

            if (long.TryParse(value, out var result))
            {
                return result;
            }

            return default;
        }

        /// <summary>
        ///     Safely parses boolean.
        /// </summary>
        /// <returns>
        ///     <see cref="bool" />
        /// </returns>
        /// <param name="value">String.</param>
        public static bool SafeBoolParse(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException();
            }

            if (bool.TryParse(value, out var result))
            {
                return result;
            }

            return false;
        }

        /// <summary>
        ///     Surrounds the strings with single quotes
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <returns>IEnumerable<string></returns>
        public static IEnumerable<string> Quote(this IEnumerable<string> strings)
        {
            if (strings == null)
            {
                yield break;
            }

            foreach (var s in strings)
            {
                yield return string.Format("'{0}'", s);
            }
        }

        public static string JoinWith(this IEnumerable<string> strings, char prefix)
        {
            if (strings.IsNullOrEmpty())
            {
                throw new ArgumentException();
            }

            StringBuilder sb = new StringBuilder();
            foreach (var str in strings)
            {
                sb.Append(str).Append(prefix);
            }

            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        /// <summary>
        ///     Gets the bytes.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="str">String.</param>
        public static byte[] GetBytes(this string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        /// <summary>
        /// Converts the time format given string variable as 'yyyy-MM-dd' to 'dd/MM/yyyy'
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>String</returns>
        public static string FromMf(this string str)
        {
            if (!str.IsNullOrEmpty())
            {
                var date = Convert.ToDateTime(str);
                return date.ToString("dd/MM/yyyy");
            }
            else
            {
                return "01/01/1900";
            }

        }
        /// <summary>
        /// Converts the time format given string variable as 'dd/MM/yyyy' to 'yyyy-MM-dd'
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>String</returns>
        public static string ToMf(this string str)
        {
            if (!str.IsNullOrEmpty())
            {
                var date = Convert.ToDateTime(str);
                return date.ToString("yyyy-MM-dd");
            }
            else
            {
                return "1900-01-01";
            }

        }

        public static SecureString ToSecureString(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException("string is null or empty");
            }
            var securePassword = new SecureString();
            foreach (char c in str)
            {
                securePassword.AppendChar(c);
            }
            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}