// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace System.Runtime.CompilerServices;

#if NETSTANDARD2_1
internal static class Unsafe
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T As<T>(object o) where T : class
    {
        return (T)o;
    }
}
#endif