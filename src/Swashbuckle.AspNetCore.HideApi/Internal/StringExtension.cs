﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swashbuckle.AspNetCore.Extensions.@internal
{
    internal static class StringExtension
    {

        public static bool IsNullOrEmpty(this string sourceString)
        {
            return string.IsNullOrEmpty(sourceString);
        }

        /// <summary>
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string sourceString)
        {
            return string.IsNullOrWhiteSpace(sourceString);
        }
    }

    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || !items.Any();
        }
    }
}
