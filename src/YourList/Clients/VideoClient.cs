using System.Threading.Tasks;
using YourList.ReverseEngineering;
using YourList.ReverseEngineering.Responses;
using YourList.Videos.Streams;

namespace YourList.Videos
{
    /// <summary>
    ///     Queries related to YouTube videos.
    /// </summary>
    public class VideoClient
    {
        private readonly YoutubeHttpClient _httpClient;

        /// <summary>
        ///     Initializes an instance of <see cref="VideoClient" />.
        /// </summary>
        internal VideoClient(YoutubeHttpClient httpClient)
        {
            _httpClient = httpClient;

            Streams = new StreamsClient(httpClient);
        }

        /// <summary>
        ///     Queries related to media streams of YouTube videos.
        /// </summary>
        public StreamsClient Streams { get; }

        /// <summary>
        ///     Gets the metadata associated with the specified video.
        /// </summary>
        public async Task<Video> GetAsync(string id)
        {
            var videoInfoResponse = await VideoInfoResponse.GetAsync(_httpClient, id);
            var playerResponse = videoInfoResponse.GetPlayerResponse();

            var watchPage = await WatchPage.GetAsync(_httpClient, id);

            //TODO
            return null;
        }
    }
}