using System.Text.RegularExpressions;

namespace StartPos.Shared.Extesions
{
    public static class StringExtensions
    {
        public static string GetIpAdress(this string input)
        {
            var ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            var result = ip.Matches(input);
            return result.Count > 0 ? result[0].ToString() : string.Empty;
        }
    }
}