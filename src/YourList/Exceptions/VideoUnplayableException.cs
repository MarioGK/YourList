namespace YourList.Exceptions
{
    public class VideoUnplayableException : YoutubeExplodeException
    {
        /// <summary>
        ///     Initializes an instance of <see cref="VideoUnplayableException" />.
        /// </summary>
        public VideoUnplayableException(string message)
            : base(message)
        {
        }

        internal static VideoUnplayableException Unplayable(string id, string? reason = null)
        {
            var message = $@"
Video '{id}' is unplayable.
Streams are not available for this video.
In most cases, this error indicates that there are some restrictions in place that prevent watching this video.

Reason: {reason}";

            return new VideoUnplayableException(message.Trim());
        }

        internal static VideoUnplayableException LiveStream(string id)
        {
            var message = $@"
Video '{id}' is an ongoing live stream.
Streams are not available for this video.
Please wait until the live stream finishes and try again.";

            return new VideoUnplayableException(message.Trim());
        }

        internal static VideoUnplayableException NotLiveStream(string id)
        {
            var message = $@"
Video '{id}' is not an ongoing live stream.
Live stream manifest is not available for this video.";

            return new VideoUnplayableException(message.Trim());
        }
    }
}