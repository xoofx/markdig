
// --------------------------------
//           Alert Blocks
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.AlertBlocks
{
    [TestFixture]
    public class TestExtensionsAlertBlocks
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Alert Blocks
        // 
        // This is supporting the [GitHub Alert blocks](https://github.com/orgs/community/discussions/16925)
        [Test]
        public void ExtensionsAlertBlocks_Example001()
        {
            // Example 1
            // Section: Extensions / Alert Blocks
            //
            // The following Markdown:
            //     > [!NOTE]  
            //     > Highlights information that users should take into account, even when skimming.
            //     
            //     > [!TIP]
            //     > Optional information to help a user be more successful.
            //     
            //     > [!IMPORTANT]  
            //     > Crucial information necessary for users to succeed.
            //     
            //     > [!WARNING]  
            //     > Critical content demanding immediate user attention due to potential risks.
            //     
            //     > [!CAUTION]
            //     > Negative potential consequences of an action.
            //
            // Should be rendered as:
            //     <div class="markdown-alert markdown-alert-note">
            //     <p class="markdown-alert-title"><svg viewBox="0 0 16 16" version="1.1" width="16" height="16" aria-hidden="true"><path d="M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8Zm8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13ZM6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75ZM8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z"></path></svg>Note</p>
            //     <p>Highlights information that users should take into account, even when skimming.</p>
            //     </div>
            //     <div class="markdown-alert markdown-alert-tip">
            //     <p class="markdown-alert-title"><svg viewBox="0 0 16 16" version="1.1" width="16" height="16" aria-hidden="true"><path d="M8 1.5c-2.363 0-4 1.69-4 3.75 0 .984.424 1.625.984 2.304l.214.253c.223.264.47.556.673.848.284.411.537.896.621 1.49a.75.75 0 0 1-1.484.211c-.04-.282-.163-.547-.37-.847a8.456 8.456 0 0 0-.542-.68c-.084-.1-.173-.205-.268-.32C3.201 7.75 2.5 6.766 2.5 5.25 2.5 2.31 4.863 0 8 0s5.5 2.31 5.5 5.25c0 1.516-.701 2.5-1.328 3.259-.095.115-.184.22-.268.319-.207.245-.383.453-.541.681-.208.3-.33.565-.37.847a.751.751 0 0 1-1.485-.212c.084-.593.337-1.078.621-1.489.203-.292.45-.584.673-.848.075-.088.147-.173.213-.253.561-.679.985-1.32.985-2.304 0-2.06-1.637-3.75-4-3.75ZM5.75 12h4.5a.75.75 0 0 1 0 1.5h-4.5a.75.75 0 0 1 0-1.5ZM6 15.25a.75.75 0 0 1 .75-.75h2.5a.75.75 0 0 1 0 1.5h-2.5a.75.75 0 0 1-.75-.75Z"></path></svg>Tip</p>
            //     <p>Optional information to help a user be more successful.</p>
            //     </div>
            //     <div class="markdown-alert markdown-alert-important">
            //     <p class="markdown-alert-title"><svg viewBox="0 0 16 16" version="1.1" width="16" height="16" aria-hidden="true"><path d="M0 1.75C0 .784.784 0 1.75 0h12.5C15.216 0 16 .784 16 1.75v9.5A1.75 1.75 0 0 1 14.25 13H8.06l-2.573 2.573A1.458 1.458 0 0 1 3 14.543V13H1.75A1.75 1.75 0 0 1 0 11.25Zm1.75-.25a.25.25 0 0 0-.25.25v9.5c0 .138.112.25.25.25h2a.75.75 0 0 1 .75.75v2.19l2.72-2.72a.749.749 0 0 1 .53-.22h6.5a.25.25 0 0 0 .25-.25v-9.5a.25.25 0 0 0-.25-.25Zm7 2.25v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 9a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z"></path></svg>Important</p>
            //     <p>Crucial information necessary for users to succeed.</p>
            //     </div>
            //     <div class="markdown-alert markdown-alert-warning">
            //     <p class="markdown-alert-title"><svg viewBox="0 0 16 16" version="1.1" width="16" height="16" aria-hidden="true"><path d="M6.457 1.047c.659-1.234 2.427-1.234 3.086 0l6.082 11.378A1.75 1.75 0 0 1 14.082 15H1.918a1.75 1.75 0 0 1-1.543-2.575Zm1.763.707a.25.25 0 0 0-.44 0L1.698 13.132a.25.25 0 0 0 .22.368h12.164a.25.25 0 0 0 .22-.368Zm.53 3.996v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 11a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z"></path></svg>Warning</p>
            //     <p>Critical content demanding immediate user attention due to potential risks.</p>
            //     </div>
            //     <div class="markdown-alert markdown-alert-caution">
            //     <p class="markdown-alert-title"><svg viewBox="0 0 16 16" version="1.1" width="16" height="16" aria-hidden="true"><path d="M4.47.22A.749.749 0 0 1 5 0h6c.199 0 .389.079.53.22l4.25 4.25c.141.14.22.331.22.53v6a.749.749 0 0 1-.22.53l-4.25 4.25A.749.749 0 0 1 11 16H5a.749.749 0 0 1-.53-.22L.22 11.53A.749.749 0 0 1 0 11V5c0-.199.079-.389.22-.53Zm.84 1.28L1.5 5.31v5.38l3.81 3.81h5.38l3.81-3.81V5.31L10.69 1.5ZM8 4a.75.75 0 0 1 .75.75v3.5a.75.75 0 0 1-1.5 0v-3.5A.75.75 0 0 1 8 4Zm0 8a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z"></path></svg>Caution</p>
            //     <p>Negative potential consequences of an action.</p>
            //     </div>

            TestParser.TestSpec("> [!NOTE]  \n> Highlights information that users should take into account, even when skimming.\n\n> [!TIP]\n> Optional information to help a user be more successful.\n\n> [!IMPORTANT]  \n> Crucial information necessary for users to succeed.\n\n> [!WARNING]  \n> Critical content demanding immediate user attention due to potential risks.\n\n> [!CAUTION]\n> Negative potential consequences of an action.", "<div class=\"markdown-alert markdown-alert-note\">\n<p class=\"markdown-alert-title\"><svg viewBox=\"0 0 16 16\" version=\"1.1\" width=\"16\" height=\"16\" aria-hidden=\"true\"><path d=\"M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8Zm8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13ZM6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75ZM8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z\"></path></svg>Note</p>\n<p>Highlights information that users should take into account, even when skimming.</p>\n</div>\n<div class=\"markdown-alert markdown-alert-tip\">\n<p class=\"markdown-alert-title\"><svg viewBox=\"0 0 16 16\" version=\"1.1\" width=\"16\" height=\"16\" aria-hidden=\"true\"><path d=\"M8 1.5c-2.363 0-4 1.69-4 3.75 0 .984.424 1.625.984 2.304l.214.253c.223.264.47.556.673.848.284.411.537.896.621 1.49a.75.75 0 0 1-1.484.211c-.04-.282-.163-.547-.37-.847a8.456 8.456 0 0 0-.542-.68c-.084-.1-.173-.205-.268-.32C3.201 7.75 2.5 6.766 2.5 5.25 2.5 2.31 4.863 0 8 0s5.5 2.31 5.5 5.25c0 1.516-.701 2.5-1.328 3.259-.095.115-.184.22-.268.319-.207.245-.383.453-.541.681-.208.3-.33.565-.37.847a.751.751 0 0 1-1.485-.212c.084-.593.337-1.078.621-1.489.203-.292.45-.584.673-.848.075-.088.147-.173.213-.253.561-.679.985-1.32.985-2.304 0-2.06-1.637-3.75-4-3.75ZM5.75 12h4.5a.75.75 0 0 1 0 1.5h-4.5a.75.75 0 0 1 0-1.5ZM6 15.25a.75.75 0 0 1 .75-.75h2.5a.75.75 0 0 1 0 1.5h-2.5a.75.75 0 0 1-.75-.75Z\"></path></svg>Tip</p>\n<p>Optional information to help a user be more successful.</p>\n</div>\n<div class=\"markdown-alert markdown-alert-important\">\n<p class=\"markdown-alert-title\"><svg viewBox=\"0 0 16 16\" version=\"1.1\" width=\"16\" height=\"16\" aria-hidden=\"true\"><path d=\"M0 1.75C0 .784.784 0 1.75 0h12.5C15.216 0 16 .784 16 1.75v9.5A1.75 1.75 0 0 1 14.25 13H8.06l-2.573 2.573A1.458 1.458 0 0 1 3 14.543V13H1.75A1.75 1.75 0 0 1 0 11.25Zm1.75-.25a.25.25 0 0 0-.25.25v9.5c0 .138.112.25.25.25h2a.75.75 0 0 1 .75.75v2.19l2.72-2.72a.749.749 0 0 1 .53-.22h6.5a.25.25 0 0 0 .25-.25v-9.5a.25.25 0 0 0-.25-.25Zm7 2.25v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 9a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z\"></path></svg>Important</p>\n<p>Crucial information necessary for users to succeed.</p>\n</div>\n<div class=\"markdown-alert markdown-alert-warning\">\n<p class=\"markdown-alert-title\"><svg viewBox=\"0 0 16 16\" version=\"1.1\" width=\"16\" height=\"16\" aria-hidden=\"true\"><path d=\"M6.457 1.047c.659-1.234 2.427-1.234 3.086 0l6.082 11.378A1.75 1.75 0 0 1 14.082 15H1.918a1.75 1.75 0 0 1-1.543-2.575Zm1.763.707a.25.25 0 0 0-.44 0L1.698 13.132a.25.25 0 0 0 .22.368h12.164a.25.25 0 0 0 .22-.368Zm.53 3.996v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 11a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z\"></path></svg>Warning</p>\n<p>Critical content demanding immediate user attention due to potential risks.</p>\n</div>\n<div class=\"markdown-alert markdown-alert-caution\">\n<p class=\"markdown-alert-title\"><svg viewBox=\"0 0 16 16\" version=\"1.1\" width=\"16\" height=\"16\" aria-hidden=\"true\"><path d=\"M4.47.22A.749.749 0 0 1 5 0h6c.199 0 .389.079.53.22l4.25 4.25c.141.14.22.331.22.53v6a.749.749 0 0 1-.22.53l-4.25 4.25A.749.749 0 0 1 11 16H5a.749.749 0 0 1-.53-.22L.22 11.53A.749.749 0 0 1 0 11V5c0-.199.079-.389.22-.53Zm.84 1.28L1.5 5.31v5.38l3.81 3.81h5.38l3.81-3.81V5.31L10.69 1.5ZM8 4a.75.75 0 0 1 .75.75v3.5a.75.75 0 0 1-1.5 0v-3.5A.75.75 0 0 1 8 4Zm0 8a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z\"></path></svg>Caution</p>\n<p>Negative potential consequences of an action.</p>\n</div>", "advanced", context: "Example 1\nSection Extensions / Alert Blocks\n");
        }

        // Example with code blocks and mix formatting:
        [Test]
        public void ExtensionsAlertBlocks_Example002()
        {
            // Example 2
            // Section: Extensions / Alert Blocks
            //
            // The following Markdown:
            //     > [!NOTE]
            //     > Highlights information that users should take into account, even when skimming.
            //     > Testing rendering for multiple lines
            //     > ```csharp
            //     > var test = "I can also add code to panels
            //     > ```
            //     > `Inline code testing`
            //
            // Should be rendered as:
            //     <div class="markdown-alert markdown-alert-note">
            //     <p class="markdown-alert-title"><svg viewBox="0 0 16 16" version="1.1" width="16" height="16" aria-hidden="true"><path d="M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8Zm8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13ZM6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75ZM8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z"></path></svg>Note</p>
            //     <p>Highlights information that users should take into account, even when skimming.
            //     Testing rendering for multiple lines</p>
            //     <pre><code class="language-csharp">var test = &quot;I can also add code to panels
            //     </code></pre>
            //     <p><code>Inline code testing</code></p>
            //     </div>

            TestParser.TestSpec("> [!NOTE]\n> Highlights information that users should take into account, even when skimming.\n> Testing rendering for multiple lines\n> ```csharp\n> var test = \"I can also add code to panels\n> ```\n> `Inline code testing`", "<div class=\"markdown-alert markdown-alert-note\">\n<p class=\"markdown-alert-title\"><svg viewBox=\"0 0 16 16\" version=\"1.1\" width=\"16\" height=\"16\" aria-hidden=\"true\"><path d=\"M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8Zm8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13ZM6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75ZM8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z\"></path></svg>Note</p>\n<p>Highlights information that users should take into account, even when skimming.\nTesting rendering for multiple lines</p>\n<pre><code class=\"language-csharp\">var test = &quot;I can also add code to panels\n</code></pre>\n<p><code>Inline code testing</code></p>\n</div>", "advanced", context: "Example 2\nSection Extensions / Alert Blocks\n");
        }

        // Multiline:
        [Test]
        public void ExtensionsAlertBlocks_Example003()
        {
            // Example 3
            // Section: Extensions / Alert Blocks
            //
            // The following Markdown:
            //     > [!NOTE]
            //     > Highlights information that users should take into account, even when skimming.
            //     > 
            //     > Testing rendering for multiple lines
            //     > 
            //     > `Inline code testing`
            //     > 
            //     > Other line
            //     > 
            //     > > Nested quote
            //     > >
            //     > > Final nested quote line
            //     > 
            //     > Final line of alert
            //
            // Should be rendered as:
            //     <div class="markdown-alert markdown-alert-note">
            //     <p class="markdown-alert-title"><svg viewBox="0 0 16 16" version="1.1" width="16" height="16" aria-hidden="true"><path d="M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8Zm8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13ZM6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75ZM8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z"></path></svg>Note</p>
            //     <p>Highlights information that users should take into account, even when skimming.</p>
            //     <p>Testing rendering for multiple lines</p>
            //     <p><code>Inline code testing</code></p>
            //     <p>Other line</p>
            //     <blockquote>
            //     <p>Nested quote</p>
            //     <p>Final nested quote line</p>
            //     </blockquote>
            //     <p>Final line of alert</p>
            //     </div>

            TestParser.TestSpec("> [!NOTE]\n> Highlights information that users should take into account, even when skimming.\n> \n> Testing rendering for multiple lines\n> \n> `Inline code testing`\n> \n> Other line\n> \n> > Nested quote\n> >\n> > Final nested quote line\n> \n> Final line of alert", "<div class=\"markdown-alert markdown-alert-note\">\n<p class=\"markdown-alert-title\"><svg viewBox=\"0 0 16 16\" version=\"1.1\" width=\"16\" height=\"16\" aria-hidden=\"true\"><path d=\"M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8Zm8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13ZM6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75ZM8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z\"></path></svg>Note</p>\n<p>Highlights information that users should take into account, even when skimming.</p>\n<p>Testing rendering for multiple lines</p>\n<p><code>Inline code testing</code></p>\n<p>Other line</p>\n<blockquote>\n<p>Nested quote</p>\n<p>Final nested quote line</p>\n</blockquote>\n<p>Final line of alert</p>\n</div>", "advanced", context: "Example 3\nSection Extensions / Alert Blocks\n");
        }

        // An alert inline (e.g `[!NOTE]`) must come first in a quote block, and must be followed by optional spaces with a new line. If no new lines are found, it will not be considered as an alert block.
        // 
        // Followed by space and new line:
        [Test]
        public void ExtensionsAlertBlocks_Example004()
        {
            // Example 4
            // Section: Extensions / Alert Blocks
            //
            // The following Markdown:
            //     > [!NOTE] This is invalid because no new line
            //     > Highlights information that users should take into account, even when skimming.
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>[!NOTE] This is invalid because no new line
            //     Highlights information that users should take into account, even when skimming.</p>
            //     </blockquote>

            TestParser.TestSpec("> [!NOTE] This is invalid because no new line\n> Highlights information that users should take into account, even when skimming.", "<blockquote>\n<p>[!NOTE] This is invalid because no new line\nHighlights information that users should take into account, even when skimming.</p>\n</blockquote>", "advanced", context: "Example 4\nSection Extensions / Alert Blocks\n");
        }

        // Must come first in a quote block:
        [Test]
        public void ExtensionsAlertBlocks_Example005()
        {
            // Example 5
            // Section: Extensions / Alert Blocks
            //
            // The following Markdown:
            //     > This is not a [!NOTE]
            //     > Highlights information that users should take into account, even when skimming.
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>This is not a [!NOTE]
            //     Highlights information that users should take into account, even when skimming.</p>
            //     </blockquote>

            TestParser.TestSpec("> This is not a [!NOTE]\n> Highlights information that users should take into account, even when skimming.", "<blockquote>\n<p>This is not a [!NOTE]\nHighlights information that users should take into account, even when skimming.</p>\n</blockquote>", "advanced", context: "Example 5\nSection Extensions / Alert Blocks\n");
        }
    }
}
