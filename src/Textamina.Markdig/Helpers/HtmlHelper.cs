using System;
using System.Text;
using Textamina.Markdig.Formatters;

namespace Textamina.Markdig.Helpers
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

        [ThreadStatic] private static readonly StringBuilder TempBuilder = new StringBuilder();

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
            char[] search = new[] {'\\', '&'};
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
                    if (CharHelper.IsEscapableSymbol(c))
                    {
                        sb.Append(url, lastPos, searchPos - lastPos - 1);
                        lastPos = searchPos;
                    }
                }
                else if (c == '&')
                {
                    string namedEntity;
                    int numericEntity;
                    match = scan_entity(url, searchPos, url.Length - searchPos, out namedEntity,
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
                                sb.Append(url, lastPos, searchPos - match - lastPos);
                                sb.Append(decoded);
                                lastPos = searchPos;
                            }
                        }
                        else if (numericEntity > 0)
                        {
                            var decoded = EntityHelper.DecodeEntity(numericEntity);
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
        public static void EscapeUrl(string input, HtmlTextWriter target)
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
        public static void EscapeHtml(string input, HtmlTextWriter target)
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

        /// <summary>
        /// Scans an entity.
        /// Returns number of chars matched.
        /// </summary>
        public static int scan_entity(string s, int pos, int length, out string namedEntity, out int numericEntity)
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