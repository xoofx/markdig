// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Diagnostics;

namespace Markdig.Helpers;

internal struct LazySubstring
{
    private string _text;
    public int Offset;
    public int Length;

    public LazySubstring(string text)
    {
        _text = text;
        Offset = 0;
        Length = text.Length;
    }

    public LazySubstring(string text, int offset, int length)
    {
        Debug.Assert((ulong)offset + (ulong)length <= (ulong)text.Length, $"{offset}-{length} in {text}");
        _text = text;
        Offset = offset;
        Length = length;
    }

    public ReadOnlySpan<char> AsSpan() => _text.AsSpan(Offset, Length);

    public override string ToString()
    {
        if (Offset != 0 || Length != _text.Length)
        {
            _text = _text.Substring(Offset, Length);
            Offset = 0;
        }

        return _text;
    }
}
