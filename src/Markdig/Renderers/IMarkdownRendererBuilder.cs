// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.IO;

namespace Markdig.Renderers;

public interface IMarkdownRendererBuilder {
    TextRendererBase Build(TextWriter writer);
}
