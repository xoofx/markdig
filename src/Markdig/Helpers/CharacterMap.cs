// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Markdig.Helpers;

/// <summary>
/// Allows to associate characters to a data structures and query efficiently for them.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class CharacterMap<T> where T : class
{
    private readonly SearchValues<char> _values;
    private readonly T[] _asciiMap;
    private readonly FrozenDictionary<uint, T>? _nonAsciiMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterMap{T}"/> class.
    /// </summary>
    /// <param name="maps">The states.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CharacterMap(IEnumerable<KeyValuePair<char, T>> maps)
    {
        if (maps is null) ThrowHelper.ArgumentNullException(nameof(maps));

        var charSet = new HashSet<char>();

        foreach (var map in maps)
        {
            charSet.Add(map.Key);
        }

        OpeningCharacters = [.. charSet];
        Array.Sort(OpeningCharacters);

        _asciiMap = new T[128];
        Dictionary<uint, T>? nonAsciiMap = null;

        foreach (var state in maps)
        {
            char openingChar = state.Key;
            if (openingChar < 128)
            {
                _asciiMap[openingChar] ??= state.Value;
            }
            else
            {
                nonAsciiMap ??= [];

                if (!nonAsciiMap.ContainsKey(openingChar))
                {
                    nonAsciiMap[openingChar] = state.Value;
                }
            }
        }

        _values = SearchValues.Create(OpeningCharacters);

        if (nonAsciiMap is not null)
        {
            _nonAsciiMap = nonAsciiMap.ToFrozenDictionary();
        }
    }

    /// <summary>
    /// Gets all the opening characters defined.
    /// </summary>
    public char[] OpeningCharacters { get; }

    /// <summary>
    /// Gets the list of parsers valid for the specified opening character.
    /// </summary>
    /// <param name="openingChar">The opening character.</param>
    /// <returns>A list of parsers valid for the specified opening character or null if no parsers registered.</returns>
    public T? this[uint openingChar]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            T[] asciiMap = _asciiMap;
            if (openingChar < (uint)asciiMap.Length)
            {
                return asciiMap[openingChar];
            }
            else
            {
                T? map = null;
                _nonAsciiMap?.TryGetValue(openingChar, out map);
                return map;
            }
        }
    }

    /// <summary>
    /// Searches for an opening character from a registered parser in the specified string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns>Index position within the string of the first opening character found in the specified text; if not found, returns -1</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexOfOpeningCharacter(string text, int start, int end)
    {
        Debug.Assert(text is not null);

        ReadOnlySpan<char> span = text.AsSpan(start, end - start + 1);

        int index = span.IndexOfAny(_values);

        if (index >= 0)
        {
            index += start;
        }

        return index;
    }
}
