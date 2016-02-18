using System;
using System.Text;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Formatters
{
    internal static class HtmlHelper
    {
        private static readonly char[] EscapeHtmlCharacters = {'&', '<', '>', '"'};
        private const string HexCharacters = "0123456789ABCDEF";

        private static readonly char[] EscapeHtmlLessThan = "&lt;".ToCharArray();
        private static readonly char[] EscapeHtmlGreaterThan = "&gt;".ToCharArray();
        private static readonly char[] EscapeHtmlAmpersand = "&amp;".ToCharArray();
        private static readonly char[] EscapeHtmlQuote = "&quot;".ToCharArray();

        private static readonly string[] HeadingOpenerTags = {"<h1>", "<h2>", "<h3>", "<h4>", "<h5>", "<h6>"};
        private static readonly string[] HeadingCloserTags = {"</h1>", "</h2>", "</h3>", "</h4>", "</h5>", "</h6>"};

        private static readonly bool[] UrlSafeCharacters =
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false,
            false, true, false, true, true, true, false, false, true, true, true, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true, true, true, true, false, true, false, true,
            true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, true,
            false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, false
        };

        [ThreadStatic]
        private static readonly StringBuilder TempBuilder = new StringBuilder();

        /// <summary>
        /// Destructively unescape a string: remove backslashes before punctuation or symbol characters.
        /// </summary>
        /// <param name="url">The string data that will be changed by unescaping any punctuation or symbol characters.</param>
        public static string Unescape(string url)
        {
            // remove backslashes before punctuation chars:
            int searchPos = 0;
            int lastPos = 0;
            int match;
            char c;
            char[] search = new[] { '\\', '&' };
            var sb = TempBuilder;
            sb.Clear();

            while ((searchPos = url.IndexOfAny(search, searchPos)) != -1)
            {
                c = url[searchPos];
                if (c == '\\')
                {
                    searchPos++;

                    if (url.Length == searchPos)
                        break;

                    c = url[searchPos];
                    if (Utility.IsEscapableSymbol(c))
                    {
                        sb.Append(url, lastPos, searchPos - lastPos - 1);
                        lastPos = searchPos;
                    }
                }
                else if (c == '&')
                {
                    string namedEntity;
                    int numericEntity;
                    match = Scanner.scan_entity(url, searchPos, url.Length - searchPos, out namedEntity, out numericEntity);
                    if (match == 0)
                    {
                        searchPos++;
                    }
                    else
                    {
                        searchPos += match;

                        if (namedEntity != null)
                        {
                            var decoded = EntityDecoder.DecodeEntity(namedEntity);
                            if (decoded != null)
                            {
                                sb.Append(url, lastPos, searchPos - match - lastPos);
                                sb.Append(decoded);
                                lastPos = searchPos;
                            }
                        }
                        else if (numericEntity > 0)
                        {
                            var decoded = EntityDecoder.DecodeEntity(numericEntity);
                            if (decoded != null)
                            {
                                sb.Append(url, lastPos, searchPos - match - lastPos);
                                sb.Append(decoded);
                            }
                            else
                            {
                                sb.Append(url, lastPos, searchPos - match - lastPos);
                                sb.Append('\uFFFD');
                            }

                            lastPos = searchPos;
                        }
                    }
                }
            }

            if (sb.Length == 0)
                return url;

            sb.Append(url, lastPos, url.Length - lastPos);
            var result = sb.ToString();
            sb.Clear();
            return result;
        }

        /// <summary>
        /// Escapes special URL characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        internal static void EscapeUrl(string input, HtmlTextWriter target)
        {
            if (input == null)
                return;

            char c;
            int lastPos = 0;
            int len = input.Length;
            char[] buffer;

            if (target.Buffer.Length < len)
                buffer = target.Buffer = input.ToCharArray();
            else
            {
                buffer = target.Buffer;
                input.CopyTo(0, buffer, 0, len);
            }

            // since both \r and \n are not url-safe characters and will be encoded, all calls are
            // made to WriteConstant.
            for (var pos = 0; pos < len; pos++)
            {
                c = buffer[pos];

                if (c == '&')
                {
                    target.WriteConstant(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;
                    target.WriteConstant(EscapeHtmlAmpersand);
                }
                else if (c < 128 && !UrlSafeCharacters[c])
                {
                    target.WriteConstant(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;

                    target.WriteConstant(new[] {'%', HexCharacters[c/16], HexCharacters[c%16]});
                }
                else if (c > 127)
                {
                    target.WriteConstant(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;

                    byte[] bytes;
                    if (c >= '\ud800' && c <= '\udfff' && len != lastPos)
                    {
                        // this char is the first of UTF-32 character pair
                        bytes = Encoding.UTF8.GetBytes(new[] {c, buffer[lastPos]});
                        lastPos = ++pos + 1;
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(new[] {c});
                    }

                    for (var i = 0; i < bytes.Length; i++)
                        target.WriteConstant(new[] {'%', HexCharacters[bytes[i]/16], HexCharacters[bytes[i]%16]});
                }
            }

            target.WriteConstant(buffer, lastPos, len - lastPos);
        }

        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        internal static void EscapeHtml(string input, HtmlTextWriter target)
        {
            if (input.Length == 0)
                return;

            int pos;
            int lastPos = 0;
            char[] buffer;

            if (target.Buffer.Length < input.Length)
                buffer = target.Buffer = new char[input.Length];
            else
                buffer = target.Buffer;

            input.CopyTo(0, buffer, 0, input.Length);

            while (
                (pos = input.IndexOfAny(EscapeHtmlCharacters, lastPos, input.Length - lastPos)) !=
                -1)
            {
                target.Write(buffer, lastPos, pos - lastPos);
                lastPos = pos + 1;

                switch (input[pos])
                {
                    case '<':
                        target.WriteConstant(EscapeHtmlLessThan);
                        break;
                    case '>':
                        target.WriteConstant(EscapeHtmlGreaterThan);
                        break;
                    case '&':
                        target.WriteConstant(EscapeHtmlAmpersand);
                        break;
                    case '"':
                        target.WriteConstant(EscapeHtmlQuote);
                        break;
                }
            }

            target.Write(buffer, lastPos, input.Length - lastPos);
        }
        /*

        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        internal static void EscapeHtml(StringContent inp, HtmlTextWriter target)
        {
            int pos;
            int lastPos;
            char[] buffer = target.Buffer;

            var parts = inp.RetrieveParts();
            for (var i = parts.Offset; i < parts.Offset + parts.Count; i++)
            {
                var part = parts.Array[i];

                if (buffer.Length < part.Length)
                    buffer = target.Buffer = new char[part.Length];

                part.Source.CopyTo(part.StartIndex, buffer, 0, part.Length);

                lastPos = part.StartIndex;
                while (
                    (pos =
                        part.Source.IndexOfAny(EscapeHtmlCharacters, lastPos, part.Length - lastPos + part.StartIndex)) !=
                    -1)
                {
                    target.Write(buffer, lastPos - part.StartIndex, pos - lastPos);
                    lastPos = pos + 1;

                    switch (part.Source[pos])
                    {
                        case '<':
                            target.WriteConstant(EscapeHtmlLessThan);
                            break;
                        case '>':
                            target.WriteConstant(EscapeHtmlGreaterThan);
                            break;
                        case '&':
                            target.WriteConstant(EscapeHtmlAmpersand);
                            break;
                        case '"':
                            target.WriteConstant(EscapeHtmlQuote);
                            break;
                    }
                }

                target.Write(buffer, lastPos - part.StartIndex, part.Length - lastPos + part.StartIndex);
            }
        }
        */
    }
}