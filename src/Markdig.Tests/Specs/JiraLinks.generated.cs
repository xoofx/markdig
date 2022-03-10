
// --------------------------------
//            Jira Links
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.JiraLinks
{
    [TestFixture]
    public class TestJiraLinks
    {
        // ## Jira Links
        // 
        // The JiraLinks extension will automatically add links to JIRA issue items within your markdown, e.g. XX-1234. For this to happen, you must configure the extension when adding to the pipeline, e.g.
        // 
        // ```
        // var pipeline = new MarkdownPipelineBuilder()
        // 	.UseJiraLinks(new JiraLinkOptions("http://your.company.abc"))
        // 	.Build();
        // ```
        // 
        // The rules for detecting a link are:
        // 
        // - The project key must be composed of one or more capitalized ASCII letters or digits `[A-Z,0-9]+`
        // - A single hyphen `-` must separate the project key and issue number.
        // - The issue number is composed of 1 or more digits `[0, 9]+`
        // - The reference must be preceded by either `(` or whitespace or EOF.
        // - The reference must be followed by either `)` or whitespace or EOF.
        // 
        // The following are valid examples:
        [Test]
        public void JiraLinks_Example001()
        {
            // Example 1
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is a ABCD-123 issue
            //
            // Should be rendered as:
            //     <p>This is a <a href="http://your.company.abc/browse/ABCD-123" target="_blank">ABCD-123</a> issue</p>

            TestParser.TestSpec("This is a ABCD-123 issue", "<p>This is a <a href=\"http://your.company.abc/browse/ABCD-123\" target=\"_blank\">ABCD-123</a> issue</p>", "jiralinks", context: "Example 1\nSection Jira Links\n");
        }

        [Test]
        public void JiraLinks_Example002()
        {
            // Example 2
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is a ABC4-123 issue
            //
            // Should be rendered as:
            //     <p>This is a <a href="http://your.company.abc/browse/ABC4-123" target="_blank">ABC4-123</a> issue</p>

            TestParser.TestSpec("This is a ABC4-123 issue", "<p>This is a <a href=\"http://your.company.abc/browse/ABC4-123\" target=\"_blank\">ABC4-123</a> issue</p>", "jiralinks", context: "Example 2\nSection Jira Links\n");
        }

        [Test]
        public void JiraLinks_Example003()
        {
            // Example 3
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is a ABC45-123 issue
            //
            // Should be rendered as:
            //     <p>This is a <a href="http://your.company.abc/browse/ABC45-123" target="_blank">ABC45-123</a> issue</p>

            TestParser.TestSpec("This is a ABC45-123 issue", "<p>This is a <a href=\"http://your.company.abc/browse/ABC45-123\" target=\"_blank\">ABC45-123</a> issue</p>", "jiralinks", context: "Example 3\nSection Jira Links\n");
        }

        [Test]
        public void JiraLinks_Example004()
        {
            // Example 4
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is a KIRA-1 issue
            //
            // Should be rendered as:
            //     <p>This is a <a href="http://your.company.abc/browse/KIRA-1" target="_blank">KIRA-1</a> issue</p>

            TestParser.TestSpec("This is a KIRA-1 issue", "<p>This is a <a href=\"http://your.company.abc/browse/KIRA-1\" target=\"_blank\">KIRA-1</a> issue</p>", "jiralinks", context: "Example 4\nSection Jira Links\n");
        }

        [Test]
        public void JiraLinks_Example005()
        {
            // Example 5
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is a Z-1 issue
            //
            // Should be rendered as:
            //     <p>This is a <a href="http://your.company.abc/browse/Z-1" target="_blank">Z-1</a> issue</p>

            TestParser.TestSpec("This is a Z-1 issue", "<p>This is a <a href=\"http://your.company.abc/browse/Z-1\" target=\"_blank\">Z-1</a> issue</p>", "jiralinks", context: "Example 5\nSection Jira Links\n");
        }

        // These are also valid links with `(` and `)`:
        [Test]
        public void JiraLinks_Example006()
        {
            // Example 6
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is a (ABCD-123) issue
            //
            // Should be rendered as:
            //     <p>This is a (<a href="http://your.company.abc/browse/ABCD-123" target="_blank">ABCD-123</a>) issue</p>

            TestParser.TestSpec("This is a (ABCD-123) issue", "<p>This is a (<a href=\"http://your.company.abc/browse/ABCD-123\" target=\"_blank\">ABCD-123</a>) issue</p>", "jiralinks", context: "Example 6\nSection Jira Links\n");
        }

        [Test]
        public void JiraLinks_Example007()
        {
            // Example 7
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is a (ABC4-123) issue
            //
            // Should be rendered as:
            //     <p>This is a (<a href="http://your.company.abc/browse/ABC4-123" target="_blank">ABC4-123</a>) issue</p>

            TestParser.TestSpec("This is a (ABC4-123) issue", "<p>This is a (<a href=\"http://your.company.abc/browse/ABC4-123\" target=\"_blank\">ABC4-123</a>) issue</p>", "jiralinks", context: "Example 7\nSection Jira Links\n");
        }

        [Test]
        public void JiraLinks_Example008()
        {
            // Example 8
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is a (KIRA-1) issue
            //
            // Should be rendered as:
            //     <p>This is a (<a href="http://your.company.abc/browse/KIRA-1" target="_blank">KIRA-1</a>) issue</p>

            TestParser.TestSpec("This is a (KIRA-1) issue", "<p>This is a (<a href=\"http://your.company.abc/browse/KIRA-1\" target=\"_blank\">KIRA-1</a>) issue</p>", "jiralinks", context: "Example 8\nSection Jira Links\n");
        }

        [Test]
        public void JiraLinks_Example009()
        {
            // Example 9
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is a (Z-1) issue
            //
            // Should be rendered as:
            //     <p>This is a (<a href="http://your.company.abc/browse/Z-1" target="_blank">Z-1</a>) issue</p>

            TestParser.TestSpec("This is a (Z-1) issue", "<p>This is a (<a href=\"http://your.company.abc/browse/Z-1\" target=\"_blank\">Z-1</a>) issue</p>", "jiralinks", context: "Example 9\nSection Jira Links\n");
        }

        // These are not valid links:
        [Test]
        public void JiraLinks_Example010()
        {
            // Example 10
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is not aJIRA-123 issue
            //
            // Should be rendered as:
            //     <p>This is not aJIRA-123 issue</p>

            TestParser.TestSpec("This is not aJIRA-123 issue", "<p>This is not aJIRA-123 issue</p>", "jiralinks", context: "Example 10\nSection Jira Links\n");
        }

        [Test]
        public void JiraLinks_Example011()
        {
            // Example 11
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is not 4JIRA-123 issue
            //
            // Should be rendered as:
            //     <p>This is not 4JIRA-123 issue</p>

            TestParser.TestSpec("This is not 4JIRA-123 issue", "<p>This is not 4JIRA-123 issue</p>", "jiralinks", context: "Example 11\nSection Jira Links\n");
        }

        [Test]
        public void JiraLinks_Example012()
        {
            // Example 12
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is not JIRA-123a issue
            //
            // Should be rendered as:
            //     <p>This is not JIRA-123a issue</p>

            TestParser.TestSpec("This is not JIRA-123a issue", "<p>This is not JIRA-123a issue</p>", "jiralinks", context: "Example 12\nSection Jira Links\n");
        }

        [Test]
        public void JiraLinks_Example013()
        {
            // Example 13
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is not JIRA- issue
            //
            // Should be rendered as:
            //     <p>This is not JIRA- issue</p>

            TestParser.TestSpec("This is not JIRA- issue", "<p>This is not JIRA- issue</p>", "jiralinks", context: "Example 13\nSection Jira Links\n");
        }

        [Test]
        public void JiraLinks_Example014()
        {
            // Example 14
            // Section: Jira Links
            //
            // The following Markdown:
            //     This is not JIR4- issue
            //
            // Should be rendered as:
            //     <p>This is not JIR4- issue</p>

            TestParser.TestSpec("This is not JIR4- issue", "<p>This is not JIR4- issue</p>", "jiralinks", context: "Example 14\nSection Jira Links\n");
        }
    }
}
