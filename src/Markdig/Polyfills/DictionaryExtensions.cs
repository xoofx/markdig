// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

#if !(NETSTANDARD2_1_OR_GREATER || NET)

namespace System.Collections.Generic;

internal static class DictionaryExtensions
{
    public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
            return true;
        }

        return false;
    }
}

#endif
