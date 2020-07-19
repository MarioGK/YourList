using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourList.Internal.Extensions;
using YourList.Models;
using YourList.ReverseEngineering;
using YourList.ReverseEngineering.Responses;
using YourList.Videos;

namespace YourList.Playlists
{
    /// <summary>
    ///     Queries related to YouTube playlists.
    /// </summary>
    public class PlaylistClient
    {
        private readonly YoutubeHttpClient _httpClient;

        /// <summary>
        ///     Initializes an instance of <see cref="PlaylistClient" />.
        /// </summary>
        internal PlaylistClient(YoutubeHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        ///     Gets the metadata associated with the specified playlist.
        /// </summary>
        public async Task<Playlist> GetAsync(string id)
        {
            var response = await PlaylistResponse.GetAsync(_httpClient, id);

            var thumbnails = response
                .GetVideos()
                .FirstOrDefault()?
                .GetId()
                .Pipe(i => new ThumbnailSet(i));

            return new Playlist(
                id,
                response.GetTitle(),
                response.TryGetAuthor(),
                response.TryGetDescription() ?? "",
                thumbnails);
        }

        /// <summary>
        ///     Enumerates videos included in the specified playlist.
        /// </summary>
        public async IAsyncEnumerable<Video> GetVideosAsync(string id)
        {
            var encounteredstrings = new HashSet<string>();

            var index = 0;
            while (true)
            {
                var response = await PlaylistResponse.GetAsync(_httpClient, id, index);

                var countDelta = 0;
                foreach (var video in response.GetVideos())
                {
                    var videoId = video.GetId();

                    // Skip already encountered videos
                    if (!encounteredstrings.Add(videoId))
                        continue;

                    //TODO
                    yield return null;

                    countDelta++;
                }

                // Videos loop around, so break when we stop seeing new videos
                if (countDelta <= 0)
                    break;

                index += countDelta;
            }
        }
    }
}