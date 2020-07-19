using System;
using System.Collections.Generic;
using System.Linq;

namespace YourList.Videos.ClosedCaptions
{
    /// <summary>
    ///     Track that contains closed captions in a specific language.
    /// </summary>
    public class ClosedCaptionTrack
    {
        /// <summary>
        ///     Initializes an instance of <see cref="ClosedCaptionTrack" />.
        /// </summary>
        public ClosedCaptionTrack(IReadOnlyList<ClosedCaption> captions)
        {
            Captions = captions;
        }

        /// <summary>
        ///     Closed captions.
        /// </summary>
        public IReadOnlyList<ClosedCaption> Captions { get; }

        /// <summary>
        ///     Gets the caption displayed at the specified point in time.
        ///     Returns null if not found.
        /// </summary>
        public ClosedCaption? TryGetByTime(TimeSpan time)
        {
            return Captions.FirstOrDefault(c => time >= c.Offset && time <= c.Offset + c.Duration);
        }
    }
}