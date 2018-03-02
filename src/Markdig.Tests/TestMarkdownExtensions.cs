using Markdig.Extensions.Emoji;
using Markdig.Extensions.Tables;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdig.Tests
{
    /// <summary>
    /// Test class for <see cref="MarkdownExtensions"/>
    /// <seealso cref="MarkdownPipelineBuilder"/>
    /// </summary>
    public class TestMarkdownExtensions
    {
        /// <summary>
        /// Testing for dynamic extension configuration
        /// </summary>
        [Test(Description = "Testing for dynamic extension configuration")]
        public void TestConfigure()
        {
            MarkdownPipelineBuilder builder = new MarkdownPipelineBuilder().Configure("emphasisextras+gridtable+pipetable");
            Assert.AreEqual(3, builder.Extensions.Count());
            Assert.IsInstanceOf(typeof(GridTableExtension), builder.Extensions[1]);

            builder = new MarkdownPipelineBuilder().Configure("advanced+emojis");
            Assert.AreEqual(19, builder.Extensions.Count());
            Assert.That(builder.Extensions.Any(extension => extension.GetType() == typeof(EmojiExtension)));

            builder = new MarkdownPipelineBuilder().Configure(typeof(GridTableExtension), typeof(PipeTableExtension));
            Assert.AreEqual(2, builder.Extensions.Count());
            Assert.That(builder.Extensions.Any(extension => extension.GetType() == typeof(GridTableExtension)));

            Assert.Throws<ArgumentException>(delegate { new MarkdownPipelineBuilder().Configure("xyz+123"); });
        }

        /// <summary>
        /// Test the extension name resolver
        /// </summary>
        [Test(Description = "Test the extension name resolver")]
        public void TestGetExtensionName()
        {
            Assert.AreEqual("emoji", MarkdownExtensions.GetExtensionName<EmojiExtension>());
            Assert.AreEqual("pipetable", MarkdownExtensions.GetExtensionName<PipeTableExtension>());

            Assert.AreEqual("emoji", MarkdownExtensions.GetExtensionName(typeof(EmojiExtension)));
            Assert.AreEqual("pipetable", MarkdownExtensions.GetExtensionName(typeof(PipeTableExtension)));
        }

        /// <summary>
        /// Test the extension name resolver with Configure
        /// </summary>
        [Test(Description = "Test the extension name resolver with Configure")]
        public void TestConfigureWithGetExtensionName()
        {
            MarkdownPipelineBuilder builder = new MarkdownPipelineBuilder().Configure(MarkdownExtensions.GetExtensionName<EmojiExtension>());
            Assert.AreEqual(1, builder.Extensions.Count());
            Assert.That(builder.Extensions.Any(extension => extension.GetType() == typeof(EmojiExtension)));
        }
    }
}
