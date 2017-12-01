using Flurl;
using Flurl.Http;
using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Housing.Scraping
{
    public static class FlurlExtension
    {
        public static async Task<HtmlDocument> GetHtmlAsync(this Url url, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            var text = await url.GetStringAsync(cancellationToken, completionOption).ConfigureAwait(false);

            var doc = new HtmlDocument();
            doc.LoadHtml(text);

            return doc;
        }
        public static Task<HtmlDocument> GetHtmlAsync(this Uri url, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return GetHtmlAsync((Url)url, cancellationToken, completionOption);
        }
        public static Task<HtmlDocument> GetHtmlAsync(this string url, CancellationToken cancellationToken = default(CancellationToken), HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return GetHtmlAsync((Url)url, cancellationToken, completionOption);
        }
    }
}
