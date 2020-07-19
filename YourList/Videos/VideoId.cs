using System;
using System.Text.RegularExpressions;
using YourList.Internal.Extensions;

namespace YourList.Videos
{
    /// <summary>
    ///     Encapsulates a valid YouTube video ID.
    /// </summary>
    public readonly partial struct VideoId
    {
        /// <summary>
        ///     ID as a string.
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     Initializes an instance of <see cref="VideoId" />.
        /// </summary>
        public VideoId(string idOrUrl)
        {
            Value = TryNormalize(idOrUrl) ??
                    throw new ArgumentException($"Invalid YouTube video ID or URL: '{idOrUrl}'.");
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }
    }

    public partial struct VideoId : IEquatable<VideoId>
    {
        /// <inheritdoc />
        public bool Equals(VideoId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is VideoId other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        /// <summary>
        ///     Equality check.
        /// </summary>
        public static bool operator ==(VideoId left, VideoId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Equality check.
        /// </summary>
        public static bool operator !=(VideoId left, VideoId right)
        {
            return !(left == right);
        }
    }

    public partial struct VideoId
    {
        /// <summary>
        ///     Converts string to ID.
        /// </summary>
        public static implicit operator VideoId(string idOrUrl)
        {
            return new VideoId(idOrUrl);
        }

        /// <summary>
        ///     Converts ID to string.
        /// </summary>
        public static implicit operator string(VideoId id)
        {
            return id.ToString();
        }
    }

    public partial struct VideoId
    {
        private static bool IsValid(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            // Video IDs are always 11 characters
            if (id.Length != 11)
                return false;

            return !Regex.IsMatch(id, @"[^0-9a-zA-Z_\-]");
        }

        private static string? TryNormalize(string? idOrUrl)
        {
            if (string.IsNullOrWhiteSpace(idOrUrl))
                return null;

            // Id
            // yIVRs6YSbOM
            if (IsValid(idOrUrl))
                return idOrUrl;

            // Regular URL
            // https://www.youtube.com/watch?v=yIVRs6YSbOM
            var regularMatch = Regex.Match(idOrUrl, @"youtube\..+?/watch.*?v=(.*?)(?:&|/|$)").Groups[1].Value;
            if (!string.IsNullOrWhiteSpace(regularMatch) && IsValid(regularMatch))
                return regularMatch;

            // Short URL
            // https://youtu.be/yIVRs6YSbOM
            var shortMatch = Regex.Match(idOrUrl, @"youtu\.be/(.*?)(?:\?|&|/|$)").Groups[1].Value;
            if (!string.IsNullOrWhiteSpace(shortMatch) && IsValid(shortMatch))
                return shortMatch;

            // Embed URL
            // https://www.youtube.com/embed/yIVRs6YSbOM
            var embedMatch = Regex.Match(idOrUrl, @"youtube\..+?/embed/(.*?)(?:\?|&|/|$)").Groups[1].Value;
            if (!string.IsNullOrWhiteSpace(embedMatch) && IsValid(embedMatch))
                return embedMatch;

            // Invalid input
            return null;
        }

        /// <summary>
        ///     Attempts to parse the specified string as a video ID or URL.
        ///     Returns null in case of failure.
        /// </summary>
        public static VideoId? TryParse(string? idOrUrl)
        {
            return TryNormalize(idOrUrl)?.Pipe(id => new VideoId(id));
        }
    }
}