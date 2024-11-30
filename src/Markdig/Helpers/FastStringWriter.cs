// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Markdig.Helpers;

internal sealed class FastStringWriter : TextWriter
{
    public override Encoding Encoding => Encoding.Unicode;

    private char[] _chars;
    private int _pos;
    private string _newLine;

    public FastStringWriter()
    {
        _chars = new char[1024];
        _newLine = "\n";
    }

    [AllowNull]
    public override string NewLine
    {
        get => _newLine;
        set => base.NewLine = _newLine = value ?? Environment.NewLine;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write(char value)
    {
        char[] chars = _chars;
        int pos = _pos;
        if ((uint)pos < (uint)chars.Length)
        {
            chars[pos] = value;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteLine(char value)
    {
        Write(value);
        WriteLine();
    }

    public override Task WriteAsync(char value)
    {
        Write(value);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(char value)
    {
        WriteLine(value);
        return Task.CompletedTask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write(string? value)
    {
        if (value is not null)
        {
            if (_pos > _chars.Length - value.Length)
            {
                Grow(value.Length);
            }

            value.AsSpan().CopyTo(_chars.AsSpan(_pos));
            _pos += value.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteLine(string? value)
    {
        Write(value);
        WriteLine();
    }

    public override Task WriteAsync(string? value)
    {
        Write(value);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(string? value)
    {
        WriteLine(value);
        return Task.CompletedTask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write(char[]? buffer)
    {
        if (buffer is not null)
        {
            if (_pos > _chars.Length - buffer.Length)
            {
                Grow(buffer.Length);
            }

            buffer.CopyTo(_chars.AsSpan(_pos));
            _pos += buffer.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteLine(char[]? buffer)
    {
        Write(buffer);
        WriteLine();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write(char[] buffer, int index, int count)
    {
        if (buffer is not null)
        {
            if (_pos > _chars.Length - count)
            {
                Grow(buffer.Length);
            }

            buffer.AsSpan(index, count).CopyTo(_chars.AsSpan(_pos));
            _pos += count;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteLine(char[] buffer, int index, int count)
    {
        Write(buffer, index, count);
        WriteLine();
    }

    public override Task WriteAsync(char[] buffer, int index, int count)
    {
        Write(buffer, index, count);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(char[] buffer, int index, int count)
    {
        WriteLine(buffer, index, count);
        return Task.CompletedTask;
    }

#if !(NETFRAMEWORK || NETSTANDARD2_0)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write(ReadOnlySpan<char> value)
    {
        if (_pos > _chars.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_chars.AsSpan(_pos));
        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteLine(ReadOnlySpan<char> buffer)
    {
        Write(buffer);
        WriteLine();
    }

    public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        Write(buffer.Span);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
    {
        WriteLine(buffer.Span);
        return Task.CompletedTask;
    }
#endif

#if !(NETFRAMEWORK || NETSTANDARD2_0 || NETSTANDARD2_1)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write(StringBuilder? value)
    {
        if (value is not null)
        {
            int length = value.Length;
            if (_pos > _chars.Length - length)
            {
                Grow(length);
            }

            value.CopyTo(0, _chars.AsSpan(_pos), length);
            _pos += length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteLine(StringBuilder? value)
    {
        Write(value);
        WriteLine();
    }

    public override Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
    {
        Write(value);
        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
    {
        WriteLine(value);
        return Task.CompletedTask;
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteLine()
    {
        foreach (char c in _newLine)
        {
            Write(c);
        }
    }

    public override Task WriteLineAsync()
    {
        WriteLine();
        return Task.CompletedTask;
    }


    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char value)
    {
        Grow(1);
        Write(value);
    }

    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "No resize is needed.");

        char[] newArray = new char[(int)Math.Max((uint)(_pos + additionalCapacityBeyondPos), (uint)_chars.Length * 2)];
        _chars.AsSpan(0, _pos).CopyTo(newArray);
        _chars = newArray;
    }


    public override void Flush() { }

    public override void Close() { }

    public override Task FlushAsync() => Task.CompletedTask;

#if !(NETFRAMEWORK || NETSTANDARD2_0)
    public override ValueTask DisposeAsync() => default;
#endif


    public void Reset()
    {
        _pos = 0;
    }

    public override string ToString() => AsSpan().ToString();

    public ReadOnlySpan<char> AsSpan() => _chars.AsSpan(0, _pos);
}
