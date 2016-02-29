using System;
using System.Text;
using Textamina.Markdig.Formatters;
using Textamina.Markdig.Formatters.Html;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Helpers
{
    public static class HtmlHelper
    {
        /// <summary>
        /// List of valid schemes of an URL. The array must be sorted.
        /// </summary>
        private static readonly string[] SchemeArray = new[]
        {
            "AAA", "AAAS", "ABOUT", "ACAP", "ADIUMXTRA", "AFP", "AFS", "AIM", "APT", "ATTACHMENT", "AW", "BESHARE",
            "BITCOIN", "BOLO", "CALLTO", "CAP", "CHROME", "CHROME-EXTENSION", "CID", "COAP", "COM-EVENTBRITE-ATTENDEE",
            "CONTENT", "CRID", "CVS", "DATA", "DAV", "DICT", "DLNA-PLAYCONTAINER", "DLNA-PLAYSINGLE", "DNS", "DOI",
            "DTN", "DVB", "ED2K", "FACETIME", "FEED", "FILE", "FINGER", "FISH", "FTP", "GEO", "GG", "GIT",
            "GIZMOPROJECT", "GO", "GOPHER", "GTALK", "H323", "HCP", "HTTP", "HTTPS", "IAX", "ICAP", "ICON", "IM", "IMAP",
            "INFO", "IPN", "IPP", "IRC", "IRC6", "IRCS", "IRIS", "IRIS.BEEP", "IRIS.LWZ", "IRIS.XPC", "IRIS.XPCS",
            "ITMS", "JAR", "JAVASCRIPT", "JMS", "KEYPARC", "LASTFM", "LDAP", "LDAPS", "MAGNET", "MAILTO", "MAPS",
            "MARKET", "MESSAGE", "MID", "MMS", "MS-HELP", "MSNIM", "MSRP", "MSRPS", "MTQP", "MUMBLE", "MUPDATE", "MVN",
            "NEWS", "NFS", "NI", "NIH", "NNTP", "NOTES", "OID", "OPAQUELOCKTOKEN", "PALM", "PAPARAZZI", "PLATFORM",
            "POP", "PRES", "PROXY", "PSYC", "QUERY", "RES", "RESOURCE", "RMI", "RSYNC", "RTMP", "RTSP", "SECONDLIFE",
            "SERVICE", "SESSION", "SFTP", "SGN", "SHTTP", "SIEVE", "SIP", "SIPS", "SKYPE", "SMB", "SMS", "SNMP",
            "SOAP.BEEP", "SOAP.BEEPS", "SOLDAT", "SPOTIFY", "SSH", "STEAM", "SVN", "TAG", "TEAMSPEAK", "TEL", "TELNET",
            "TFTP", "THINGS", "THISMESSAGE", "TIP", "TN3270", "TV", "UDP", "UNREAL", "URN", "UT2004", "VEMMI",
            "VENTRILO", "VIEW-SOURCE", "WEBCAL", "WS", "WSS", "WTAI", "WYCIWYG", "XCON", "XCON-USERID", "XFIRE",
            "XMLRPC.BEEP", "XMLRPC.BEEPS", "XMPP", "XRI", "YMSGR", "Z39.50R", "Z39.50S"
        };


        public static bool TryParseHtmlTag(StringSlice text, out string htmlTag)
        {
            return TryParseHtmlTag(ref text, out htmlTag);
        }

        public static bool TryParseHtmlTag(ref StringSlice text, out string htmlTag)
        {
            var builder = StringBuilderCache.Local();
            var result = TryParseHtmlTag(ref text, builder);
            htmlTag = builder.ToString();
            builder.Clear();
            return result;
        }

        public static bool TryParseHtmlTag(ref StringSlice text, StringBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var c = text.CurrentChar;
            if (c != '<')
            {
                return false;
            }
            c = text.NextChar();

            builder.Append('<');

            switch (c)
            {
                case '/':
                    return TryParseHtmlCloseTag(ref text, builder);
                case '?':
                    return TryParseHtmlTagProcessingInstruction(ref text, builder);
                case '!':
                    builder.Append(c);
                    c = text.NextChar();
                    if (c == '-')
                    {
                        return TryParseHtmlTagHtmlComment(ref text, builder);
                    }

                    if (c == '[')
                    {
                        return TryParseHtmlTagCData(ref text, builder);
                    }

                    return TryParseHtmlTagDeclaration(ref text, builder);
            }

            return TryParseHtmlTagOpenTag(ref text, builder);
        }

        internal static bool TryParseHtmlTagOpenTag(ref StringSlice text, StringBuilder builder)
        {
            var c = text.CurrentChar;

            // Parse the tagname
            if (!c.IsAlpha())
            {
                return false;
            }
            builder.Append(c);

            while (true)
            {
                c = text.NextChar();
                if (c.IsAlphaNumeric() || c == '-')
                {
                    builder.Append(c);
                }
                else
                {
                    break;
                }
            }

            bool hasAttribute = false;
            while (true)
            {
                var hasWhitespaces = false;
                // Skip any whitespaces
                while (c.IsWhitespace())
                {
                    builder.Append(c);
                    c = text.NextChar();
                    hasWhitespaces = true;
                }

                switch (c)
                {
                    case '\0':
                        return false;
                    case '>':
                        text.NextChar();
                        builder.Append(c);
                        return true;
                    case '/':
                        builder.Append('/');
                        c = text.NextChar();
                        if (c != '>')
                        {
                            return false;
                        }
                        text.NextChar();
                        builder.Append('>');
                        return true;
                    case '=':

                        if (!hasAttribute)
                        {
                            return false;
                        }

                        builder.Append('=');

                        // Skip any spaces after
                        c = text.NextChar();
                        while (c.IsWhitespace())
                        {
                            builder.Append(c);
                            c = text.NextChar();
                        }

                        // Parse a quoted string
                        if (c == '\'' || c == '\"')
                        {
                            builder.Append(c);
                            char openingStringChar = c;
                            while (true)
                            {
                                c = text.NextChar();
                                if (c == '\0')
                                {
                                    return false;
                                }
                                if (c != openingStringChar)
                                {
                                    builder.Append(c);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            builder.Append(c);
                            c = text.NextChar();
                        }
                        else
                        {
                            // Parse until we match a space or a special html character
                            int matchCount = 0;
                            while (true)
                            {
                                if (c == '\0')
                                {
                                    return false;
                                }
                                if (c == ' ' || c == '\n' || c == '"' || c == '\'' || c == '=' || c == '<' || c == '>' || c == '`')
                                {
                                    break;
                                }
                                matchCount++;
                                builder.Append(c);
                                c = text.NextChar();
                            }

                            // We need at least one char after '='
                            if (matchCount == 0)
                            {
                                return false;
                            }
                        }

                        hasAttribute = false;
                        continue;
                    default:
                        if (!hasWhitespaces)
                        {
                            return false;
                        }

                        // Parse the attribute name
                        if (!(c.IsAlpha() || c == '_' || c == ':'))
                        {
                            return false;
                        }
                        builder.Append(c);

                        while (true)
                        {
                            c = text.NextChar();
                            if (c.IsAlphaNumeric() || c == '_' || c == ':' || c == '.' || c == '-')
                            {
                                builder.Append(c);
                            }
                            else
                            {
                                break;
                            }
                        }

                        hasAttribute = true;
                        break;
                }
            }
        }

        private static bool TryParseHtmlTagDeclaration(ref StringSlice text, StringBuilder builder)
        {
            var c = text.CurrentChar;
            bool hasAlpha = false;
            while (c.IsAlphaUpper())
            {
                builder.Append(c);
                c = text.NextChar();
                hasAlpha = true;
            }

            if (!hasAlpha || !c.IsWhitespace())
            {
                return false;
            }

            // Regexp: "\\![A-Z]+\\s+[^>\\x00]*>"
            while (true)
            {
                builder.Append(c);
                c = text.NextChar();
                if (c == '\0')
                {
                    return false;
                }

                if (c == '>')
                {
                    text.NextChar();
                    builder.Append('>');
                    return true;
                }
            }
        }

        private static bool TryParseHtmlTagCData(ref StringSlice text, StringBuilder builder)
        {
            builder.Append('[');
            var c = text.NextChar();
            if (c == 'C' &&
                text.NextChar() == 'D' &&
                text.NextChar() == 'A' &&
                text.NextChar() == 'T' &&
                text.NextChar() == 'A' && 
                (c = text.NextChar()) == '[')
            {
                builder.Append("CDATA[");
                while (true)
                {
                    var pc = c;
                    c = text.NextChar();
                    if (c == '\0')
                    {
                        return false;
                    }

                    if (c == ']' && pc == ']')
                    {
                        builder.Append(']');
                        c = text.NextChar();
                        if (c == '>')
                        {
                            builder.Append('>');
                            text.NextChar();
                            return true;
                        }

                        if (c == '\0')
                        {
                            return false;
                        }
                    }
                    builder.Append(c);
                }
            }
            return false;
        }

        internal static bool TryParseHtmlCloseTag(ref StringSlice text, StringBuilder builder)
        {
            // </[A-Za-z][A-Za-z0-9]+\s*>
            builder.Append('/');

            var c = text.NextChar();
            if (!c.IsAlpha())
            {
                return false;
            }
            builder.Append(c);

            bool skipSpaces = false;
            while (true)
            {
                c = text.NextChar();
                if (c == '>')
                {
                    text.NextChar();
                    builder.Append('>');
                    return true;
                }

                if (skipSpaces)
                {
                    if (c != ' ')
                    {
                        break;
                    }
                }
                else if (c == ' ')
                {
                    skipSpaces = true;
                }
                else if (!(c.IsAlphaNumeric() || c == '-'))
                {
                    break;
                }

                builder.Append(c);
            }
            return false;
        }


        private static bool TryParseHtmlTagHtmlComment(ref StringSlice text, StringBuilder builder)
        {
            var c = text.NextChar();
            if (c != '-')
            {
                return false;
            }
            builder.Append('-');
            builder.Append('-');
            if (text.PeekChar(1) == '>')
            {
                return false;
            }

            var countHyphen = 0;
            while (true)
            {
                c = text.NextChar();
                if (c == '\0')
                {
                    return false;
                }

                if (countHyphen == 2)
                {
                    if (c == '>')
                    {
                        builder.Append('>');
                        text.NextChar();
                        return true;
                    }
                    return false;
                }
                countHyphen = c == '-' ? countHyphen + 1 : 0;
                builder.Append(c);
            }
        }

        private static bool TryParseHtmlTagProcessingInstruction(ref StringSlice text, StringBuilder builder)
        {
            builder.Append('?');
            var prevChar = '\0';
            while (true)
            {
                var c = text.NextChar();
                if (c == '\0')
                {
                    return false;
                }

                if (c == '>' && prevChar == '?')
                {
                    builder.Append('>');
                    text.NextChar();
                    return true;
                }
                prevChar = c;
                builder.Append(c);
            }
        }

        private static readonly char[] SearchBackAndAmp = {'\\', '&'};
        private static readonly char[] SearchAmp = {'&'};

        /// <summary>
        /// Destructively unescape a string: remove backslashes before punctuation or symbol characters.
        /// </summary>
        /// <param name="text">The string data that will be changed by unescaping any punctuation or symbol characters.</param>
        public static string Unescape(string text, bool removeBackSlash = true)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // remove backslashes before punctuation chars:
            int searchPos = 0;
            int lastPos = 0;
            char c;
            char[] search = removeBackSlash ? SearchBackAndAmp : SearchAmp;
            StringBuilder sb = null;

            while ((searchPos = text.IndexOfAny(search, searchPos)) != -1)
            {
                if (sb == null)
                {
                    sb = StringBuilderCache.Local();
                    sb.Clear();
                }
                c = text[searchPos];
                if (removeBackSlash && c == '\\')
                {
                    searchPos++;

                    if (text.Length == searchPos)
                        break;

                    c = text[searchPos];
                    if (c.IsEscapableSymbol())
                    {
                        sb.Append(text, lastPos, searchPos - lastPos - 1);
                        lastPos = searchPos;
                    }
                }
                else if (c == '&')
                {
                    string namedEntity;
                    int numericEntity;
                    var match = ScanEntity(text, searchPos, text.Length - searchPos, out namedEntity,
                        out numericEntity);
                    if (match == 0)
                    {
                        searchPos++;
                    }
                    else
                    {
                        searchPos += match;

                        if (namedEntity != null)
                        {
                            var decoded = EntityHelper.DecodeEntity(namedEntity);
                            if (decoded != null)
                            {
                                sb.Append(text, lastPos, searchPos - match - lastPos);
                                sb.Append(decoded);
                                lastPos = searchPos;
                            }
                        }
                        else if (numericEntity >= 0)
                        {
                            sb.Append(text, lastPos, searchPos - match - lastPos);
                            if (numericEntity == 0)
                            {
                                sb.Append('\0'.EscapeInsecure());
                            }
                            else
                            {
                                var decoded = EntityHelper.DecodeEntity(numericEntity);
                                if (decoded != null)
                                {
                                    sb.Append(decoded);
                                }
                                else
                                {
                                    sb.Append('\uFFFD');
                                }
                            }

                            lastPos = searchPos;
                        }
                    }
                }
            }

            if (sb == null)
                return text;

            sb.Append(text, lastPos, text.Length - lastPos);
            var result = sb.ToString();
            sb.Clear();
            return result;
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

        /// <summary>
        /// Scans an entity.
        /// Returns number of chars matched.
        /// </summary>
        public static int ScanEntity(string s, int pos, int length, out string namedEntity, out int numericEntity)
        {
            /*!re2c
                  [&] ([#] ([Xx][A-Fa-f0-9]{1,8}|[0-9]{1,8}) |[A-Za-z][A-Za-z0-9]{1,31} ) [;]
                     { return (p - start); }
                  .? { return 0; }
                */

            var lastPos = pos + length;

            namedEntity = null;
            numericEntity = 0;

            if (pos + 3 >= lastPos)
                return 0;

            if (s[pos] != '&')
                return 0;

            char c;
            int i;
            int counter = 0;
            if (s[pos + 1] == '#')
            {
                c = s[pos + 2];
                if (c == 'x' || c == 'X')
                {
                    // expect 1-8 hex digits starting from pos+3
                    for (i = pos + 3; i < lastPos; i++)
                    {
                        c = s[i];
                        if (c >= '0' && c <= '9')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity*16 + (c - '0');
                            continue;
                        }
                        else if (c >= 'A' && c <= 'F')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity*16 + (c - 'A' + 10);
                            continue;
                        }
                        else if (c >= 'a' && c <= 'f')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity*16 + (c - 'a' + 10);
                            continue;
                        }

                        if (c == ';')
                            return counter == 0 ? 0 : i - pos + 1;

                        return 0;
                    }
                }
                else
                {
                    // expect 1-8 digits starting from pos+2
                    for (i = pos + 2; i < lastPos; i++)
                    {
                        c = s[i];
                        if (c >= '0' && c <= '9')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity*10 + (c - '0');
                            continue;
                        }

                        if (c == ';')
                            return counter == 0 ? 0 : i - pos + 1;

                        return 0;
                    }
                }
            }
            else
            {
                // expect a letter and 1-31 letters or digits
                c = s[pos + 1];
                if ((c < 'A' || c > 'Z') && (c < 'a' && c > 'z'))
                    return 0;

                for (i = pos + 2; i < lastPos; i++)
                {
                    c = s[i];
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    {
                        if (++counter == 32)
                            return 0;

                        continue;
                    }

                    if (c == ';')
                    {
                        namedEntity = s.Substring(pos + 1, counter + 1);
                        return counter == 0 ? 0 : i - pos + 1;
                    }

                    return 0;
                }
            }

            return 0;
        }

        public static bool IsUrlScheme(string scheme)
        {
            return Array.BinarySearch(SchemeArray, scheme, StringComparer.Ordinal) >= 0;
        }
    }
}