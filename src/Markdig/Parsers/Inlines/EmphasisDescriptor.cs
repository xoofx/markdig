// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using System.Diagnostics;

namespace Markdig.Parsers.Inlines;

/// <summary>
/// Descriptor for an emphasis.
/// </summary>
[DebuggerDisplay("Emphasis Char={Character}, Min={MinimumCount}, Max={MaximumCount}, EnableWithinWord={EnableWithinWord}")]
public sealed class EmphasisDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmphasisDescriptor"/> class.
    /// </summary>
    /// <param name="character">The character used for this emphasis.</param>
    /// <param name="minimumCount">The minimum number of character.</param>
    /// <param name="maximumCount">The maximum number of characters.</param>
    /// <param name="enableWithinWord">if set to <c>true</c> the emphasis can be used inside a word.</param>
    public EmphasisDescriptor(char character, int minimumCount, int maximumCount, bool enableWithinWord)
    {
        if (minimumCount < 1) ThrowHelper.ArgumentOutOfRangeException(nameof(minimumCount), "minimumCount must be >= 1");
        if (maximumCount < 1) ThrowHelper.ArgumentOutOfRangeException(nameof(maximumCount), "maximumCount must be >= 1");
        if (minimumCount > maximumCount) ThrowHelper.ArgumentOutOfRangeException(nameof(minimumCount), "minimumCount must be <= maximumCount");

        Character = character;
        MinimumCount = minimumCount;
        MaximumCount = maximumCount;
        EnableWithinWord = enableWithinWord;
    }

    /// <summary>
    /// The character of this emphasis.
    /// </summary>
    public char Character { get; }

    /// <summary>
    /// The minimum number of character this emphasis is expected to have (must be >=1)
    /// </summary>
    public int MinimumCount { get; }

    /// <summary>
    /// The maximum number of character this emphasis is expected to have (must be >=1 and >= minimumCount)
    /// </summary>
    public int MaximumCount { get; }

    /// <summary>
    /// This emphasis can be used within a word.
    /// </summary>
    public bool EnableWithinWord { get; }
}