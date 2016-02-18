using System.Runtime.CompilerServices;

namespace Textamina.Markdig.Parsing
{
    internal class Utility
    {
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsWhitespace(char c)
        {
            // 2.1 Characters and lines 
            // A whitespace character is a space(U + 0020), tab(U + 0009), newline(U + 000A), line tabulation (U + 000B), form feed (U + 000C), or carriage return (U + 000D).
            return c == ' ' || c == '\t' || c == '\n' || c == '\v' || c == '\f' || c == '\r';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsControl(char c)
        {
            return c < ' ' || char.IsControl(c);
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsEscapableSymbol(char c)
        {
            // char.IsSymbol also works with Unicode symbols that cannot be escaped based on the specification.
            return (c > ' ' && c < '0') || (c > '9' && c < 'A') || (c > 'Z' && c < 'a') || (c > 'z' && c < 127) || c == '•';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsWhiteSpaceOrZero(char c)
        {
            return IsWhitespace(c) || IsZero(c);
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsNewLine(char c)
        {
            return c == '\n';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsZero(char c)
        {
            return c == '\0';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsSpace(char c)
        {
            // 2.1 Characters and lines 
            // A space is U+0020.
            return c == ' ';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsTab(char c)
        {
            // 2.1 Characters and lines 
            // A space is U+0009.
            return c == '\t';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsSpaceOrTab(char c)
        {
            return IsSpace(c) || IsTab(c);
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static char EscapeInsecure(char c)
        {
            // 2.3 Insecure characters
            // For security reasons, the Unicode character U+0000 must be replaced with the REPLACEMENT CHARACTER (U+FFFD).
            return c == '\0' ? '\ufffd' : c;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsBulletListMarker(char c)
        {
            return c == '-' || c == '+' || c == '*';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsAlphaUpper(char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsAlphaNumeric(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9');
        }

        public static bool IsASCIIPunctuation(char c)
        {
            // 2.1 Characters and lines 
            // An ASCII punctuation character is !, ", #, $, %, &, ', (, ), *, +, ,, -, ., /, :, ;, <, =, >, ?, @, [, \, ], ^, _, `, {, |, }, or ~.
            switch (c)
            {
                case '!':
                case '"':
                case '#':
                case '$':
                case '%':
                case '&':
                case '\'':
                case '(':
                case ')':
                case '*':
                case '+':
                case ',':
                case '-':
                case '.':
                case '/':
                case ':':
                case ';':
                case '<':
                case '=':
                case '>':
                case '?':
                case '@':
                case '[':
                case '\\':
                case ']':
                case '^':
                case '_':
                case '`':
                case '{':
                case '|':
                case '}':
                case '~':
                    return true;
            }
            return false;
        }
    }
}