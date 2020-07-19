namespace YourList.Exceptions
{
    public class VideoUnavailableException : VideoUnplayableException
    {
        /// <summary>
        ///     Initializes an instance of <see cref="VideoUnavailableException" />.
        /// </summary>
        public VideoUnavailableException(string message)
            : base(message)
        {
        }

        internal static VideoUnavailableException Unavailable(string id)
        {
            var message = $@"
Video '{id}' is unavailable.
In most cases, this error indicates that the video doesn't exist, is private, or has been taken down.
If you can however open this video in your browser in incognito mode, it most likely means that YouTube changed something, which broke this library.
Please report this issue on GitHub in that case.";

            return new VideoUnavailableException(message.Trim());
        }
    }
}