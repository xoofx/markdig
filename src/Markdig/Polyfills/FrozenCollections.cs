// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

#if !NET8_0_OR_GREATER

namespace System.Collections.Frozen;

// We're using a polyfill instead of conditionally referencing the package as the package is untested on older TFMs, and
// brings in a reference to System.Runtime.CompilerServices.Unsafe, which conflicts with our polyfills of that type.

internal sealed class FrozenDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    public FrozenDictionary(Dictionary<TKey, TValue> dictionary) : base(dictionary) { }
}

internal static class FrozenDictionaryExtensions
{
    public static FrozenDictionary<TKey, TValue> ToFrozenDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        return new FrozenDictionary<TKey, TValue>(dictionary);
    }
}

internal sealed class FrozenSet<T> : HashSet<T>
{
    public FrozenSet(HashSet<T> set, IEqualityComparer<T> comparer) : base(set, comparer) { }
}

internal static class FrozenSetExtensions
{
    public static FrozenSet<T> ToFrozenSet<T>(this HashSet<T> set, IEqualityComparer<T> comparer)
    {
        return new FrozenSet<T>(set, comparer);
    }
}

#endif