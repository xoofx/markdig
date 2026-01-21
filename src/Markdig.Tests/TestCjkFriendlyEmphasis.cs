// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestCjkFriendlyEmphasis
    {
        private static MarkdownPipeline GetPipeline() => new MarkdownPipelineBuilder().UseCjkFriendlyEmphasis().Build();
        
        private static MarkdownPipeline GetPipelineWithStrikethrough() => new MarkdownPipelineBuilder()
            .UseCjkFriendlyEmphasis()
            .UseEmphasisExtras()
            .Build();

        [Test]
        [TestCase("これは**私のやりたかったこと。**だからするの。", "<p>これは<strong>私のやりたかったこと。</strong>だからするの。</p>\n")]
        [TestCase("**[製品ほげ](./product-foo)**と**[製品ふが](./product-bar)**をお試しください", "<p><strong><a href=\"./product-foo\">製品ほげ</a></strong>と<strong><a href=\"./product-bar\">製品ふが</a></strong>をお試しください</p>\n")]
        [TestCase("先頭の**`コード`も注意。**", "<p>先頭の<strong><code>コード</code>も注意。</strong></p>\n")]
        [TestCase("**末尾の`コード`**も注意。", "<p><strong>末尾の<code>コード</code></strong>も注意。</p>\n")]
        [TestCase("税込**¥10,000**で入手できます。", "<p>税込<strong>¥10,000</strong>で入手できます。</p>\n")]
        [TestCase("""太郎は**"こんにちわ"**といった""", "<p>太郎は<strong>&quot;こんにちわ&quot;</strong>といった</p>\n")]
        [TestCase("**C#**や**F#**は**「.NET」**というプラットフォーム上で動作します。", "<p><strong>C#</strong>や<strong>F#</strong>は<strong>「.NET」</strong>というプラットフォーム上で動作します。</p>\n")]
        [TestCase(".NET**（.NET Frameworkは不可）**では、", "<p>.NET<strong>（.NET Frameworkは不可）</strong>では、</p>\n")]
        [TestCase("大塚︀**(U+585A U+FE00)** 大塚**(U+FA10)**", "<p>大塚︀<strong>(U+585A U+FE00)</strong> 大塚<strong>(U+FA10)</strong></p>\n")]
        [TestCase("〽︎**(庵点)**は、\n\n","<p>〽︎<strong>(庵点)</strong>は、</p>\n")]
        [TestCase("**true。︁**false\n\n", "<p><strong>true。︁</strong>false</p>\n")]
        [TestCase("禰󠄀**(ね)**豆子", "<p>禰󠄀<strong>(ね)</strong>豆子</p>\n")]
        public void TestCjkFriendlyEmphasisJapanese(string source, string expected)
        {
            var pipeline = GetPipeline();
            var actual = Markdown.ToHtml(source, pipeline);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("**이 [링크](https://example.kr/)**만을 강조하고 싶다.", "<p><strong>이 <a href=\"https://example.kr/\">링크</a></strong>만을 강조하고 싶다.</p>\n")]
        [TestCase("**스크립트(script)**라고", "<p><strong>스크립트(script)</strong>라고</p>\n")]
        [TestCase("패키지를 발행하려면 **`npm publish`**를 실행하십시오.", "<p>패키지를 발행하려면 <strong><code>npm publish</code></strong>를 실행하십시오.</p>\n")]
        [TestCase("**안녕(hello)**하세요.", "<p><strong>안녕(hello)</strong>하세요.</p>\n")]
        [TestCase("ᅡ**(a)**", "<p>ᅡ<strong>(a)</strong></p>\n")]
        [TestCase("**(k)**ᄏ", "<p><strong>(k)</strong>ᄏ</p>\n")]
        public void TestCjkFriendlyEmphasisKorean(string source, string expected)
        {
            var pipeline = GetPipeline();
            var actual = Markdown.ToHtml(source, pipeline);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("__注意__：注意事項", "<p><strong>注意</strong>：注意事項</p>\n")]
        [TestCase("注意：__注意事項__", "<p>注意：<strong>注意事項</strong></p>\n")]
        [TestCase("正體字。︁_Traditional._", "<p>正體字。︁<em>Traditional.</em></p>\n")]
        [TestCase("正體字。︁__Hong Kong and Taiwan.__", "<p>正體字。︁<strong>Hong Kong and Taiwan.</strong></p>\n")]
        [TestCase("简体字 / 新字体。︀_Simplified._", "<p>简体字 / 新字体。︀<em>Simplified.</em></p>\n")]
        [TestCase("简体字 / 新字体。︀__Mainland China or Japan.__", "<p>简体字 / 新字体。︀<strong>Mainland China or Japan.</strong></p>\n")]
        [TestCase("“︁Git”︁__Hub__", "<p>“︁Git”︁<strong>Hub</strong></p>\n")]
        public void TestCjkFriendlyEmphasisUnderscore(string source, string expected)
        {
            var pipeline = GetPipeline();
            var actual = Markdown.ToHtml(source, pipeline);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("a~~a()~~あ", "<p>a<del>a()</del>あ</p>\n")]
        [TestCase("あ~~()a~~a", "<p>あ<del>()a</del>a</p>\n")]
        [TestCase("𩸽~~()a~~a", "<p>𩸽<del>()a</del>a</p>\n")]
        [TestCase("a~~a()~~𩸽", "<p>a<del>a()</del>𩸽</p>\n")]
        [TestCase("葛󠄀~~()a~~a", "<p>葛󠄀<del>()a</del>a</p>\n")]
        [TestCase("羽︀~~()a~~a", "<p>羽︀<del>()a</del>a</p>\n")]
        [TestCase("a~~「a~~」", "<p>a<del>「a</del>」</p>\n")]
        [TestCase("「~~a」~~a", "<p>「<del>a」</del>a</p>\n")]
        [TestCase("~~a~~：~~a~~", "<p><del>a</del>：<del>a</del></p>\n")]
        [TestCase("~~日本語。︀~~English.", "<p><del>日本語。︀</del>English.</p>\n")]
        [TestCase("~~“︁a”︁~~a", "<p><del>“︁a”︁</del>a</p>\n")]
        public void TestCjkFriendlyEmphasisGfmStrikethrough(string source, string expected)
        {
            var pipeline = GetPipelineWithStrikethrough();
            var actual = Markdown.ToHtml(source, pipeline);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("a**〰**a", "<p>a<strong>〰</strong>a</p>\n")]
        [TestCase("a**〽**a", "<p>a<strong>〽</strong>a</p>\n")]
        [TestCase("a**🈂**a", "<p>a<strong>🈂</strong>a</p>\n")]
        [TestCase("a**🈷**a", "<p>a<strong>🈷</strong>a</p>\n")]
        [TestCase("a**㊗**a", "<p>a<strong>㊗</strong>a</p>\n")]
        [TestCase("a**㊙**a", "<p>a<strong>㊙</strong>a</p>\n")]
        public void TestCjkFriendlyPseudoEmoji(string source, string expected)
        {
            var pipeline = GetPipeline();
            var actual = Markdown.ToHtml(source, pipeline);
            Assert.AreEqual(expected, actual);
        }
    }
}
