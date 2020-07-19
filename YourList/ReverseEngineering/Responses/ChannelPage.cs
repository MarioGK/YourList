using System;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using YourList.Exceptions;
using YourList.Internal;
using YourList.Internal.Extensions;

namespace YourList.ReverseEngineering.Responses
{
    internal partial class ChannelPage
    {
        private readonly IHtmlDocument _root;

        public ChannelPage(IHtmlDocument root)
        {
            _root = root;
        }

        private bool IsOk()
        {
            return _root
                .QuerySelector("meta[property=\"og:url\"]") != null;
        }

        public string GetChannelUrl()
        {
            return _root
                .QuerySelectorOrThrow("meta[property=\"og:url\"]")
                .GetAttributeOrThrow("content");
        }

        public string GetChannelId()
        {
            return GetChannelUrl()
                .SubstringAfter("channel/", StringComparison.OrdinalIgnoreCase);
        }

        public string GetChannelTitle()
        {
            return _root
                .QuerySelectorOrThrow("meta[property=\"og:title\"]")
                .GetAttributeOrThrow("content");
        }

        public string GetChannelLogoUrl()
        {
            return _root
                .QuerySelectorOrThrow("meta[property=\"og:image\"]")
                .GetAttributeOrThrow("content");
        }
    }

    internal partial class ChannelPage
    {
        public static ChannelPage Parse(string raw)
        {
            return new ChannelPage(Html.Parse(raw));
        }

        public static async Task<ChannelPage> GetAsync(YoutubeHttpClient httpClient, string id)
        {
            return await Retry.WrapAsync(async () =>
            {
                var url = $"https://www.youtube.com/channel/{id}?hl=en";
                var raw = await httpClient.GetStringAsync(url);

                var result = Parse(raw);

                if (!result.IsOk())
                    throw TransientFailureException.Generic("Channel page is broken.");

                return result;
            });
        }

        public static async Task<ChannelPage> GetByUserNameAsync(YoutubeHttpClient httpClient, string userName)
        {
            return await Retry.WrapAsync(async () =>
            {
                var url = $"https://www.youtube.com/user/{userName}?hl=en";
                var raw = await httpClient.GetStringAsync(url);

                var result = Parse(raw);

                if (!result.IsOk())
                    throw TransientFailureException.Generic("Channel page is broken.");

                return result;
            });
        }
    }
}