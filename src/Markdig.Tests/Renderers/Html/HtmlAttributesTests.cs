using Markdig.Renderers.Html;
using NUnit.Framework;

namespace Markdig.Tests.Renderers.Html
{
    [TestFixture]
    public class HtmlAttributesTests
    {
        public class CopyTo : HtmlAttributesTests
        {
            [Test]
            public void ShouldNotThrowExceptionWhenSharedIsFalseAndClassesIsNull()
            {
                var sut = new HtmlAttributes();

                Assert.Null(sut.Classes);

                Assert.DoesNotThrow(() => sut.CopyTo(new HtmlAttributes(), shared: false));
            }

            [Test]
            public void ShouldNotThrowExceptionWhenSharedIsFalseAndPropertiesIsNull()
            {
                var sut = new HtmlAttributes();

                Assert.Null(sut.Properties);

                Assert.DoesNotThrow(() => sut.CopyTo(new HtmlAttributes(), shared: false));
            }
        }
    }
}
