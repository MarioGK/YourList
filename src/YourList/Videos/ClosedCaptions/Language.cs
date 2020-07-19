using System;

namespace YourList.Videos.ClosedCaptions
{
    /// <summary>
    ///     Encapsulates a human written language.
    /// </summary>
    public readonly partial struct Language
    {
        /// <summary>
        ///     ISO 639-1 code of this language.
        /// </summary>
        public string Code { get; }

        /// <summary>
        ///     Full English name of this language.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Initializes an instance of <see cref="Language" />.
        /// </summary>
        public Language(string code, string name)
        {
            Code = code;
            Name = name;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Code} ({Name})";
        }
    }

    public partial struct Language : IEquatable<Language>
    {
        /// <inheritdoc />
        public bool Equals(Language other)
        {
            return string.Equals(Code, other.Code, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Language other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Code);
        }

        /// <summary>
        ///     Equality check.
        /// </summary>
        public static bool operator ==(Language left, Language right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Equality check.
        /// </summary>
        public static bool operator !=(Language left, Language right)
        {
            return !(left == right);
        }
    }
}