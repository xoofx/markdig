using System;

namespace Markdig.Helpers
{
    /// <summary>
    /// Represents a character or set of characters that represent a separation
    /// between two lines of text
    /// </summary>
    public enum Newline
    {
        None,
        CarriageReturn,
        LineFeed,
        CarriageReturnLineFeed
    }

    public static class NewlineExtensions
    {
        public static string AsString(this Newline newline)
        {
            if (newline == Newline.CarriageReturnLineFeed)
            {
                return "\r\n";
            }
            if (newline == Newline.LineFeed)
            {
                return "\n";
            }
            if (newline == Newline.CarriageReturn)
            {
                return "\r";
            }
            return string.Empty;
        }

        public static int Length(this Newline newline) => newline switch
        {
            Newline.None => 0,
            Newline.CarriageReturn => 1,
            Newline.LineFeed => 1,
            Newline.CarriageReturnLineFeed => 2,
            _ => throw new NotSupportedException(),
        };
    }
}
