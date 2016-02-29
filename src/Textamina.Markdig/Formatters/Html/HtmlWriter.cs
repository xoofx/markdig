using System;
using System.IO;
using System.Text;
using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Formatters.Html
{
    public class HtmlWriter
    {
        private const string HexCharacters = "0123456789ABCDEF";
        private static readonly char[] EscapeHtmlCharacters = { '&', '<', '>', '"' };
        private static readonly string EscapeHtmlLessThan = "&lt;";
        private static readonly string EscapeHtmlGreaterThan = "&gt;";
        private static readonly string EscapeHtmlAmpersand = "&amp;";
        private static readonly string EscapeHtmlQuote = "&quot;";

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

        private readonly TextWriter textWriter;

        public HtmlWriter(TextWriter textWriter)
        {
            this.textWriter = textWriter;
        }

        public bool WriteOnlyContent { get; set; }

        public bool ImplicitParagraph { get; set; }

        public HtmlWriter EnsureLine()
        {
            return this;
        }

        public HtmlWriter Write(string content)
        {
            textWriter.Write(content);
            return this;
        }

        public HtmlWriter Write(char content)
        {
            textWriter.Write(content);
            return this;
        }

        public HtmlWriter Write(string content, int offset, int length)
        {
            throw new NotImplementedException();
            //textWriter.Write(content, offset, length);
            return this;
        }

        public HtmlWriter WriteLine()
        {
            textWriter.WriteLine();
            return this;
        }

        public HtmlWriter WriteLine(string content)
        {
            textWriter.WriteLine(content);
            return this;
        }


        public HtmlWriter WriteEscape(string content)
        {
            if (string.IsNullOrEmpty(content))
                return this;

            int pos;
            int lastPos = 0;
            while ((pos = content.IndexOfAny(EscapeHtmlCharacters, lastPos, content.Length - lastPos)) !=
                -1)
            {
                Write(content, lastPos, pos - lastPos);
                lastPos = pos + 1;

                switch (content[pos])
                {
                    case '<':
                        Write("&lt;");
                        break;
                    case '>':
                        Write("&gt;");
                        break;
                    case '&':
                        Write("&amp;");
                        break;
                    case '"':
                        Write("&quot;");
                        break;
                }
            }

            Write(content, lastPos, content.Length - lastPos);
            return this;
        }

        public HtmlWriter WriteEscapeUrl(string content)
        {
            if (content == null)
                return this;

            char c;
            int lastPos = 0;
            int len = content.Length;

            // since both \r and \n are not url-safe characters and will be encoded, all calls are
            // made to WriteConstant.
            for (var pos = 0; pos < len; pos++)
            {
                c = content[pos];

                if (c == '&')
                {
                    Write(content, lastPos, pos - lastPos);
                    lastPos = pos + 1;
                    Write("&amp;");
                }
                else if (c < 128 && !UrlSafeCharacters[c])
                {
                    Write(content, lastPos, pos - lastPos);
                    lastPos = pos + 1;

                    Write("%").Write(HexCharacters[c / 16]).Write(HexCharacters[c % 16]);
                }
                else if (c > 127)
                {
                    Write(content, lastPos, pos - lastPos);
                    lastPos = pos + 1;

                    byte[] bytes;
                    if (c >= '\ud800' && c <= '\udfff' && len != lastPos)
                    {
                        // this char is the first of UTF-32 character pair
                        bytes = Encoding.UTF8.GetBytes(new[] { c, content[lastPos] });
                        lastPos = ++pos + 1;
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(new[] { c });
                    }

                    for (var i = 0; i < bytes.Length; i++)
                    {
                        Write('%').Write(HexCharacters[bytes[i] / 16]).Write(HexCharacters[bytes[i] % 16]);
                    }
                }
            }

            Write(content, lastPos, len - lastPos);
            return this;
        }
    }
}