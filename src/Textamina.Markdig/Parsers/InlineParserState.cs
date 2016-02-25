using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers
{
    public class InlineParserState
    {
        public InlineParserState(StringBuilderCache stringBuilders, Document document)
        {
            StringBuilders = stringBuilders;
            Document = document;
            InlinesToClose = new List<Inline>();
        }

        public LeafBlock Block { get; internal set; }

        public Inline Inline { get; set; }

        public readonly List<Inline> InlinesToClose;

        public readonly Document Document;

        public StringBuilderCache StringBuilders { get;  }
    }
}