using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using YourList.Internal;

namespace YourList.ReverseEngineering.Responses
{
    internal partial class ClosedCaptionTrackResponse
    {
        private readonly XElement _root;

        public ClosedCaptionTrackResponse(XElement root)
        {
            _root = root;
        }

        public IEnumerable<ClosedCaption> GetClosedCaptions()
        {
            return _root
                .Descendants("p")
                .Select(x => new ClosedCaption(x));
        }
    }

    internal partial class ClosedCaptionTrackResponse
    {
        public class ClosedCaption
        {
            private readonly XElement _root;

            public ClosedCaption(XElement root)
            {
                _root = root;
            }

            public string GetText()
            {
                return (string) _root;
            }

            public TimeSpan GetOffset()
            {
                return TimeSpan.FromMilliseconds((double?) _root.Attribute("t") ?? 0);
            }

            public TimeSpan GetDuration()
            {
                return TimeSpan.FromMilliseconds((double?) _root.Attribute("d") ?? 0);
            }

            public IEnumerable<ClosedCaptionPart> GetParts()
            {
                return _root
                    .Elements("s")
                    .Select(x => new ClosedCaptionPart(x));
            }
        }

        public class ClosedCaptionPart
        {
            private readonly XElement _root;

            public ClosedCaptionPart(XElement root)
            {
                _root = root;
            }

            public string GetText()
            {
                return (string) _root;
            }

            public TimeSpan GetOffset()
            {
                return TimeSpan.FromMilliseconds((double?) _root.Attribute("t") ?? 0);
            }
        }
    }

    internal partial class ClosedCaptionTrackResponse
    {
        public static ClosedCaptionTrackResponse Parse(string raw)
        {
            return new ClosedCaptionTrackResponse(Xml.Parse(raw));
        }

        public static async Task<ClosedCaptionTrackResponse> GetAsync(YoutubeHttpClient httpClient, string url)
        {
            return await Retry.WrapAsync(async () =>
            {
                // Enforce known format
                var urlWithFormat = Url.SetQueryParameter(url, "format", "3");

                var raw = await httpClient.GetStringAsync(urlWithFormat);

                return Parse(raw);
            });
        }
    }
}