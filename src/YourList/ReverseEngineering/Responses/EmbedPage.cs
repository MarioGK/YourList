using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using YourList.Internal;
using YourList.Internal.Extensions;

namespace YourList.ReverseEngineering.Responses
{
    internal partial class EmbedPage
    {
        private readonly IHtmlDocument _root;

        public EmbedPage(IHtmlDocument root)
        {
            _root = root;
        }

        public PlayerConfig? TryGetPlayerConfig()
        {
            return _root
                .GetElementsByTagName("script")
                .Select(e => e.Text())
                .Select(s => Regex.Match(s, @"yt\.setConfig\({'PLAYER_CONFIG':(.*)}\);").Groups[1].Value)
                .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?
                .NullIfWhiteSpace()?
                .Pipe(Json.Parse)
                .Pipe(j => new PlayerConfig(j));
        }
    }

    internal partial class EmbedPage
    {
        public class PlayerConfig
        {
            private readonly JsonElement _root;

            public PlayerConfig(JsonElement root)
            {
                _root = root;
            }

            public string GetPlayerSourceUrl()
            {
                return _root
                    .GetProperty("assets")
                    .GetProperty("js")
                    .GetString()
                    .Pipe(s => "https://youtube.com" + s);
            }
        }
    }

    internal partial class EmbedPage
    {
        public static EmbedPage Parse(string raw)
        {
            return new EmbedPage(Html.Parse(raw));
        }

        public static async Task<EmbedPage> GetAsync(YoutubeHttpClient httpClient, string id)
        {
            return await Retry.WrapAsync(async () =>
            {
                var url = $"https://youtube.com/embed/{id}?hl=en";
                var raw = await httpClient.GetStringAsync(url);

                return Parse(raw);
            });
        }
    }
}