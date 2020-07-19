using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using YourList.Internal;
using YourList.Internal.Extensions;

namespace YourList.ReverseEngineering.Responses
{
    internal partial class DashManifest
    {
        private readonly XElement _root;

        public DashManifest(XElement root)
        {
            _root = root;
        }

        public IEnumerable<StreamInfo> GetStreams()
        {
            return _root
                .Descendants("Representation")
                .Where(x => x
                    .Descendants("Initialization")
                    .FirstOrDefault()?
                    .Attribute("sourceURL")?
                    .Value
                    .Contains("sq/") != true)
                .Where(x => !string.IsNullOrWhiteSpace(x.Attribute("codecs")?.Value))
                .Select(x => new StreamInfo(x));
        }
    }

    internal partial class DashManifest
    {
        public class StreamInfo : IStreamInfoProvider
        {
            private readonly XElement _root;

            public StreamInfo(XElement root)
            {
                _root = root;
            }

            public int GetTag()
            {
                return (int) _root.Attribute("id");
            }

            public string GetUrl()
            {
                return (string) _root.Element("BaseURL");
            }

            // DASH streams don't have signatures
            public string? TryGetSignature()
            {
                return null;
            }

            // DASH streams don't have signatures
            public string? TryGetSignatureParameter()
            {
                return null;
            }

            public long? TryGetContentLength()
            {
                return (long?) _root.Attribute("contentLength") ??
                       GetUrl().Pipe(s => Regex.Match(s, @"[/\?]clen[/=](\d+)").Groups[1].Value).NullIfWhiteSpace()
                           ?.ParseLong();
            }

            public long GetBitrate()
            {
                return (long) _root.Attribute("bandwidth");
            }

            public string GetContainer()
            {
                return GetUrl()
                    .Pipe(s => Regex.Match(s, @"mime[/=]\w*%2F([\w\d]*)").Groups[1].Value)
                    .Pipe(WebUtility.UrlDecode)!;
            }

            public string? TryGetAudioCodec()
            {
                return IsAudioOnly()
                    ? (string) _root.Attribute("codecs")
                    : null;
            }

            public string? TryGetVideoCodec()
            {
                return IsAudioOnly()
                    ? null
                    : (string) _root.Attribute("codecs");
            }

            public string? TryGetVideoQualityLabel()
            {
                return null;
            }

            public int? TryGetVideoWidth()
            {
                return (int?) _root.Attribute("width");
            }

            public int? TryGetVideoHeight()
            {
                return (int?) _root.Attribute("height");
            }

            public int? TryGetFramerate()
            {
                return (int?) _root.Attribute("frameRate");
            }

            private bool IsAudioOnly()
            {
                return _root
                    .Element("AudioChannelConfiguration") != null;
            }
        }
    }

    internal partial class DashManifest
    {
        public static DashManifest Parse(string raw)
        {
            return new DashManifest(Xml.Parse(raw));
        }

        public static async Task<DashManifest> GetAsync(YoutubeHttpClient httpClient, string url)
        {
            return await Retry.WrapAsync(async () =>
            {
                var raw = await httpClient.GetStringAsync(url);
                return Parse(raw);
            });
        }

        public static string? TryGetSignatureFromUrl(string url)
        {
            return Regex.Match(url, "/s/(.*?)(?:/|$)").Groups[1].Value.NullIfWhiteSpace();
        }
    }
}