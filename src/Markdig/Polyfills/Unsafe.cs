// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

#if NETSTANDARD2_1

using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices;

internal static class Unsafe
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(o))]
    public static T? As<T>(object? o) where T : class
    {
        return (T?)o;
    }
}
#endif