// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using NUnit.Framework;

namespace Markdig.Tests
{
    [TestFixture]
    public partial class TestEmphasisPlus
    {
        [Test]
        public void StrongNormal()
        {
            TestParser.TestSpec("***Strong emphasis*** normal", "<p><em><strong>Strong emphasis</strong></em> normal</p>", "");
        }

        [Test]
        public void NormalStrongNormal()
        {
            TestParser.TestSpec("normal ***Strong emphasis*** normal", "<p>normal <em><strong>Strong emphasis</strong></em> normal</p>", "");
        }
    }
}