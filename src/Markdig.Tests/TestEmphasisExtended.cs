using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestEmphasisExtended
    {
        class EmphasisTestExtension : IMarkdownExtension
        {
            public void Setup(MarkdownPipelineBuilder pipeline)
            {
                var emphasisParser = pipeline.InlineParsers.Find<EmphasisInlineParser>();
                Debug.Assert(emphasisParser != null);

                foreach (var emphasis in EmphasisTestDescriptors)
                {
                    emphasisParser.EmphasisDescriptors.Add(
                        new EmphasisDescriptor(emphasis.Character, emphasis.Minimum, emphasis.Maximum, true));
                }
                emphasisParser.TryCreateEmphasisInlineList.Add((delimiterChar, delimiterCount) =>
                {
                    return delimiterChar == '*' || delimiterChar == '_'
                        ? null
                        : new CustomEmphasisInline() { DelimiterChar = delimiterChar, DelimiterCount = delimiterCount };
                });
            }

            public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
            {
                renderer.ObjectRenderers.Insert(0, new EmphasisRenderer());
            }

            class EmphasisRenderer : HtmlObjectRenderer<CustomEmphasisInline>
            {
                protected override void Write(HtmlRenderer renderer, CustomEmphasisInline obj)
                {
                    var tag = EmphasisTestDescriptors.First(test => test.Character == obj.DelimiterChar).Tags[obj.DelimiterCount];

                    renderer.Write(tag.OpeningTag);
                    renderer.WriteChildren(obj);
                    renderer.Write(tag.ClosingTag);
                }
            }
        }
        class Tag
        {
#pragma warning disable CS0649
            public int Level;
#pragma warning restore CS0649
            public string RawTag;
            public string OpeningTag;
            public string ClosingTag;

            public Tag(string tag)
            {
                RawTag = tag;
                OpeningTag = "<" + tag + ">";
                ClosingTag = "</" + tag + ">";
            }

            public static implicit operator Tag(string tag)
                => new Tag(tag);
        }
        class EmphasisTestDescriptor
        {
            public char Character;
            public int Minimum;
            public int Maximum;
            public Dictionary<int, Tag> Tags = new Dictionary<int, Tag>();

            private EmphasisTestDescriptor(char character, int min, int max)
            {
                Character = character;
                Minimum = min;
                Maximum = max;
            }
            public EmphasisTestDescriptor(char character, int min, int max, params Tag[] tags)
                : this(character, min, max)
            {
                Debug.Assert(tags.Length == max - min + 1);
                foreach (var tag in tags)
                {
                    Tags.Add(min++, tag);
                }
            }
            public EmphasisTestDescriptor(char character, int min, int max, string tag)
                : this(character, min, max, new Tag(tag)) { }
        }
        class CustomEmphasisInline : EmphasisInline { }
        static readonly EmphasisTestDescriptor[] EmphasisTestDescriptors = new[]
        {
            //                            Min Max
            new EmphasisTestDescriptor('"', 1, 1, "quotation"),
            new EmphasisTestDescriptor(',', 1, 2, "comma", "extra-comma"),
            new EmphasisTestDescriptor('!', 2, 3, "warning", "error"),
            new EmphasisTestDescriptor('=', 1, 3, "equal", "really-equal", "congruent"),
            new EmphasisTestDescriptor('1', 1, 1, "one-only"),
            new EmphasisTestDescriptor('2', 2, 2, "two-only"),
            new EmphasisTestDescriptor('3', 3, 3, "three-only"),
        };

        static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().Use<EmphasisTestExtension>().Build();

        [Test]
        [TestCase("*foo**",         "<em>foo</em>*")]
        [TestCase("**foo*",         "*<em>foo</em>")]
        [TestCase("***foo***",      "<em><strong>foo</strong></em>")]
        [TestCase("**_foo_**",      "<strong><em>foo</em></strong>")]
        [TestCase("_**foo**_",      "<em><strong>foo</strong></em>")]
        [TestCase("\"foo\"",        "<quotation>foo</quotation>")]
        [TestCase("\"\"foo\"\"",    "<quotation><quotation>foo</quotation></quotation>")]
        [TestCase("\"foo\"\"",      "<quotation>foo</quotation>&quot;")]
        [TestCase("\"\"foo\"",      "&quot;<quotation>foo</quotation>")]
        [TestCase(", foo",          ", foo")]
        [TestCase(", foo,",         ", foo,")]
        [TestCase(",some, foo,",    "<comma>some</comma> foo,")]
        [TestCase(",,foo,,",        "<extra-comma>foo</extra-comma>")]
        [TestCase(",foo,,",         "<comma>foo</comma>,")]
        [TestCase(",,,foo,,,",      "<comma><extra-comma>foo</extra-comma></comma>")]
        [TestCase("!1!",            "!1!")]
        [TestCase("!!2!!",          "<warning>2</warning>")]
        [TestCase("!!!3!!!",        "<error>3</error>")]
        [TestCase("!!!34!!!!",      "<error>34</error>!")]
        [TestCase("!!!!43!!!",      "!<error>43</error>")]
        [TestCase("!!!!44!!!!",     "!<error>44!</error>")] // This is a new case - should the second ! be before or after </error>?
        [TestCase("!!!!!5!!!!!",    "<warning><error>5</error></warning>")]
        [TestCase("!!!!!!6!!!!!!",  "<error><error>6</error></error>")]
        [TestCase("!! !mixed!!!",   "!! !mixed!!!")] // can't open the delimiter because of the whitespace
        [TestCase("=",              "=")]
        [TestCase("==",             "==")]
        [TestCase("====",           "====")]
        [TestCase("=a",             "=a")]
        [TestCase("=a=",            "<equal>a</equal>")]
        [TestCase("==a=",           "=<equal>a</equal>")]
        [TestCase("==a==",          "<really-equal>a</really-equal>")]
        [TestCase("==a===",         "<really-equal>a</really-equal>=")]
        [TestCase("===a===",        "<congruent>a</congruent>")]
        [TestCase("====a====",      "<equal><congruent>a</congruent></equal>")]
        [TestCase("=====a=====",    "<really-equal><congruent>a</congruent></really-equal>")]
        [TestCase("1",              "1")]
        [TestCase("1 1",            "1 1")]
        [TestCase("1Foo1",          "<one-only>Foo</one-only>")]
        [TestCase("1121",           "1<one-only>2</one-only>")]
        [TestCase("22322",          "<two-only>3</two-only>")]
        [TestCase("2232",           "2232")]
        [TestCase("333",            "333")]
        [TestCase("3334333",        "<three-only>4</three-only>")]
        [TestCase("33334333",       "3<three-only>4</three-only>")]
        [TestCase("33343333",       "<three-only>4</three-only>3")]
        [TestCase("122122",         "<one-only>22</one-only>22")]
        [TestCase("221221",         "<two-only>1</two-only>1")]
        [TestCase("122foo221",      "<one-only><two-only>foo</two-only></one-only>")]
        [TestCase("122foo122",      "<one-only>22foo</one-only>22")]
        [TestCase("!!!!!Attention:!! \"==1+1== 2\",but ===333 and 222===, mod 111!!!",
            "<error><warning>Attention:</warning> <quotation><really-equal><one-only>+</one-only></really-equal> 2</quotation><comma>but <congruent>333 and 222</congruent></comma> mod 111</error>")]
        public void TestEmphasis(string markdown, string expectedHtml)
        {
            TestParser.TestSpec(markdown, "<p>" + expectedHtml + "</p>", Pipeline);
        }
    }
}
