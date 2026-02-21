// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Syntax;

/// <summary>
/// A span of text.
/// </summary>
public struct SourceSpan : IEquatable<SourceSpan>
{
    /// <summary>
    /// Gets or sets the empty.
    /// </summary>
    public static readonly SourceSpan Empty = new SourceSpan(0, -1);

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceSpan"/> struct.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    public SourceSpan(int start, int end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets or sets the starting character position from the original text source. 
    /// Note that for inline elements, this is only valid if <see cref="MarkdownExtensions.UsePreciseSourceLocation"/> is setup on the pipeline.
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// Gets or sets the ending character position from the original text source.
    /// Note that for inline elements, this is only valid if <see cref="MarkdownExtensions.UsePreciseSourceLocation"/> is setup on the pipeline.
    /// </summary>
    public int End { get; set; }

    /// <summary>
    /// Gets the character length of this element within the original source code.
    /// </summary>
    public readonly int Length => End - Start + 1;

    /// <summary>
    /// Gets or sets the is empty.
    /// </summary>
    public readonly bool IsEmpty => Start > End;

    /// <summary>
    /// Performs the move forward operation.
    /// </summary>
    public SourceSpan MoveForward(int count)
    {
        return new SourceSpan(Start + count, End + count);
    }

    /// <summary>
    /// Performs the equals operation.
    /// </summary>
    public readonly bool Equals(SourceSpan other)
    {
        return Start == other.Start && End == other.End;
    }

    /// <summary>
    /// Performs the equals operation.
    /// </summary>
    public override readonly bool Equals(object? obj)
    {
        return obj is SourceSpan sourceSpan && Equals(sourceSpan);
    }

    /// <summary>
    /// Gets hash code.
    /// </summary>
    public override readonly int GetHashCode()
    {
        unchecked
        {
            return (Start*397) ^ End;
        }
    }

    /// <summary>
    /// Performs the  operation.
    /// </summary>
    public static bool operator ==(SourceSpan left, SourceSpan right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Performs the  operation.
    /// </summary>
    public static bool operator !=(SourceSpan left, SourceSpan right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Performs the to string operation.
    /// </summary>
    public override readonly string ToString()
    {
        return $"{Start}-{End}";
    }
}
