namespace YourList.Models
{
    /// <summary>
    ///     Set of thumbnails for a video.
    /// </summary>
    public class ThumbnailSet
    {
        private readonly string _string;

        /// <summary>
        ///     Initializes an instance of <see cref="ThumbnailSet" />.
        /// </summary>
        public ThumbnailSet(string id)
        {
            _string = id;
        }

        /// <summary>
        ///     Low resolution thumbnail URL.
        /// </summary>
        public string LowResUrl => $"https://img.youtube.com/vi/{_string}/default.jpg";

        /// <summary>
        ///     Medium resolution thumbnail URL.
        /// </summary>
        public string MediumResUrl => $"https://img.youtube.com/vi/{_string}/mqdefault.jpg";

        /// <summary>
        ///     High resolution thumbnail URL.
        /// </summary>
        public string HighResUrl => $"https://img.youtube.com/vi/{_string}/hqdefault.jpg";

        /// <summary>
        ///     Standard resolution thumbnail URL.
        ///     Not always available.
        /// </summary>
        public string StandardResUrl => $"https://img.youtube.com/vi/{_string}/sddefault.jpg";

        /// <summary>
        ///     Max resolution thumbnail URL.
        ///     Not always available.
        /// </summary>
        public string MaxResUrl => $"https://img.youtube.com/vi/{_string}/maxresdefault.jpg";
    }
}