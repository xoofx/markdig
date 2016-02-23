


using System.IO;
using System.Text;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    internal class LineReader
    {
        private readonly TextReader reader;
        private readonly StringBuilder tempBuilder;
        private int lineIndex;

        public LineReader(TextReader reader)
        {
            this.reader = reader;
            this.tempBuilder = new StringBuilder();
        }

        public bool IsEof { get; private set; }

        public StringLine ReadLine()
        {
            tempBuilder.Clear();
            while (true)
            {
                var nextChar = reader.Read();
                if (nextChar < 0)
                {
                    IsEof = true;
                    break;
                }
                var c = (char)nextChar;

                // Go to next char, expecting most likely a \n, otherwise skip it
                // TODO: Should we treat it as an error in no \n is following?
                if (c == '\r')
                {
                    continue;
                }
                if (c == '\n')
                {
                    break;
                }

                // 2.3 Insecure characters '\0'
                tempBuilder.Append(c.EscapeInsecure());
            }
            return new StringLine(tempBuilder.ToString(), lineIndex++);
        }
    }
}