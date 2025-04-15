// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Extensions.Alerts;

/// <summary>
/// A block representing an alert quote block.
/// </summary>
public class AlertBlock : QuoteBlock
{
    /// <summary>
    /// Creates a new instance of this block.
    /// </summary>
    /// <param name="kind"></param>
    public AlertBlock(StringSlice kind) : base(null)
    {
        Kind = kind;
    }

    /// <summary>
    /// Gets or sets the kind of the alert block (e.g `NOTE`, `TIP`, `IMPORTANT`, `WARNING`, `CAUTION`).
    /// </summary>
    public StringSlice Kind { get; set; }

    /// <summary>
    /// Gets or sets the trivia space after the kind.
    /// </summary>
    public StringSlice TriviaSpaceAfterKind { get; set; }
}
