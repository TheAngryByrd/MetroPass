using System;

namespace Framework
{
    public static class StringExtensions
    {
        public static bool ContainsInsensitive(this string source, string searchString)
        {
            return source.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}