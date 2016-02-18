using System;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Formatters
{

    /// <summary>
    /// Contains the regular expressions that are used in the parsers.
    /// </summary>
    internal static partial class Scanner
    {
        /// <summary>
        /// List of valid schemes of an URL. The array must be sorted.
        /// </summary>
        private static readonly string[] schemeArray = new[]
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

        /// <summary>
        /// Try to match URI autolink after first &lt;, returning number of chars matched.
        /// </summary>
        public static int scan_autolink_uri(string s, int pos, int sourceLength)
        {
            /*!re2c
                  scheme [:]([^\x00-\x20<>\\]|escaped_char)*[>]  { return (p - start); }
                  .? { return 0; }
                */
            // for now the tests do not include anything that would require the use of `escaped_char` part so it is ignored.

            // 24 is the maximum length of a valid scheme
            var checkLen = sourceLength - pos;
            if (checkLen > 24)
                checkLen = 24;

            // PERF: potential small improvement - instead of using IndexOf, check char-by-char and return as soon as an invalid character is found ([^a-z0-9\.])
            // alternative approach (if we want to go crazy about performance - store the valid schemes as a prefix tree and lookup the valid scheme char by char and
            // return as soon as the part does not match any prefix.
            var colonpos = s.IndexOf(':', pos, checkLen);
            if (colonpos == -1)
                return 0;

            var potentialScheme = s.Substring(pos, colonpos - pos).ToUpperInvariant();
            if (Array.BinarySearch(schemeArray, potentialScheme, StringComparer.Ordinal) < 0)
                return 0;

            for (var i = colonpos + 1; i < sourceLength; i++)
            {
                var c = s[i];
                if (c == '>')
                    return i - pos + 1;

                if (c == '<' || c <= 0x20)
                    return 0;
            }

            return 0;
        }

        /// <summary>
        /// Try to match a link title (in single quotes, in double quotes, or
        /// in parentheses), returning number of chars matched.  Allow one
        /// level of internal nesting (quotes within quotes).
        /// </summary>
        public static int scan_link_title(string s, int pos, int sourceLength)
        {
            /*!re2c
                  ["] (escaped_char|[^"\x00])* ["]   { return (p - start); }
                  ['] (escaped_char|[^'\x00])* ['] { return (p - start); }
                  [(] (escaped_char|[^)\x00])* [)]  { return (p - start); }
                  .? { return 0; }
                */

            if (pos + 2 >= sourceLength)
                return 0;

            var c1 = s[pos];
            if (c1 != '"' && c1 != '\'' && c1 != '(')
                return 0;

            if (c1 == '(') c1 = ')';

            var nextEscaped = false;
            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];
                if (c == c1 && !nextEscaped)
                    return i - pos + 1;

                nextEscaped = !nextEscaped && c == '\\';
            }

            return 0;
        }

        /// <summary>
        /// Match space characters, including newlines.
        /// </summary>
        public static int scan_spacechars(string s, int pos, int sourceLength)
        {
            /*!re2c
                  [ \t\n]* { return (p - start); }
                  . { return 0; }
                */
            if (pos >= sourceLength)
                return 0;

            for (var i = pos; i < sourceLength; i++)
            {
                if (!Utility.IsWhitespace(s[i]))
                    return i - pos;
            }

            return sourceLength - pos;
        }

        /// <summary>
        /// Match ATX heading start.
        /// </summary>
        public static int scan_atx_heading_start(string s, int pos, int sourceLength, out int headingLevel)
        {
            /*!re2c
                  [#]{1,6} ([ ]+|[\n])  { return (p - start); }
                  .? { return 0; }
                */

            headingLevel = 1;
            if (pos + 1 >= sourceLength)
                return 0;

            if (s[pos] != '#')
                return 0;

            var spaceExists = false;
            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];

                if (c == '#')
                {
                    if (headingLevel == 6)
                        return 0;

                    if (spaceExists)
                        return i - pos;
                    else
                        headingLevel++;
                }
                else if (c == ' ')
                {
                    spaceExists = true;
                }
                else if (c == '\n')
                {
                    return i - pos + 1;
                }
                else
                {
                    return spaceExists ? i - pos : 0;
                }
            }

            if (spaceExists)
                return sourceLength - pos;

            return 0;
        }

        /// <summary>
        /// Match sexext heading line.  Return 1 for level-1 heading,
        /// 2 for level-2, 0 for no match.
        /// </summary>
        public static int scan_setext_heading_line(string s, int pos, int sourceLength)
        {
            /*!re2c
                  [=]+ [ ]* [\n] { return 1; }
                  [-]+ [ ]* [\n] { return 2; }
                  .? { return 0; }
                */

            if (pos >= sourceLength)
                return 0;

            var c1 = s[pos];

            if (c1 != '=' && c1 != '-')
                return 0;

            var fin = false;
            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];
                if (c == c1 && !fin)
                    continue;

                fin = true;
                if (c == ' ')
                    continue;

                if (c == '\n')
                    break;

                return 0;
            }

            return c1 == '=' ? 1 : 2;
        }

        /// <summary>
        /// Scan a thematic break line: "...three or more hyphens, asterisks,
        /// or underscores on a line by themselves. If you wish, you may use
        /// spaces between the hyphens or asterisks."
        /// </summary>
        public static int scan_thematic_break(string s, int pos, int sourceLength)
        {
            // @"^([\*][ ]*){3,}[\s]*$",
            // @"^([_][ ]*){3,}[\s]*$",
            // @"^([-][ ]*){3,}[\s]*$",

            var count = 0;
            var x = '\0';
            var ipos = pos;
            while (ipos < sourceLength)
            {
                var c = s[ipos++];

                if (c == ' ' || c == '\n')
                    continue;
                if (count == 0)
                {
                    if (c == '*' || c == '_' || c == '-')
                        x = c;
                    else
                        return 0;

                    count = 1;
                }
                else if (c == x)
                    count++;
                else
                    return 0;
            }

            if (count < 3)
                return 0;

            return sourceLength - pos;
        }

        /// <summary>
        /// Scan an opening code fence. Returns the number of characters forming the fence.
        /// </summary>
        public static int scan_open_code_fence(string s, int pos, int sourceLength)
        {
            /*!re2c
                  [`]{3,} / [^`\n\x00]*[\n] { return (p - start); }
                  [~]{3,} / [^~\n\x00]*[\n] { return (p - start); }
                  .?                        { return 0; }
                */

            if (pos + 3 >= sourceLength)
                return 0;

            var fchar = s[pos];
            if (fchar != '`' && fchar != '~')
                return 0;

            var cnt = 1;
            var fenceDone = false;
            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];

                if (c == fchar)
                {
                    if (fenceDone)
                        return 0;

                    cnt++;
                    continue;
                }

                fenceDone = true;
                if (cnt < 3)
                    return 0;

                if (c == '\n')
                    return cnt;
            }

            if (cnt < 3)
                return 0;

            return cnt;
        }

        /// <summary>
        /// Scan a closing code fence with length at least len.
        /// </summary>
        public static int scan_close_code_fence(string s, int pos, int len, int sourceLength)
        {
            /*!re2c
                  ([`]{3,} | [~]{3,}) / spacechar* [\n]
                                              { if (p - start > len) {
                                                return (p - start);
                                              } else {
                                                return 0;
                                              } }
                  .? { return 0; }
                */
            if (pos + len >= sourceLength)
                return 0;

            var c1 = s[pos];
            if (c1 != '`' && c1 != '~')
                return 0;

            var cnt = 1;
            var spaces = false;
            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];
                if (c == c1 && !spaces)
                    cnt++;
                else if (c == ' ')
                    spaces = true;
                else if (c == '\n')
                    return cnt < len ? 0 : cnt;
                else
                    return 0;
            }

            return 0;
        }

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
    }
}