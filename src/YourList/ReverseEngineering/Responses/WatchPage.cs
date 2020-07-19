using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using YourList.Exceptions;
using YourList.Internal;
using YourList.Internal.Extensions;

namespace YourList.ReverseEngineering.Responses
{
    internal partial class WatchPage
    {
        private readonly IHtmlDocument _root;

        public WatchPage(IHtmlDocument root)
        {
            _root = root;
        }

        private bool IsOk()
        {
            return _root.Body.QuerySelector("#player") != null;
        }

        public bool IsVideoAvailable()
        {
            return _root
                .QuerySelector("meta[property=\"og:url\"]") != null;
        }

        public long? TryGetVideoLikeCount()
        {
            return _root
                .Source
                .Text
                .Pipe(s => Regex.Match(s, @"""label""\s*:\s*""([\d,\.]+) likes""").Groups[1].Value)
                .NullIfWhiteSpace()?
                .StripNonDigit()
                .ParseLong();
        }

        public long? TryGetVideoDislikeCount()
        {
            return _root
                .Source
                .Text
                .Pipe(s => Regex.Match(s, @"""label""\s*:\s*""([\d,\.]+) dislikes""").Groups[1].Value)
                .NullIfWhiteSpace()?
                .StripNonDigit()
                .ParseLong();
        }

        public PlayerConfig? TryGetPlayerConfig()
        {
            return _root
                .GetElementsByTagName("script")
                .Select(e => e.Text())
                .Select(s => Regex.Match(s,
                        @"ytplayer\.config = (?<Json>\{[^\{\}]*(((?<Open>\{)[^\{\}]*)+((?<Close-Open>\})[^\{\}]*)+)*(?(Open)(?!))\})")
                    .Groups["Json"].Value)
                .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?
                .NullIfWhiteSpace()?
                .Pipe(Json.Parse)
                .Pipe(j => new PlayerConfig(j));
        }
    }

    internal partial class WatchPage
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

            public PlayerResponse GetPlayerResponse()
            {
                return _root
                    .GetProperty("args")
                    .GetProperty("player_response")
                    .GetString()
                    .Pipe(PlayerResponse.Parse);
            }

            private IEnumerable<VideoInfoResponse.StreamInfo> GetMuxedStreams()
            {
                return Fallback.ToEmpty(
                    _root
                        .GetProperty("args")
                        .GetPropertyOrNull("url_encoded_fmt_stream_map")?
                        .GetString()?
                        .Split(",")
                        .Select(Url.SplitQuery)
                        .Select(d => new VideoInfoResponse.StreamInfo(d))
                );
            }

            private IEnumerable<VideoInfoResponse.StreamInfo> GetAdaptiveStreams()
            {
                return Fallback.ToEmpty(
                    _root
                        .GetProperty("args")
                        .GetPropertyOrNull("adaptive_fmts")?
                        .GetString()?
                        .Split(",")
                        .Select(Url.SplitQuery)
                        .Select(d => new VideoInfoResponse.StreamInfo(d))
                );
            }

            public IEnumerable<VideoInfoResponse.StreamInfo> GetStreams()
            {
                return GetMuxedStreams().Concat(GetAdaptiveStreams());
            }
        }
    }

    internal partial class WatchPage
    {
        public static WatchPage Parse(string raw)
        {
            return new WatchPage(Html.Parse(raw));
        }

        public static async Task<WatchPage> GetAsync(YoutubeHttpClient httpClient, string id)
        {
            return await Retry.WrapAsync(async () =>
            {
                var url = $"https://youtube.com/watch?v={id}&bpctr=9999999999&hl=en";
                var raw = await httpClient.GetStringAsync(url);

                var result = Parse(raw);

                if (!result.IsOk())
                    throw TransientFailureException.Generic("Video watch page is broken.");

                if (!result.IsVideoAvailable())
                    throw VideoUnavailableException.Unavailable(id);

                return result;
            });
        }
    }
}