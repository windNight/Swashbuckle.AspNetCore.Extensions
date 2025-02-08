namespace System
{
    /// <summary> </summary>
    internal static class StringExtension
    {
        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string str, int defaultValue = 0)
        {
            int result;
            if (string.IsNullOrEmpty(str) || !int.TryParse(str, out result))
                return defaultValue;
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToLong(this string str, long defaultValue = 0)
        {
            long result;
            if (string.IsNullOrEmpty(str) || !long.TryParse(str, out result))
                return defaultValue;
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string str, decimal defaultValue = 0M)
        {
            decimal result;
            if (string.IsNullOrEmpty(str) || !decimal.TryParse(str, out result))
                return defaultValue;
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble(this string str, double defaultValue = 0.0)
        {
            double result;
            if (string.IsNullOrEmpty(str) || !double.TryParse(str, out result))
                return defaultValue;
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="limitnum"></param>
        /// <returns></returns>
        public static string Limit(this string str, int limitnum)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            if (str.Length <= limitnum)
                return str;
            return str.Substring(0, limitnum);
        }
    }
}
