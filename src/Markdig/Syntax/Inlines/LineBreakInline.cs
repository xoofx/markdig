// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Syntax.Inlines;

/// <summary>
/// A base class for a line break.
/// </summary>
/// <seealso cref="LeafInline" />
public class LineBreakInline : LeafInline
{
    public bool IsHard { get; set; }

    public bool IsBackslash { get; set; }

    public NewLine NewLine { get; set; }
}