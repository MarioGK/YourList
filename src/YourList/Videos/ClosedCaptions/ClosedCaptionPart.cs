using System;

namespace YourList.Videos.ClosedCaptions
{
    /// <summary>
    ///     Part of a closed caption (usually a single word).
    /// </summary>
    public class ClosedCaptionPart
    {
        /// <summary>
        ///     Initializes an instance of <see cref="ClosedCaptionPart" />.
        /// </summary>
        public ClosedCaptionPart(string text, TimeSpan offset)
        {
            Text = text;
            Offset = offset;
        }

        /// <summary>
        ///     Text displayed by this caption part.
        /// </summary>
        public string Text { get; }

        /// <summary>
        ///     Time at which this caption part starts being displayed (relative to the caption's own offset).
        /// </summary>
        public TimeSpan Offset { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Text;
        }
    }
}