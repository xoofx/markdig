using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public class FencedCodeBlockParser : BlockParser
    {
        public FencedCodeBlockParser()
        {
            OpeningCharacters = new[] {'`', '~'};
        }

        public override BlockState TryOpen(BlockParserState state)
        {
            // Else if the we have an indent, it is not valid
            if (state.IsCodeIndent)
            {
                return BlockState.None;
            }

            int count = 0;
            var line = state.Line;
            char c = line.CurrentChar;
            var matchChar = c;
            while (c != '\0')
            {
                if (c != matchChar)
                {
                    break;
                }
                count++;
                c = line.NextChar();
            }

            if (count < 3)
            {
                return BlockState.None;
            }

            // TODO: We need to count the number of leading space to remove them on each line
            var column = state.Column;

            // specs spaces: Is space and tabs? or only spaces? Use space and tab for this case
            while (c.IsSpaceOrTab())
            {
                c = line.NextChar();
            }
            string infoString;
            string argString = null;

            // An info string cannot contain any backsticks
            int firstSpace = -1;
            for (int i = line.Start; i <= line.End; i++)
            {
                c = line.Text[i];
                if (c == '`')
                {
                    return BlockState.None;
                }

                if (firstSpace < 0 && c.IsSpaceOrTab())
                {
                    firstSpace = i;
                }
            }

            if (firstSpace > 0)
            {
                infoString = line.Text.Substring(line.Start, firstSpace - line.Start);

                // Skip any spaces after info string
                firstSpace++;
                while (true)
                {
                    c = line[firstSpace];
                    if (c.IsSpaceOrTab())
                    {
                        firstSpace++;
                    }
                    else
                    {
                        break;
                    }
                }

                argString = line.Text.Substring(firstSpace, line.End - firstSpace + 1);
            }
            else
            {
                infoString = line.ToString();
            }

            // Store the number of matched string into the context
            state.NewBlocks.Push(new FencedCodeBlock(this)
            {
                Column = column,
                FencedChar = matchChar,
                FencedCharCount = count,
                IndentCount = state.Indent,
                Language = HtmlHelper.Unescape(infoString),
                Arguments = HtmlHelper.Unescape(argString),
            });

            // Discard the current line as it is already parsed
            return BlockState.ContinueDiscard;
        }

        public override BlockState TryContinue(BlockParserState state, Block block)
        {
            var fence = (FencedCodeBlock)block;
            var count = fence.FencedCharCount;
            var matchChar = fence.FencedChar;
            var c = state.CurrentChar;

            // Work on a copy of StringSlice
            var line = state.Line;
            while (c == matchChar)
            {
                c = line.NextChar();
                count--;
            }

            if (count <=0 && line.TrimEnd())
            {
                // Don't keep the last line
                return BlockState.BreakDiscard;
            }

            // Remove any indent spaces
            c = state.CurrentChar;
            var indentCount = fence.IndentCount;
            while (indentCount > 0 && c.IsSpace())
            {
                indentCount--;
                c = state.NextChar();
            }

            // TODO: It is unclear how to handle this correctly
            // Break only if Eof
            return BlockState.Continue;
        }
    }
}