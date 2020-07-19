using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YourList.Exceptions;
using YourList.Internal;
using YourList.Internal.Extensions;

namespace YourList.ReverseEngineering.Responses
{
    internal partial class VideoInfoResponse
    {
        private readonly IReadOnlyDictionary<string, string> _root;

        public VideoInfoResponse(IReadOnlyDictionary<string, string> root)
        {
            _root = root;
        }

        private string GetStatus()
        {
            return _root["status"];
        }

        public bool IsVideoAvailable()
        {
            return !string.Equals(GetStatus(), "fail", StringComparison.OrdinalIgnoreCase);
        }

        public PlayerResponse GetPlayerResponse()
        {
            return _root["player_response"]
                .Pipe(PlayerResponse.Parse);
        }

        private IEnumerable<StreamInfo> GetMuxedStreams()
        {
            return Fallback.ToEmpty(
                _root
                    .GetValueOrDefault("url_encoded_fmt_stream_map")?
                    .Split(",")
                    .Select(Url.SplitQuery)
                    .Select(d => new StreamInfo(d))
            );
        }

        private IEnumerable<StreamInfo> GetAdaptiveStreams()
        {
            return Fallback.ToEmpty(
                _root
                    .GetValueOrDefault("adaptive_fmts")?
                    .Split(",")
                    .Select(Url.SplitQuery)
                    .Select(d => new StreamInfo(d))
            );
        }

        public IEnumerable<StreamInfo> GetStreams()
        {
            return GetMuxedStreams().Concat(GetAdaptiveStreams());
        }
    }

    internal partial class VideoInfoResponse
    {
        public class StreamInfo : IStreamInfoProvider
        {
            private readonly IReadOnlyDictionary<string, string> _root;

            public StreamInfo(IReadOnlyDictionary<string, string> root)
            {
                _root = root;
            }

            public int GetTag()
            {
                return _root["itag"].ParseInt();
            }

            public string GetUrl()
            {
                return _root["url"];
            }

            public string? TryGetSignature()
            {
                return _root.GetValueOrDefault("s");
            }

            public string? TryGetSignatureParameter()
            {
                return _root.GetValueOrDefault("sp");
            }

            public long? TryGetContentLength()
            {
                return _root
                           .GetValueOrDefault("clen")?
                           .ParseLong() ??
                       GetUrl()
                           .Pipe(s => Regex.Match(s, @"clen=(\d+)").Groups[1].Value)
                           .NullIfWhiteSpace()?
                           .ParseLong();
            }

            public long GetBitrate()
            {
                return _root["bitrate"]
                    .ParseLong();
            }

            public string GetContainer()
            {
                return GetMimeType()
                    .SubstringUntil(";")
                    .SubstringAfter("/");
            }

            public string? TryGetAudioCodec()
            {
                return IsAudioOnly()
                    ? GetCodecs()
                    : GetCodecs().SubstringAfter(", ").NullIfWhiteSpace();
            }

            public string? TryGetVideoCodec()
            {
                return IsAudioOnly()
                    ? null
                    : GetCodecs().SubstringUntil(", ").NullIfWhiteSpace();
            }

            public string? TryGetVideoQualityLabel()
            {
                return _root
                    .GetValueOrDefault("quality_label");
            }

            public int? TryGetVideoWidth()
            {
                return _root
                    .GetValueOrDefault("size")?
                    .SubstringUntil("x")
                    .NullIfWhiteSpace()?
                    .ParseInt();
            }

            public int? TryGetVideoHeight()
            {
                return _root
                    .GetValueOrDefault("size")?
                    .SubstringAfter("x")
                    .NullIfWhiteSpace()?
                    .ParseInt();
            }

            public int? TryGetFramerate()
            {
                return _root
                    .GetValueOrDefault("fps")?
                    .ParseInt();
            }

            private string GetMimeType()
            {
                return _root["type"];
            }

            private bool IsAudioOnly()
            {
                return GetMimeType()
                    .StartsWith("audio/", StringComparison.OrdinalIgnoreCase);
            }

            private string GetCodecs()
            {
                return GetMimeType()
                    .SubstringAfter("codecs=\"")
                    .SubstringUntil("\"");
            }
        }
    }

    internal partial class VideoInfoResponse
    {
        public static VideoInfoResponse Parse(string raw)
        {
            return new VideoInfoResponse(Url.SplitQuery(raw));
        }

        public static async Task<VideoInfoResponse> GetAsync(YoutubeHttpClient httpClient, string videoId,
            string? sts = null)
        {
            return await Retry.WrapAsync(async () =>
            {
                var eurl = WebUtility.HtmlEncode($"https://youtube.googleapis.com/v/{videoId}");

                var url =
                    $"https://youtube.com/get_video_info?video_id={videoId}&el=embedded&eurl={eurl}&hl=en&sts={sts}";
                var raw = await httpClient.GetStringAsync(url);

                var result = Parse(raw);

                if (!result.IsVideoAvailable() || !result.GetPlayerResponse().IsVideoAvailable())
                    throw VideoUnavailableException.Unavailable(videoId);

                return result;
            });
        }
    }
}