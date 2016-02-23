using System.Collections.Generic;
using System.Text;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class InlineParserState
    {
        public InlineParserState(StringBuilderCache stringBuilders, Document document)
        {
            StringBuilders = stringBuilders;
            Document = document;
            OpenedInlines = new HashSet<Inline>();
        }

        public LeafBlock Block { get; internal set; }

        public StringLineGroup Lines;

        public Inline Inline { get; set; }

        public readonly HashSet<Inline> OpenedInlines;

        public readonly Document Document;

        public StringBuilderCache StringBuilders { get;  }
    }
}