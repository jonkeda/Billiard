using System.Text.RegularExpressions;

namespace VouwwandImages.Extensions
{
    public static class RegexExtension
    {
        public static Regex WildcardToRegex(this string pattern, bool atEnd)
        {
            if (pattern == null)
            {
                return null;
            }
            return new Regex(pattern.WildcardToRegexString(atEnd), RegexOptions.Compiled);
        }

        public static string WildcardToRegexString(this string pattern, bool atEnd)
        {
            if (atEnd)
            {
                return Regex.Escape(pattern)
                           .Replace(@"\*", ".*")
                           .Replace(@"\?", ".")
                       + "$";

            }
            return Regex.Escape(pattern)
                       .Replace(@"\*", ".*")
                       .Replace(@"\?", ".");

        }
    }
}
