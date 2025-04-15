
// --------------------------------
//            Auto Links
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.AutoLinks
{
    [TestFixture]
    public class TestExtensionsAutoLinks
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## AutoLinks
        // 
        // Autolinks will format as a HTML link any string that starts by:
        // 
        // - `http://` or `https://` 
        // - `ftp://`
        // - `mailto:`
        // - `tel:`
        // - `www.` 
        [Test]
        public void ExtensionsAutoLinks_Example001()
        {
            // Example 1
            // Section: Extensions / AutoLinks
            //
            // The following Markdown:
            //     This is a http://www.google.com URL and https://www.google.com
            //     This is a ftp://test.com
            //     And a mailto:email@toto.com
            //     And a tel:+1555123456
            //     And a plain www.google.com
            //
            // Should be rendered as:
            //     <p>This is a <a href="http://www.google.com">http://www.google.com</a> URL and <a href="https://www.google.com">https://www.google.com</a>
            //     This is a <a href="ftp://test.com">ftp://test.com</a>
            //     And a <a href="mailto:email@toto.com">email@toto.com</a>
            //     And a <a href="tel:+1555123456">+1555123456</a>
            //     And a plain <a href="http://www.google.com">www.google.com</a></p>

            TestParser.TestSpec("This is a http://www.google.com URL and https://www.google.com\nThis is a ftp://test.com\nAnd a mailto:email@toto.com\nAnd a tel:+1555123456\nAnd a plain www.google.com", "<p>This is a <a href=\"http://www.google.com\">http://www.google.com</a> URL and <a href=\"https://www.google.com\">https://www.google.com</a>\nThis is a <a href=\"ftp://test.com\">ftp://test.com</a>\nAnd a <a href=\"mailto:email@toto.com\">email@toto.com</a>\nAnd a <a href=\"tel:+1555123456\">+1555123456</a>\nAnd a plain <a href=\"http://www.google.com\">www.google.com</a></p>", "autolinks|advanced", context: "Example 1\nSection Extensions / AutoLinks\n");
        }

        // But incomplete links will not be matched:
        [Test]
        public void ExtensionsAutoLinks_Example002()
        {
            // Example 2
            // Section: Extensions / AutoLinks
            //
            // The following Markdown:
            //     This is not a http:/www.google.com URL and https:/www.google.com
            //     This is not a ftp:/test.com
            //     And not a mailto:emailtoto.com
            //     And not a plain www. or a www.x 
            //     And not a tel:
            //
            // Should be rendered as:
            //     <p>This is not a http:/www.google.com URL and https:/www.google.com
            //     This is not a ftp:/test.com
            //     And not a mailto:emailtoto.com
            //     And not a plain www. or a www.x
            //     And not a tel:</p>

            TestParser.TestSpec("This is not a http:/www.google.com URL and https:/www.google.com\nThis is not a ftp:/test.com\nAnd not a mailto:emailtoto.com\nAnd not a plain www. or a www.x \nAnd not a tel:", "<p>This is not a http:/www.google.com URL and https:/www.google.com\nThis is not a ftp:/test.com\nAnd not a mailto:emailtoto.com\nAnd not a plain www. or a www.x\nAnd not a tel:</p>", "autolinks|advanced", context: "Example 2\nSection Extensions / AutoLinks\n");
        }

        // Previous character must be a punctuation or a valid space (tab, space, new line):
        [Test]
        public void ExtensionsAutoLinks_Example003()
        {
            // Example 3
            // Section: Extensions / AutoLinks
            //
            // The following Markdown:
            //     This is not a nhttp://www.google.com URL but this is (https://www.google.com)
            //
            // Should be rendered as:
            //     <p>This is not a nhttp://www.google.com URL but this is (<a href="https://www.google.com">https://www.google.com</a>)</p>

            TestParser.TestSpec("This is not a nhttp://www.google.com URL but this is (https://www.google.com)", "<p>This is not a nhttp://www.google.com URL but this is (<a href=\"https://www.google.com\">https://www.google.com</a>)</p>", "autolinks|advanced", context: "Example 3\nSection Extensions / AutoLinks\n");
        }

        // An autolink should not interfere with an `<a>` HTML inline:
        [Test]
        public void ExtensionsAutoLinks_Example004()
        {
            // Example 4
            // Section: Extensions / AutoLinks
            //
            // The following Markdown:
            //     This is an HTML <a href="http://www.google.com">http://www.google.com</a> link
            //
            // Should be rendered as:
            //     <p>This is an HTML <a href="http://www.google.com">http://www.google.com</a> link</p>

            TestParser.TestSpec("This is an HTML <a href=\"http://www.google.com\">http://www.google.com</a> link", "<p>This is an HTML <a href=\"http://www.google.com\">http://www.google.com</a> link</p>", "autolinks|advanced", context: "Example 4\nSection Extensions / AutoLinks\n");
        }

        // or even within emphasis:
        [Test]
        public void ExtensionsAutoLinks_Example005()
        {
            // Example 5
            // Section: Extensions / AutoLinks
            //
            // The following Markdown:
            //     This is an HTML <a href="http://www.google.com"> **http://www.google.com** </a> link
            //
            // Should be rendered as:
            //     <p>This is an HTML <a href="http://www.google.com"> <strong>http://www.google.com</strong> </a> link</p>

            TestParser.TestSpec("This is an HTML <a href=\"http://www.google.com\"> **http://www.google.com** </a> link", "<p>This is an HTML <a href=\"http://www.google.com\"> <strong>http://www.google.com</strong> </a> link</p>", "autolinks|advanced", context: "Example 5\nSection Extensions / AutoLinks\n");
        }

        // An autolink should not interfere with a markdown link:
        [Test]
        public void ExtensionsAutoLinks_Example006()
        {
            // Example 6
            // Section: Extensions / AutoLinks
            //
            // The following Markdown:
            //     This is an HTML [http://www.google.com](http://www.google.com) link
            //
            // Should be rendered as:
            //     <p>This is an HTML <a href="http://www.google.com">http://www.google.com</a> link</p>

            TestParser.TestSpec("This is an HTML [http://www.google.com](http://www.google.com) link", "<p>This is an HTML <a href=\"http://www.google.com\">http://www.google.com</a> link</p>", "autolinks|advanced", context: "Example 6\nSection Extensions / AutoLinks\n");
        }

        // A link embraced by pending emphasis should let the emphasis takes precedence if characters are placed at the end of the matched link:
        [Test]
        public void ExtensionsAutoLinks_Example007()
        {
            // Example 7
            // Section: Extensions / AutoLinks
            //
            // The following Markdown:
            //     Check **http://www.a.com** or __http://www.b.com__
            //
            // Should be rendered as:
            //     <p>Check <strong><a href="http://www.a.com">http://www.a.com</a></strong> or <strong><a href="http://www.b.com">http://www.b.com</a></strong></p>

            TestParser.TestSpec("Check **http://www.a.com** or __http://www.b.com__", "<p>Check <strong><a href=\"http://www.a.com\">http://www.a.com</a></strong> or <strong><a href=\"http://www.b.com\">http://www.b.com</a></strong></p>", "autolinks|advanced", context: "Example 7\nSection Extensions / AutoLinks\n");
        }

        // It is not mentioned by the spec, but empty emails won't be matched (only a subset of [RFC2368](https://tools.ietf.org/html/rfc2368) is supported by auto links):
        [Test]
        public void ExtensionsAutoLinks_Example008()
        {
            // Example 8
            // Section: Extensions / AutoLinks
            //
            // The following Markdown:
            //     mailto:email@test.com is okay, but mailto:@test.com is not
            //
            // Should be rendered as:
            //     <p><a href="mailto:email@test.com">email@test.com</a> is okay, but mailto:@test.com is not</p>

            TestParser.TestSpec("mailto:email@test.com is okay, but mailto:@test.com is not", "<p><a href=\"mailto:email@test.com\">email@test.com</a> is okay, but mailto:@test.com is not</p>", "autolinks|advanced", context: "Example 8\nSection Extensions / AutoLinks\n");
        }
    }

    [TestFixture]
    public class TestExtensionsAutoLinksGFMSupport
    {
        // ### GFM Support
        // 
        // Extract from [GFM Autolinks extensions specs](https://github.github.com/gfm/#autolinks-extension-)
        [Test]
        public void ExtensionsAutoLinksGFMSupport_Example009()
        {
            // Example 9
            // Section: Extensions / AutoLinks / GFM Support
            //
            // The following Markdown:
            //     www.commonmark.org
            //
            // Should be rendered as:
            //     <p><a href="http://www.commonmark.org">www.commonmark.org</a></p>

            TestParser.TestSpec("www.commonmark.org", "<p><a href=\"http://www.commonmark.org\">www.commonmark.org</a></p>", "autolinks|advanced", context: "Example 9\nSection Extensions / AutoLinks / GFM Support\n");
        }

        [Test]
        public void ExtensionsAutoLinksGFMSupport_Example010()
        {
            // Example 10
            // Section: Extensions / AutoLinks / GFM Support
            //
            // The following Markdown:
            //     Visit www.commonmark.org/help for more information.
            //
            // Should be rendered as:
            //     <p>Visit <a href="http://www.commonmark.org/help">www.commonmark.org/help</a> for more information.</p>

            TestParser.TestSpec("Visit www.commonmark.org/help for more information.", "<p>Visit <a href=\"http://www.commonmark.org/help\">www.commonmark.org/help</a> for more information.</p>", "autolinks|advanced", context: "Example 10\nSection Extensions / AutoLinks / GFM Support\n");
        }

        [Test]
        public void ExtensionsAutoLinksGFMSupport_Example011()
        {
            // Example 11
            // Section: Extensions / AutoLinks / GFM Support
            //
            // The following Markdown:
            //     Visit www.commonmark.org.
            //     
            //     Visit www.commonmark.org/a.b.
            //
            // Should be rendered as:
            //     <p>Visit <a href="http://www.commonmark.org">www.commonmark.org</a>.</p>
            //     <p>Visit <a href="http://www.commonmark.org/a.b">www.commonmark.org/a.b</a>.</p>

            TestParser.TestSpec("Visit www.commonmark.org.\n\nVisit www.commonmark.org/a.b.", "<p>Visit <a href=\"http://www.commonmark.org\">www.commonmark.org</a>.</p>\n<p>Visit <a href=\"http://www.commonmark.org/a.b\">www.commonmark.org/a.b</a>.</p>", "autolinks|advanced", context: "Example 11\nSection Extensions / AutoLinks / GFM Support\n");
        }

        [Test]
        public void ExtensionsAutoLinksGFMSupport_Example012()
        {
            // Example 12
            // Section: Extensions / AutoLinks / GFM Support
            //
            // The following Markdown:
            //     www.google.com/search?q=Markup+(business)
            //     
            //     (www.google.com/search?q=Markup+(business))
            //
            // Should be rendered as:
            //     <p><a href="http://www.google.com/search?q=Markup+(business)">www.google.com/search?q=Markup+(business)</a></p>
            //     <p>(<a href="http://www.google.com/search?q=Markup+(business)">www.google.com/search?q=Markup+(business)</a>)</p>

            TestParser.TestSpec("www.google.com/search?q=Markup+(business)\n\n(www.google.com/search?q=Markup+(business))", "<p><a href=\"http://www.google.com/search?q=Markup+(business)\">www.google.com/search?q=Markup+(business)</a></p>\n<p>(<a href=\"http://www.google.com/search?q=Markup+(business)\">www.google.com/search?q=Markup+(business)</a>)</p>", "autolinks|advanced", context: "Example 12\nSection Extensions / AutoLinks / GFM Support\n");
        }

        [Test]
        public void ExtensionsAutoLinksGFMSupport_Example013()
        {
            // Example 13
            // Section: Extensions / AutoLinks / GFM Support
            //
            // The following Markdown:
            //     www.google.com/search?q=commonmark&hl=en
            //     
            //     www.google.com/search?q=commonmark&hl;
            //
            // Should be rendered as:
            //     <p><a href="http://www.google.com/search?q=commonmark&amp;hl=en">www.google.com/search?q=commonmark&amp;hl=en</a></p>
            //     <p><a href="http://www.google.com/search?q=commonmark">www.google.com/search?q=commonmark</a>&amp;hl;</p>

            TestParser.TestSpec("www.google.com/search?q=commonmark&hl=en\n\nwww.google.com/search?q=commonmark&hl;", "<p><a href=\"http://www.google.com/search?q=commonmark&amp;hl=en\">www.google.com/search?q=commonmark&amp;hl=en</a></p>\n<p><a href=\"http://www.google.com/search?q=commonmark\">www.google.com/search?q=commonmark</a>&amp;hl;</p>", "autolinks|advanced", context: "Example 13\nSection Extensions / AutoLinks / GFM Support\n");
        }

        [Test]
        public void ExtensionsAutoLinksGFMSupport_Example014()
        {
            // Example 14
            // Section: Extensions / AutoLinks / GFM Support
            //
            // The following Markdown:
            //     www.commonmark.org/he<lp
            //
            // Should be rendered as:
            //     <p><a href="http://www.commonmark.org/he">www.commonmark.org/he</a>&lt;lp</p>

            TestParser.TestSpec("www.commonmark.org/he<lp", "<p><a href=\"http://www.commonmark.org/he\">www.commonmark.org/he</a>&lt;lp</p>", "autolinks|advanced", context: "Example 14\nSection Extensions / AutoLinks / GFM Support\n");
        }

        [Test]
        public void ExtensionsAutoLinksGFMSupport_Example015()
        {
            // Example 15
            // Section: Extensions / AutoLinks / GFM Support
            //
            // The following Markdown:
            //     http://commonmark.org
            //     
            //     (Visit https://encrypted.google.com/search?q=Markup+(business))
            //     
            //     Anonymous FTP is available at ftp://foo.bar.baz.
            //
            // Should be rendered as:
            //     <p><a href="http://commonmark.org">http://commonmark.org</a></p>
            //     <p>(Visit <a href="https://encrypted.google.com/search?q=Markup+(business)">https://encrypted.google.com/search?q=Markup+(business)</a>)</p>
            //     <p>Anonymous FTP is available at <a href="ftp://foo.bar.baz">ftp://foo.bar.baz</a>.</p>

            TestParser.TestSpec("http://commonmark.org\n\n(Visit https://encrypted.google.com/search?q=Markup+(business))\n\nAnonymous FTP is available at ftp://foo.bar.baz.", "<p><a href=\"http://commonmark.org\">http://commonmark.org</a></p>\n<p>(Visit <a href=\"https://encrypted.google.com/search?q=Markup+(business)\">https://encrypted.google.com/search?q=Markup+(business)</a>)</p>\n<p>Anonymous FTP is available at <a href=\"ftp://foo.bar.baz\">ftp://foo.bar.baz</a>.</p>", "autolinks|advanced", context: "Example 15\nSection Extensions / AutoLinks / GFM Support\n");
        }
    }

    [TestFixture]
    public class TestExtensionsAutoLinksValidDomainTests
    {
        // ### Valid Domain Tests
        // 
        // Domain names that have empty segments won't be matched
        [Test]
        public void ExtensionsAutoLinksValidDomainTests_Example016()
        {
            // Example 16
            // Section: Extensions / AutoLinks / Valid Domain Tests
            //
            // The following Markdown:
            //     www..
            //     www..com
            //     http://test.
            //     http://.test
            //     http://.
            //     http://..
            //     ftp://test.
            //     ftp://.test
            //     mailto:email@test.
            //     mailto:email@.test
            //
            // Should be rendered as:
            //     <p>www..
            //     www..com
            //     http://test.
            //     http://.test
            //     http://.
            //     http://..
            //     ftp://test.
            //     ftp://.test
            //     mailto:email@test.
            //     mailto:email@.test</p>

            TestParser.TestSpec("www..\nwww..com\nhttp://test.\nhttp://.test\nhttp://.\nhttp://..\nftp://test.\nftp://.test\nmailto:email@test.\nmailto:email@.test", "<p>www..\nwww..com\nhttp://test.\nhttp://.test\nhttp://.\nhttp://..\nftp://test.\nftp://.test\nmailto:email@test.\nmailto:email@.test</p>", "autolinks|advanced", context: "Example 16\nSection Extensions / AutoLinks / Valid Domain Tests\n");
        }

        // Domain names with too few segments won't be matched
        [Test]
        public void ExtensionsAutoLinksValidDomainTests_Example017()
        {
            // Example 17
            // Section: Extensions / AutoLinks / Valid Domain Tests
            //
            // The following Markdown:
            //     www
            //     www.com
            //     http://test
            //     ftp://test
            //     mailto:email@test
            //
            // Should be rendered as:
            //     <p>www
            //     www.com
            //     http://test
            //     ftp://test
            //     mailto:email@test</p>

            TestParser.TestSpec("www\nwww.com\nhttp://test\nftp://test\nmailto:email@test", "<p>www\nwww.com\nhttp://test\nftp://test\nmailto:email@test</p>", "autolinks|advanced", context: "Example 17\nSection Extensions / AutoLinks / Valid Domain Tests\n");
        }

        // Domain names that contain an underscores in the last two segments won't be matched
        [Test]
        public void ExtensionsAutoLinksValidDomainTests_Example018()
        {
            // Example 18
            // Section: Extensions / AutoLinks / Valid Domain Tests
            //
            // The following Markdown:
            //     www._test.foo.bar is okay, but www._test.foo is not
            //     
            //     http://te_st.foo.bar is okay, as is http://test.foo_.bar.foo
            //     
            //     But http://te_st.foo, http://test.foo_.bar and http://test._foo are not
            //     
            //     ftp://test_.foo.bar is okay, but ftp://test.fo_o is not
            //     
            //     mailto:email@_test.foo.bar is okay, but mailto:email@_test.foo is not
            //
            // Should be rendered as:
            //     <p><a href="http://www._test.foo.bar">www._test.foo.bar</a> is okay, but www._test.foo is not</p>
            //     <p><a href="http://te_st.foo.bar">http://te_st.foo.bar</a> is okay, as is <a href="http://test.foo_.bar.foo">http://test.foo_.bar.foo</a></p>
            //     <p>But http://te_st.foo, http://test.foo_.bar and http://test._foo are not</p>
            //     <p><a href="ftp://test_.foo.bar">ftp://test_.foo.bar</a> is okay, but ftp://test.fo_o is not</p>
            //     <p><a href="mailto:email@_test.foo.bar">email@_test.foo.bar</a> is okay, but mailto:email@_test.foo is not</p>

            TestParser.TestSpec("www._test.foo.bar is okay, but www._test.foo is not\n\nhttp://te_st.foo.bar is okay, as is http://test.foo_.bar.foo\n\nBut http://te_st.foo, http://test.foo_.bar and http://test._foo are not\n\nftp://test_.foo.bar is okay, but ftp://test.fo_o is not\n\nmailto:email@_test.foo.bar is okay, but mailto:email@_test.foo is not", "<p><a href=\"http://www._test.foo.bar\">www._test.foo.bar</a> is okay, but www._test.foo is not</p>\n<p><a href=\"http://te_st.foo.bar\">http://te_st.foo.bar</a> is okay, as is <a href=\"http://test.foo_.bar.foo\">http://test.foo_.bar.foo</a></p>\n<p>But http://te_st.foo, http://test.foo_.bar and http://test._foo are not</p>\n<p><a href=\"ftp://test_.foo.bar\">ftp://test_.foo.bar</a> is okay, but ftp://test.fo_o is not</p>\n<p><a href=\"mailto:email@_test.foo.bar\">email@_test.foo.bar</a> is okay, but mailto:email@_test.foo is not</p>", "autolinks|advanced", context: "Example 18\nSection Extensions / AutoLinks / Valid Domain Tests\n");
        }

        // Domain names that contain invalid characters (not AlphaNumberic, -, _ or .) won't be matched
        [Test]
        public void ExtensionsAutoLinksValidDomainTests_Example019()
        {
            // Example 19
            // Section: Extensions / AutoLinks / Valid Domain Tests
            //
            // The following Markdown:
            //     https://[your-domain]/api
            //
            // Should be rendered as:
            //     <p>https://[your-domain]/api</p>

            TestParser.TestSpec("https://[your-domain]/api", "<p>https://[your-domain]/api</p>", "autolinks|advanced", context: "Example 19\nSection Extensions / AutoLinks / Valid Domain Tests\n");
        }

        // Domain names followed by ?, : or # instead of / are matched
        [Test]
        public void ExtensionsAutoLinksValidDomainTests_Example020()
        {
            // Example 20
            // Section: Extensions / AutoLinks / Valid Domain Tests
            //
            // The following Markdown:
            //     https://github.com?
            //     
            //     https://github.com?a
            //     
            //     https://github.com#a
            //     
            //     https://github.com:
            //     
            //     https://github.com:443
            //
            // Should be rendered as:
            //     <p><a href="https://github.com">https://github.com</a>?</p>
            //     <p><a href="https://github.com?a">https://github.com?a</a></p>
            //     <p><a href="https://github.com#a">https://github.com#a</a></p>
            //     <p><a href="https://github.com">https://github.com</a>:</p>
            //     <p><a href="https://github.com:443">https://github.com:443</a></p>

            TestParser.TestSpec("https://github.com?\n\nhttps://github.com?a\n\nhttps://github.com#a\n\nhttps://github.com:\n\nhttps://github.com:443", "<p><a href=\"https://github.com\">https://github.com</a>?</p>\n<p><a href=\"https://github.com?a\">https://github.com?a</a></p>\n<p><a href=\"https://github.com#a\">https://github.com#a</a></p>\n<p><a href=\"https://github.com\">https://github.com</a>:</p>\n<p><a href=\"https://github.com:443\">https://github.com:443</a></p>", "autolinks|advanced", context: "Example 20\nSection Extensions / AutoLinks / Valid Domain Tests\n");
        }
    }

    [TestFixture]
    public class TestExtensionsAutoLinksUnicodeSupport
    {
        // ### Unicode support
        // 
        // Links with unicode characters in the path / query / fragment are matched and url encoded
        [Test]
        public void ExtensionsAutoLinksUnicodeSupport_Example021()
        {
            // Example 21
            // Section: Extensions / AutoLinks / Unicode support
            //
            // The following Markdown:
            //     http://abc.net/☃
            //     
            //     http://abc.net?☃
            //     
            //     http://abc.net#☃
            //     
            //     http://abc.net/foo#☃
            //
            // Should be rendered as:
            //     <p><a href="http://abc.net/%E2%98%83">http://abc.net/☃</a></p>
            //     <p><a href="http://abc.net?%E2%98%83">http://abc.net?☃</a></p>
            //     <p><a href="http://abc.net#%E2%98%83">http://abc.net#☃</a></p>
            //     <p><a href="http://abc.net/foo#%E2%98%83">http://abc.net/foo#☃</a></p>

            TestParser.TestSpec("http://abc.net/☃\n\nhttp://abc.net?☃\n\nhttp://abc.net#☃\n\nhttp://abc.net/foo#☃", "<p><a href=\"http://abc.net/%E2%98%83\">http://abc.net/☃</a></p>\n<p><a href=\"http://abc.net?%E2%98%83\">http://abc.net?☃</a></p>\n<p><a href=\"http://abc.net#%E2%98%83\">http://abc.net#☃</a></p>\n<p><a href=\"http://abc.net/foo#%E2%98%83\">http://abc.net/foo#☃</a></p>", "autolinks|advanced", context: "Example 21\nSection Extensions / AutoLinks / Unicode support\n");
        }

        // Unicode characters in the FQDN are matched and IDNA encoded
        [Test]
        public void ExtensionsAutoLinksUnicodeSupport_Example022()
        {
            // Example 22
            // Section: Extensions / AutoLinks / Unicode support
            //
            // The following Markdown:
            //     http://☃.net?☃
            //
            // Should be rendered as:
            //     <p><a href="http://xn--n3h.net?%E2%98%83">http://☃.net?☃</a></p>

            TestParser.TestSpec("http://☃.net?☃", "<p><a href=\"http://xn--n3h.net?%E2%98%83\">http://☃.net?☃</a></p>", "autolinks|advanced", context: "Example 22\nSection Extensions / AutoLinks / Unicode support\n");
        }

        // Same goes for regular autolinks
        [Test]
        public void ExtensionsAutoLinksUnicodeSupport_Example023()
        {
            // Example 23
            // Section: Extensions / AutoLinks / Unicode support
            //
            // The following Markdown:
            //     <http://abc.net/☃>
            //     
            //     <http://abc.net?☃>
            //     
            //     <http://abc.net#☃>
            //     
            //     <http://abc.net/foo#☃>
            //
            // Should be rendered as:
            //     <p><a href="http://abc.net/%E2%98%83">http://abc.net/☃</a></p>
            //     <p><a href="http://abc.net?%E2%98%83">http://abc.net?☃</a></p>
            //     <p><a href="http://abc.net#%E2%98%83">http://abc.net#☃</a></p>
            //     <p><a href="http://abc.net/foo#%E2%98%83">http://abc.net/foo#☃</a></p>

            TestParser.TestSpec("<http://abc.net/☃>\n\n<http://abc.net?☃>\n\n<http://abc.net#☃>\n\n<http://abc.net/foo#☃>", "<p><a href=\"http://abc.net/%E2%98%83\">http://abc.net/☃</a></p>\n<p><a href=\"http://abc.net?%E2%98%83\">http://abc.net?☃</a></p>\n<p><a href=\"http://abc.net#%E2%98%83\">http://abc.net#☃</a></p>\n<p><a href=\"http://abc.net/foo#%E2%98%83\">http://abc.net/foo#☃</a></p>", "autolinks|advanced", context: "Example 23\nSection Extensions / AutoLinks / Unicode support\n");
        }

        [Test]
        public void ExtensionsAutoLinksUnicodeSupport_Example024()
        {
            // Example 24
            // Section: Extensions / AutoLinks / Unicode support
            //
            // The following Markdown:
            //     <http://☃.net?☃>
            //
            // Should be rendered as:
            //     <p><a href="http://xn--n3h.net?%E2%98%83">http://☃.net?☃</a></p>

            TestParser.TestSpec("<http://☃.net?☃>", "<p><a href=\"http://xn--n3h.net?%E2%98%83\">http://☃.net?☃</a></p>", "autolinks|advanced", context: "Example 24\nSection Extensions / AutoLinks / Unicode support\n");
        }

        // It also complies with CommonMark's vision of priority.
        // This will therefore be seen as an autolink and not as code inline.
        [Test]
        public void ExtensionsAutoLinksUnicodeSupport_Example025()
        {
            // Example 25
            // Section: Extensions / AutoLinks / Unicode support
            //
            // The following Markdown:
            //     <http://foö.bar.`baz>`
            //
            // Should be rendered as:
            //     <p><a href="http://xn--fo-gka.bar.%60baz">http://foö.bar.`baz</a>`</p>

            TestParser.TestSpec("<http://foö.bar.`baz>`", "<p><a href=\"http://xn--fo-gka.bar.%60baz\">http://foö.bar.`baz</a>`</p>", "autolinks|advanced", context: "Example 25\nSection Extensions / AutoLinks / Unicode support\n");
        }

        // Unicode punctuation characters are not allowed, but symbols are.
        // Note that this does _not_ exactly match CommonMark's "Unicode punctuation character" definition.
        [Test]
        public void ExtensionsAutoLinksUnicodeSupport_Example026()
        {
            // Example 26
            // Section: Extensions / AutoLinks / Unicode support
            //
            // The following Markdown:
            //     http://☃.net?☃ // OtherSymbol
            //     
            //     http://🍉.net?🍉 // A UTF-16 surrogate pair, but code point is OtherSymbol
            //     
            //     http://‰.net?‰ // OtherPunctuation
            //
            // Should be rendered as:
            //     <p><a href="http://xn--n3h.net?%E2%98%83">http://☃.net?☃</a> // OtherSymbol</p>
            //     <p><a href="http://xn--ji8h.net?%F0%9F%8D%89">http://🍉.net?🍉</a> // A UTF-16 surrogate pair, but code point is OtherSymbol</p>
            //     <p>http://‰.net?‰ // OtherPunctuation</p>

            TestParser.TestSpec("http://☃.net?☃ // OtherSymbol\n\nhttp://🍉.net?🍉 // A UTF-16 surrogate pair, but code point is OtherSymbol\n\nhttp://‰.net?‰ // OtherPunctuation", "<p><a href=\"http://xn--n3h.net?%E2%98%83\">http://☃.net?☃</a> // OtherSymbol</p>\n<p><a href=\"http://xn--ji8h.net?%F0%9F%8D%89\">http://🍉.net?🍉</a> // A UTF-16 surrogate pair, but code point is OtherSymbol</p>\n<p>http://‰.net?‰ // OtherPunctuation</p>", "autolinks|advanced", context: "Example 26\nSection Extensions / AutoLinks / Unicode support\n");
        }
    }
}
