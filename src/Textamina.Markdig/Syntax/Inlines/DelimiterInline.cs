namespace Textamina.Markdig.Syntax
{
    public class DelimiterInline : ContainerInline
    {
        public DelimiterInline()
        {
            Active = true;
        }

        /// <summary>
        /// The delimiter character found.
        /// </summary>
        public char DelimiterChar { get; set; }

        /// <summary>
        /// The number of delimiter characters found for this delimiter.
        /// </summary>
        public int DelimiterCount { get; set; }

        /// <summary>
        /// Gets or sets the priority of this delimiter.
        /// </summary>
        public int Priority { get; set; }

        public bool Active { get; set; }

        public static Inline FindMatchingOpen(Inline inline, int priority, char delimiterChar, int delimiterCount)
        {
            var delimiter = inline as DelimiterInline;
            if (delimiter != null && delimiter.Active && !delimiter.IsClosed && priority >= delimiter.Priority && delimiter.DelimiterChar == delimiterChar &&
                delimiterCount <= delimiter.DelimiterCount)
            {
                return delimiter;
            }

            if (inline.Parent != null)
            {
                return FindMatchingOpen(inline.Parent, priority, delimiterChar, delimiterCount);
            }

            return null;
        }
    }
}