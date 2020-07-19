using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using YourList.Internal;
using YourList.Internal.Extensions;

namespace YourList.ReverseEngineering.Responses
{
    internal partial class PlayerResponse
    {
        private readonly JsonElement _root;

        public PlayerResponse(JsonElement root)
        {
            _root = root;
        }

        private string GetVideoPlayabilityStatus()
        {
            return _root
                .GetProperty("playabilityStatus")
                .GetProperty("status")
                .GetString();
        }

        public string? TryGetVideoPlayabilityError()
        {
            return _root
                .GetPropertyOrNull("playabilityStatus")?
                .GetPropertyOrNull("reason")?
                .GetString();
        }

        public bool IsVideoAvailable()
        {
            return !string.Equals(GetVideoPlayabilityStatus(), "error", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsVideoPlayable()
        {
            return string.Equals(GetVideoPlayabilityStatus(), "ok", StringComparison.OrdinalIgnoreCase);
        }

        public string GetVideoTitle()
        {
            return _root
                .GetProperty("videoDetails")
                .GetProperty("title")
                .GetString();
        }

        public string GetVideoAuthor()
        {
            return _root
                .GetProperty("videoDetails")
                .GetProperty("author")
                .GetString();
        }

        public DateTimeOffset GetVideoUploadDate()
        {
            return _root
                .GetProperty("microformat")
                .GetProperty("playerMicroformatRenderer")
                .GetProperty("uploadDate")
                .GetDateTimeOffset();
        }

        public string GetVideoChannelId()
        {
            return _root
                .GetProperty("videoDetails")
                .GetProperty("channelId")
                .GetString();
        }

        public TimeSpan GetVideoDuration()
        {
            return _root
                .GetProperty("videoDetails")
                .GetProperty("lengthSeconds")
                .GetString()
                .ParseDouble()
                .Pipe(TimeSpan.FromSeconds);
        }

        public IReadOnlyList<string> GetVideoKeywords()
        {
            return Fallback.ToEmpty(
                _root
                    .GetProperty("videoDetails")
                    .GetPropertyOrNull("keywords")?
                    .EnumerateArray()
                    .Select(j => j.GetString())
                    .ToArray()
            );
        }

        public string GetVideoDescription()
        {
            return _root
                .GetProperty("videoDetails")
                .GetProperty("shortDescription")
                .GetString();
        }

        public long? TryGetVideoViewCount()
        {
            return _root
                .GetProperty("videoDetails")
                .GetPropertyOrNull("viewCount")?
                .GetString()
                .ParseLong();
        }

        public string? TryGetPreviewstring()
        {
            return _root
                       .GetPropertyOrNull("playabilityStatus")?
                       .GetPropertyOrNull("errorScreen")?
                       .GetPropertyOrNull("playerLegacyDesktopYpcTrailerRenderer")?
                       .GetPropertyOrNull("trailerstring")?
                       .GetString() ??
                   _root
                       .GetPropertyOrNull("playabilityStatus")?
                       .GetPropertyOrNull("errorScreen")?
                       .GetPropertyOrNull("ypcTrailerRenderer")?
                       .GetPropertyOrNull("playerVars")?
                       .GetString()
                       .Pipe(Url.SplitQuery)
                       .GetValueOrDefault("video_id");
        }

        public bool IsLive()
        {
            return _root
                .GetProperty("videoDetails")
                .GetPropertyOrNull("isLive")?
                .GetBoolean() ?? false;
        }

        public string? TryGetDashManifestUrl()
        {
            return _root
                .GetPropertyOrNull("streamingData")?
                .GetPropertyOrNull("dashManifestUrl")?
                .GetString();
        }

        public string? TryGetHlsManifestUrl()
        {
            return _root
                .GetPropertyOrNull("streamingData")?
                .GetPropertyOrNull("hlsManifestUrl")?
                .GetString();
        }

        private IEnumerable<StreamInfo> GetMuxedStreams()
        {
            return Fallback.ToEmpty(
                _root
                    .GetPropertyOrNull("streamingData")?
                    .GetPropertyOrNull("formats")?
                    .EnumerateArray()
                    .Select(j => new StreamInfo(j))
                    .Where(s => !string.Equals(s.GetCodecs(), "unknown", StringComparison.OrdinalIgnoreCase))
            );
        }

        private IEnumerable<StreamInfo> GetAdaptiveStreams()
        {
            return Fallback.ToEmpty(
                _root
                    .GetPropertyOrNull("streamingData")?
                    .GetPropertyOrNull("adaptiveFormats")?
                    .EnumerateArray()
                    .Select(j => new StreamInfo(j))
                    .Where(s => !string.Equals(s.GetCodecs(), "unknown", StringComparison.OrdinalIgnoreCase))
            );
        }

        public IEnumerable<StreamInfo> GetStreams()
        {
            return GetMuxedStreams().Concat(GetAdaptiveStreams());
        }

        public IEnumerable<ClosedCaptionTrack> GetClosedCaptionTracks()
        {
            return Fallback.ToEmpty(
                _root
                    .GetPropertyOrNull("captions")?
                    .GetPropertyOrNull("playerCaptionsTracklistRenderer")?
                    .GetPropertyOrNull("captionTracks")?
                    .EnumerateArray()
                    .Select(j => new ClosedCaptionTrack(j))
            );
        }
    }

    internal partial class PlayerResponse
    {
        public class StreamInfo : IStreamInfoProvider
        {
            private readonly JsonElement _root;

            public StreamInfo(JsonElement root)
            {
                _root = root;
            }

            public int GetTag()
            {
                return _root
                    .GetProperty("itag")
                    .GetInt32();
            }

            public string GetUrl()
            {
                return _root
                           .GetPropertyOrNull("url")?
                           .GetString() ??
                       _root
                           .GetPropertyOrNull("cipher")?
                           .GetString()
                           .Pipe(Url.SplitQuery)["url"] ??
                       _root
                           .GetProperty("signatureCipher")
                           .GetString()
                           .Pipe(Url.SplitQuery)["url"];
            }

            public string? TryGetSignature()
            {
                return _root
                           .GetPropertyOrNull("cipher")?
                           .GetString()
                           .Pipe(Url.SplitQuery)
                           .GetValueOrDefault("s") ??
                       _root
                           .GetPropertyOrNull("signatureCipher")?
                           .GetString()
                           .Pipe(Url.SplitQuery)
                           .GetValueOrDefault("s");
            }

            public string? TryGetSignatureParameter()
            {
                return _root
                           .GetPropertyOrNull("cipher")?
                           .GetString()
                           .Pipe(Url.SplitQuery)
                           .GetValueOrDefault("sp") ??
                       _root
                           .GetPropertyOrNull("signatureCipher")?
                           .GetString()
                           .Pipe(Url.SplitQuery)
                           .GetValueOrDefault("sp");
            }

            public long? TryGetContentLength()
            {
                return _root
                           .GetPropertyOrNull("contentLength")?
                           .GetString()
                           .ParseLong() ??
                       GetUrl()
                           .Pipe(s => Regex.Match(s, @"[\?&]clen=(\d+)").Groups[1].Value)
                           .NullIfWhiteSpace()?
                           .ParseLong();
            }

            public long GetBitrate()
            {
                return _root
                    .GetProperty("bitrate")
                    .GetInt64();
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
                    .GetPropertyOrNull("qualityLabel")?
                    .GetString();
            }

            public int? TryGetVideoWidth()
            {
                return _root
                    .GetPropertyOrNull("width")?
                    .GetInt32();
            }

            public int? TryGetVideoHeight()
            {
                return _root
                    .GetPropertyOrNull("height")?
                    .GetInt32();
            }

            public int? TryGetFramerate()
            {
                return _root
                    .GetPropertyOrNull("fps")?
                    .GetInt32();
            }

            private string GetMimeType()
            {
                return _root
                    .GetProperty("mimeType")
                    .GetString();
            }

            private bool IsAudioOnly()
            {
                return GetMimeType()
                    .StartsWith("audio/", StringComparison.OrdinalIgnoreCase);
            }

            public string GetCodecs()
            {
                return GetMimeType()
                    .SubstringAfter("codecs=\"")
                    .SubstringUntil("\"");
            }
        }

        public class ClosedCaptionTrack
        {
            private readonly JsonElement _root;

            public ClosedCaptionTrack(JsonElement root)
            {
                _root = root;
            }

            public string GetUrl()
            {
                return _root
                    .GetProperty("baseUrl")
                    .GetString();
            }

            public string GetLanguageCode()
            {
                return _root
                    .GetProperty("languageCode")
                    .GetString();
            }

            public string GetLanguageName()
            {
                return _root
                    .GetProperty("name")
                    .GetProperty("simpleText")
                    .GetString();
            }

            public bool IsAutoGenerated()
            {
                return _root
                    .GetProperty("vssId")
                    .GetString()
                    .StartsWith("a.", StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    internal partial class PlayerResponse
    {
        public static PlayerResponse Parse(string raw)
        {
            return new PlayerResponse(Json.Parse(raw));
        }
    }
}