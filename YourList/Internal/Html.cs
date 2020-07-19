using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace YourList.Internal
{
    internal static class Html
    {
        private static readonly HtmlParser HtmlParser = new HtmlParser();

        public static IHtmlDocument Parse(string source)
        {
            return HtmlParser.ParseDocument(source);
        }
    }
}