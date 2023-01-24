// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Linq;
using System.Threading;

namespace Markdig.Helpers;

internal sealed class TransformedStringCache
{
    internal const int InputLengthLimit = 20; // Avoid caching unreasonably long strings
    internal const int MaxEntriesPerCharacter = 8; // Avoid growing too much

    private readonly EntryGroup[] _groups; // One per ASCII character
    private readonly Func<string, string> _transformation;

    public TransformedStringCache(Func<string, string> transformation)
    {
        _transformation = transformation ?? throw new ArgumentNullException(nameof(transformation));
        _groups = new EntryGroup[128];
    }

    public string Get(ReadOnlySpan<char> inputSpan)
    {
        if ((uint)(inputSpan.Length - 1) < InputLengthLimit) // Length: [1, LengthLimit]
        {
            int firstCharacter = inputSpan[0];
            EntryGroup[] groups = _groups;
            if ((uint)firstCharacter < (uint)groups.Length)
            {
                ref EntryGroup group = ref groups[firstCharacter];
                string? transformed = group.TryGet(inputSpan);
                if (transformed is null)
                {
                    string input = inputSpan.ToString();
                    transformed = _transformation(input);
                    group.TryAdd(input, transformed);
                }
                return transformed;
            }
        }

        return _transformation(inputSpan.ToString());
    }

    public string Get(string input)
    {
        if ((uint)(input.Length - 1) < InputLengthLimit) // Length: [1, LengthLimit]
        {
            int firstCharacter = input[0];
            EntryGroup[] groups = _groups;
            if ((uint)firstCharacter < (uint)groups.Length)
            {
                ref EntryGroup group = ref groups[firstCharacter];
                string? transformed = group.TryGet(input.AsSpan());
                if (transformed is null)
                {
                    transformed = _transformation(input);
                    group.TryAdd(input, transformed);
                }
                return transformed;
            }
        }

        return _transformation(input);
    }

    private struct EntryGroup
    {
        private struct Entry
        {
            public string Input;
            public string Transformed;
        }

        private Entry[]? _entries;

        public string? TryGet(ReadOnlySpan<char> inputSpan)
        {
            Entry[]? entries = _entries;
            if (entries is not null)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    if (inputSpan.SequenceEqual(entries[i].Input.AsSpan()))
                    {
                        return entries[i].Transformed;
                    }
                }
            }
            return null;
        }

        public void TryAdd(string input, string transformed)
        {
            if (_entries is null)
            {
                Interlocked.CompareExchange(ref _entries, new Entry[MaxEntriesPerCharacter], null);
            }

            if (_entries[MaxEntriesPerCharacter - 1].Input is null) // There is still space
            {
                lock (_entries)
                {
                    for (int i = 0; i < _entries.Length; i++)
                    {
                        string? existingInput = _entries[i].Input;

                        if (existingInput is null)
                        {
                            ref Entry entry = ref _entries[i];
                            Volatile.Write(ref entry.Transformed, transformed);
                            Volatile.Write(ref entry.Input, input);
                            break;
                        }

                        if (input == existingInput)
                        {
                            // We lost a race and a different thread already added the same value
                            break;
                        }
                    }
                }
            }
        }
    }
}
