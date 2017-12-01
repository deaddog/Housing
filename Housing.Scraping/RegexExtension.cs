using System.Text.RegularExpressions;

namespace Housing.Scraping
{
    public static class RegexExtension
    {
        public static Match MatchRegex(this string str, string pattern)
        {
            return Regex.Match(str, pattern);
        }
    }
}
