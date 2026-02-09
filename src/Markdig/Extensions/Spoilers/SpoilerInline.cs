// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Spoilers;

/// <summary>
///     An inline spoiler.
/// </summary>
/// <seealso cref="SpoilerExtension"/>
public sealed class SpoilerInline : LeafInline
{
    public StringSlice Content { get; }

    internal SpoilerInline(StringSlice content) => Content = content;
}