using Markdig.Helpers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Helpers for the <see cref="ICharIterator"/> class.
    /// </summary>
    public static class CharIteratorHelper
    {
        public static bool TrimStartAndCountNewLines<T>(ref T iterator, out int countNewLines) where T : ICharIterator
        {
            countNewLines = 0;
            var c = iterator.CurrentChar;
            bool hasWhitespaces = false;
            while (c.IsWhitespace())
            {
                if (c == '\n')
                {
                    countNewLines++;
                }
                hasWhitespaces = true;
                c = iterator.NextChar();
            }
            return hasWhitespaces;
        }
    }
}