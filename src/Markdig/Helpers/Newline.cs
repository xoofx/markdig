using System;

namespace Markdig.Helpers
{
    /// <summary>
    /// Represents a character or set of characters that represent a separation
    /// between two lines of text
    /// </summary>
    public enum NewLine
    {
        None,
        CarriageReturn,
        LineFeed,
        CarriageReturnLineFeed
    }

    public static class NewLineExtensions
    {
        public static string AsString(this NewLine newLine)
        {
            if (newLine == NewLine.CarriageReturnLineFeed)
            {
                return "\r\n";
            }
            if (newLine == NewLine.LineFeed)
            {
                return "\n";
            }
            if (newLine == NewLine.CarriageReturn)
            {
                return "\r";
            }
            return string.Empty;
        }

        public static int Length(this NewLine newline) => newline switch
        {
            NewLine.None => 0,
            NewLine.CarriageReturn => 1,
            NewLine.LineFeed => 1,
            NewLine.CarriageReturnLineFeed => 2,
            _ => throw new NotSupportedException(),
        };
    }
}

