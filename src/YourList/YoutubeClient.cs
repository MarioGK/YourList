using System;
using System.Net;
using System.Net.Http;
using YourList.Playlists;
using YourList.ReverseEngineering;
using YourList.Videos;

namespace YourList
{
    public class YoutubeClient
    {
        private static readonly Lazy<HttpClient> LazyHttpClient = new Lazy<HttpClient>(() =>
        {
            var handler = new HttpClientHandler();

            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            var httpClient = new HttpClient(handler, true);

            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36");

            return httpClient;
        });

        /// <summary>
        ///     Initializes an instance of <see cref="YoutubeClient" />.
        /// </summary>
        internal YoutubeClient(YoutubeHttpClient httpClient)
        {
            Videos = new VideoClient(httpClient);
            Playlists = new PlaylistClient(httpClient);
        }

        /// <summary>
        ///     Initializes an instance of <see cref="YoutubeClient" />.
        /// </summary>
        public YoutubeClient(HttpClient httpClient)
            : this(new YoutubeHttpClient(httpClient))
        {
        }

        /// <summary>
        ///     Initializes an instance of <see cref="YoutubeClient" />.
        /// </summary>
        public YoutubeClient()
            : this(LazyHttpClient.Value)
        {
        }

        /// <summary>
        ///     Queries related to YouTube videos.
        /// </summary>
        public VideoClient Videos { get; }

        /// <summary>
        ///     Queries related to YouTube playlists.
        /// </summary>
        public PlaylistClient Playlists { get; }
    }
}