namespace YourList.Exceptions
{
    public class VideoRequiresPurchaseException : VideoUnplayableException
    {
        /// <summary>
        ///     Initializes an instance of <see cref="VideoRequiresPurchaseException" />
        /// </summary>
        public VideoRequiresPurchaseException(string message, string previewVideoId) : base(message)
        {
            PreviewVideoId = previewVideoId;
        }

        /// <summary>
        ///     ID of a free preview video for this video.
        /// </summary>
        public string PreviewVideoId { get; }

        internal static VideoRequiresPurchaseException Preview(string videoId, string previewVideoId)
        {
            var message = $@"
Video '{videoId}' is unplayable because it requires purchase.
Streams are not available for this video.
There is a preview video available: '{previewVideoId}'.";

            return new VideoRequiresPurchaseException(message, previewVideoId);
        }
    }
}