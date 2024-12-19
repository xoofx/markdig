// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.IO;

namespace Markdig.Renderers;

/// <summary>
/// Defines a common interface for all rendererer builders.
/// </summary>
public interface IMarkdownRendererBuilder {
    /// <summary>
    /// Creates a new instance of a renderer for use in a pipeline.
    /// </summary>
    /// <remarks>
    /// The renderer needs to be a subclass of <see cref="TextRendererBase"/>
    /// because it is intended to be used by <see cref="MarkdownPipeline.HtmlRendererCache"/> which depends on
    /// <see cref="TextRendererBase.ResetInternal"/> for re-use.
    /// </remarks>
    TextRendererBase Build(TextWriter writer);
}
