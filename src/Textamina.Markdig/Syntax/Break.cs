using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a thematic break.
    /// </summary>
    public class Break : BlockLeaf
    {
        public static readonly BlockBuilder Builder = new BuilderInternal();

        private class BuilderInternal : BlockBuilder
        {
            public override bool Match(ref StringLiner liner, ref Block block)
            {
                liner.SkipLeadingSpaces3();

                // 4.1 Thematic breaks 
                // A line consisting of 0-3 spaces of indentation, followed by a sequence of three or more matching -, _, or * characters, each followed optionally by any number of spaces
                var c = liner.Current;

                int count = 0;
                var matchChar = (char)0;
                while (!liner.IsEol)
                {
                    if (count == 0 && (c == '-' || c == '_' || c == '*'))
                    {
                        matchChar = c;
                        count++;
                    }
                    else if (c != matchChar && !Utility.IsSpace(c))
                    {
                        return false;
                    }
                    c = liner.NextChar();
                }

                block = new Break();
                return true;
            }
        }
    }
}