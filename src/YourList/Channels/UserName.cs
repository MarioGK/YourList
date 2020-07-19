using System;
using System.Text.RegularExpressions;
using YourList.Internal.Extensions;

namespace YourList.Channels
{
    /// <summary>
    ///     Encapsulates a valid YouTube user name.
    /// </summary>
    public readonly partial struct UserName
    {
        /// <summary>
        ///     User name as a string.
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     Initializes an instance of <see cref="UserName" />.
        /// </summary>
        public UserName(string nameOrUrl)
        {
            Value = TryNormalize(nameOrUrl) ??
                    throw new ArgumentException($"Invalid YouTube username or URL: '{nameOrUrl}'.");
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }
    }

    public partial struct UserName : IEquatable<UserName>
    {
        /// <inheritdoc />
        public bool Equals(UserName other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is UserName other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        /// <summary>
        ///     Equality check.
        /// </summary>
        public static bool operator ==(UserName left, UserName right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Equality check.
        /// </summary>
        public static bool operator !=(UserName left, UserName right)
        {
            return !(left == right);
        }
    }

    public partial struct UserName
    {
        /// <summary>
        ///     Converts string to user name.
        /// </summary>
        public static implicit operator UserName(string nameOrUrl)
        {
            return new UserName(nameOrUrl);
        }

        /// <summary>
        ///     Converts user name to string.
        /// </summary>
        public static implicit operator string(UserName id)
        {
            return id.ToString();
        }
    }

    public partial struct UserName
    {
        private static bool IsValid(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Usernames can't be longer than 20 characters
            if (name.Length > 20)
                return false;

            return !Regex.IsMatch(name, @"[^0-9a-zA-Z]");
        }

        private static string? TryNormalize(string? nameOrUrl)
        {
            if (string.IsNullOrWhiteSpace(nameOrUrl))
                return null;

            // Name
            // TheTyrrr
            if (IsValid(nameOrUrl))
                return nameOrUrl;

            // URL
            // https://www.youtube.com/user/TheTyrrr
            var regularMatch = Regex.Match(nameOrUrl, @"youtube\..+?/user/(.*?)(?:\?|&|/|$)").Groups[1].Value;
            if (!string.IsNullOrWhiteSpace(regularMatch) && IsValid(regularMatch))
                return regularMatch;

            // Invalid input
            return null;
        }

        /// <summary>
        ///     Attempts to parse the specified string as a username or URL.
        ///     Returns null in case of failure.
        /// </summary>
        public static UserName? TryParse(string? nameOrUrl)
        {
            return TryNormalize(nameOrUrl)?.Pipe(name => new UserName(name));
        }
    }
}