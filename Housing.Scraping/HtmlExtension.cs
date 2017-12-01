using HtmlAgilityPack;
using System.Web;

namespace Housing.Scraping
{
    public static class HtmlExtension
    {
        public static string DecodedInnerHtml(this HtmlNode node)
        {
            return HttpUtility.HtmlDecode(node.InnerHtml);
        }
        public static string DecodedInnerText(this HtmlNode node)
        {
            return HttpUtility.HtmlDecode(node.InnerText);
        }
        public static string DecodeHtml(this string html) => HttpUtility.HtmlDecode(html);
    }
}
