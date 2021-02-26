// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Extensions.ReferralLinks;
using Markdig.Renderers;
using System;

namespace Markdig.Extensions.NoRefLinks
{
    /// <summary>
    /// Extension to automatically render rel=nofollow to all links in an HTML output.
    /// </summary>
    [Obsolete("Use ReferralLinksExtension class instead")]
    public class NoFollowLinksExtension : IMarkdownExtension
    {
        private ReferralLinksExtension _referralLinksExtension;

        public NoFollowLinksExtension()
        {
            _referralLinksExtension = new ReferralLinksExtension(new[] { "nofollow" });
        }
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            _referralLinksExtension.Setup(pipeline);
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            _referralLinksExtension.Setup(pipeline, renderer);
        }
    }
}
