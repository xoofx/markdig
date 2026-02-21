// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Extensions.ReferralLinks;
using Markdig.Renderers;

namespace Markdig.Extensions.NoRefLinks;

/// <summary>
/// Extension to automatically render rel=nofollow to all links in an HTML output.
/// </summary>
[Obsolete("Use ReferralLinksExtension class instead")]
public class NoFollowLinksExtension : IMarkdownExtension
{
    private ReferralLinksExtension _referralLinksExtension;

    /// <summary>
    /// Initializes a new instance of the NoFollowLinksExtension class.
    /// </summary>
    public NoFollowLinksExtension()
    {
        _referralLinksExtension = new ReferralLinksExtension(["nofollow"]);
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        _referralLinksExtension.Setup(pipeline);
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        _referralLinksExtension.Setup(pipeline, renderer);
    }
}
