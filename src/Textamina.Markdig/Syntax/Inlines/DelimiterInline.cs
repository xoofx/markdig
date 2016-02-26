using System;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax.Inlines
{
    public abstract class DelimiterInline : ContainerInline
    {
        protected DelimiterInline(InlineParser parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            Parser = parser;
            IsActive = true;
        }

        public InlineParser Parser { get; }

        public DelimiterType Type { get; set; }

        /// <summary>
        /// Gets or sets the priority of this delimiter.
        /// </summary>
        public int Priority { get; set; }

        public bool IsActive { get; set; }

        public abstract string ToLiteral();

        public override string ToString()
        {
            return $"{ToLiteral()} {Type}";
        }
    }
}