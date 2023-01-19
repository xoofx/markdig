using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

[TestFixture]
public class TestYamlFrontMatterBlock
{
    [TestCase("---\nkey1: value1\nkey2: value2\n---\n\nContent\n")]
    [TestCase("No front matter")]
    [TestCase("Looks like front matter but actually is not\n---\nkey1: value1\nkey2: value2\n---")]
    public void FrontMatterBlockIsPreserved(string value)
    {
        RoundTrip(value);
    }
}
