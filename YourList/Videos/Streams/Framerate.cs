using System;

namespace YourList.Videos.Streams
{
    /// <summary>
    ///     Encapsulates framerate.
    /// </summary>
    public readonly partial struct Framerate
    {
        /// <summary>
        ///     Framerate as frames per second.
        /// </summary>
        public double FramesPerSecond { get; }

        /// <summary>
        ///     Initializes an instance of <see cref="Framerate" />.
        /// </summary>
        public Framerate(double framesPerSecond)
        {
            FramesPerSecond = framesPerSecond;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{FramesPerSecond:N0} FPS";
        }
    }

    public partial struct Framerate : IComparable<Framerate>, IEquatable<Framerate>
    {
        /// <inheritdoc />
        public int CompareTo(Framerate other)
        {
            return FramesPerSecond.CompareTo(other.FramesPerSecond);
        }

        /// <inheritdoc />
        public bool Equals(Framerate other)
        {
            return CompareTo(other) == 0;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Framerate other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(FramesPerSecond);
        }

        /// <summary>
        ///     Equality check.
        /// </summary>
        public static bool operator ==(Framerate left, Framerate right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Equality check.
        /// </summary>
        public static bool operator !=(Framerate left, Framerate right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Comparison.
        /// </summary>
        public static bool operator >(Framerate left, Framerate right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        ///     Comparison.
        /// </summary>
        public static bool operator <(Framerate left, Framerate right)
        {
            return left.CompareTo(right) < 0;
        }
    }
}