// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

#if NET462 || NETSTANDARD2_0

using System.Diagnostics;

namespace System;

internal static class SpanExtensions
{
    public static bool StartsWith(this ReadOnlySpan<char> span, string prefix, StringComparison comparisonType)
    {
        Debug.Assert(comparisonType is StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase);

        return
            span.Length >= prefix.Length &&
            span.Slice(0, prefix.Length).Equals(prefix.AsSpan(), comparisonType);
    }
}

#endif