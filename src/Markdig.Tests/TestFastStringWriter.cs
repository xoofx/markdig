using System.Text;

using Markdig.Helpers;

namespace Markdig.Tests;

[TestFixture]
public class TestFastStringWriter
{
    private const string NewLineReplacement = "~~NEW_LINE~~";

    private FastStringWriter _writer = new();

    [SetUp]
    public void Setup()
    {
        _writer = new FastStringWriter
        {
            NewLine = NewLineReplacement
        };
    }

    public void AssertToString(string value)
    {
        value = value.Replace("\n", NewLineReplacement);
        Assert.AreEqual(value, _writer.ToString());
        Assert.AreEqual(value, _writer.ToString());
    }

    [Test]
    public async Task NewLine()
    {
        Assert.AreEqual("\n", new FastStringWriter().NewLine);

        _writer.NewLine = "\r";
        Assert.AreEqual("\r", _writer.NewLine);

        _writer.NewLine = "foo";
        Assert.AreEqual("foo", _writer.NewLine);

        _writer.WriteLine();
        await _writer.WriteLineAsync();
        _writer.WriteLine("bar");
        Assert.AreEqual("foofoobarfoo", _writer.ToString());
    }

    [Test]
    public async Task FlushCloseDispose()
    {
        _writer.Write('a');

        // Nops
        _writer.Close();
        _writer.Dispose();
        await _writer.DisposeAsync();
        _writer.Flush();
        await _writer.FlushAsync();

        _writer.Write('b');
        AssertToString("ab");
    }

    [Test]
    public async Task Write_Char()
    {
        _writer.Write('a');
        AssertToString("a");

        _writer.Write('b');
        AssertToString("ab");

        _writer.Write('\0');
        _writer.Write('\r');
        _writer.Write('\u1234');
        AssertToString("ab\0\r\u1234");

        _writer.Reset();
        AssertToString("");

        _writer.Write('a');
        _writer.WriteLine('b');
        _writer.Write('c');
        _writer.Write('d');
        _writer.WriteLine('e');
        AssertToString("ab\ncde\n");

        await _writer.WriteAsync('f');
        await _writer.WriteLineAsync('g');
        AssertToString("ab\ncde\nfg\n");

        _writer.Reset();

        for (int i = 0; i < 2050; i++)
        {
            _writer.Write('a');
            AssertToString(new string('a', i + 1));
        }
    }

    [Test]
    public async Task Write_String()
    {
        _writer.Write("foo");
        AssertToString("foo");

        _writer.WriteLine("bar");
        AssertToString("foobar\n");

        await _writer.WriteAsync("baz");
        await _writer.WriteLineAsync("foo");
        AssertToString("foobar\nbazfoo\n");

        _writer.Write(new string('a', 1050));
        AssertToString("foobar\nbazfoo\n" + new string('a', 1050));
    }

    [Test]
    public async Task Write_Span()
    {
        _writer.Write("foo".AsSpan());
        AssertToString("foo");

        _writer.WriteLine("bar".AsSpan());
        AssertToString("foobar\n");

        await _writer.WriteAsync("baz".AsMemory());
        await _writer.WriteLineAsync("foo".AsMemory());
        AssertToString("foobar\nbazfoo\n");

        _writer.Write(new string('a', 1050).AsSpan());
        AssertToString("foobar\nbazfoo\n" + new string('a', 1050));
    }

    [Test]
    public async Task Write_CharArray()
    {
        _writer.Write("foo".ToCharArray());
        AssertToString("foo");

        _writer.WriteLine("bar".ToCharArray());
        AssertToString("foobar\n");

        await _writer.WriteAsync("baz".ToCharArray());
        await _writer.WriteLineAsync("foo".ToCharArray());
        AssertToString("foobar\nbazfoo\n");

        _writer.Write(new string('a', 1050).ToCharArray());
        AssertToString("foobar\nbazfoo\n" + new string('a', 1050));
    }

    [Test]
    public async Task Write_CharArrayWithIndexes()
    {
        _writer.Write("foo".ToCharArray(), 1, 1);
        AssertToString("o");

        _writer.WriteLine("bar".ToCharArray(), 0, 2);
        AssertToString("oba\n");

        await _writer.WriteAsync("baz".ToCharArray(), 0, 1);
        await _writer.WriteLineAsync("foo".ToCharArray(), 0, 3);
        AssertToString("oba\nbfoo\n");

        _writer.Write(new string('a', 1050).ToCharArray(), 10, 1035);
        AssertToString("oba\nbfoo\n" + new string('a', 1035));
    }

    [Test]
    public async Task Write_StringBuilder()
    {
        _writer.Write(new StringBuilder("foo"));
        AssertToString("foo");

        _writer.WriteLine(new StringBuilder("bar"));
        AssertToString("foobar\n");

        await _writer.WriteAsync(new StringBuilder("baz"));
        await _writer.WriteLineAsync(new StringBuilder("foo"));
        AssertToString("foobar\nbazfoo\n");

        var sb = new StringBuilder("foo");
        sb.Append('a', 1050);
        _writer.Write(sb);
        AssertToString("foobar\nbazfoo\nfoo" + new string('a', 1050));
    }
}
