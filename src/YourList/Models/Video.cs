using System;
using YourList.Models;

namespace YourList.Videos
{
    /// <summary>
    ///     YouTube video metadata.
    /// </summary>
    public class Video
    {
        /// <summary>
        ///     Initializes an instance of <see cref="Video" />.
        /// </summary>
        public Video(
            string id,
            string title,
            string author,
            DateTimeOffset uploadDate,
            ThumbnailSet thumbnails)
        {
            Id = id;
            Title = title;
            Author = author;
            UploadDate = uploadDate;
            Thumbnails = thumbnails;
        }

        /// <summary>
        ///     Video ID.
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Video URL.
        /// </summary>
        public string Url => $"https://www.youtube.com/watch?v={Id}";

        /// <summary>
        ///     Video title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        ///     Video author.
        /// </summary>
        public string Author { get; }

        /// <summary>
        ///     Video upload date.
        /// </summary>
        public DateTimeOffset UploadDate { get; }

        /// <summary>
        ///     Available thumbnails for this video.
        /// </summary>
        public ThumbnailSet Thumbnails { get; }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"Video ({Title})";
        }
    }
}