using NUnit.Framework;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestExceptionNotThrown
    {
        [Test]
        public void DoesNotThrowIndexOutOfRangeException1()
        {
            Assert.DoesNotThrow(() =>
            {
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                Markdown.ToHtml("+-\n|\n+", pipeline);
            });
        }

        [Test]
        public void DoesNotThrowIndexOutOfRangeException2()
        {
            Assert.DoesNotThrow(() =>
            {
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                Markdown.ToHtml("+--\n|\n+0", pipeline);
            });
        }

        [Test]
        public void DoesNotThrowIndexOutOfRangeException3()
        {
            Assert.DoesNotThrow(() =>
            {
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                Markdown.ToHtml("+-\n|\n+\n0", pipeline);
            });
        }

        [Test]
        public void DoesNotThrowIndexOutOfRangeException4()
        {
            Assert.DoesNotThrow(() =>
            {
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                Markdown.ToHtml("+-\n|\n+0", pipeline);
            });
        }
    }
}
