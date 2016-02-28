using System.Collections.Generic;

namespace Textamina.Markdig.Parsers
{
    public class InlineParserList : ParserList<InlineParser>
    {
        public InlineParserList()
        {
        }

        public InlineParser[] ClosingParsers { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            var closingParsers = new InlineParserList();
            foreach (var parser in this)
            {
                if (parser.OnCloseBlock != null)
                {
                    closingParsers.Add(parser);
                }
            }

            ClosingParsers = closingParsers.ToArray();
        }
    }
}