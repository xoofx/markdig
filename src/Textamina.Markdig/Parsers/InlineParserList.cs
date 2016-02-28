using System.Collections.Generic;

namespace Textamina.Markdig.Parsers
{
    public class InlineParserList : ParserList<InlineParser, InlineParserState>
    {
        public InlineParserList()
        {
        }

        public IDelimiterProcessor[] DelimiterProcessors { get; private set; }

        protected override void InitializeCore()
        {
            base.InitializeCore();
            var delimiterProcessors = new List<IDelimiterProcessor>();
            foreach (var parser in this)
            {
                var delimProcessor = parser as IDelimiterProcessor;
                if (delimProcessor != null)
                {
                    delimiterProcessors.Add(delimProcessor);
                }
            }
            DelimiterProcessors = delimiterProcessors.ToArray();
        }
    }
}