// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Tests;

/// <summary>
/// Test for <see cref="LineReader"/>.
/// </summary>
[TestFixture]
public class TestLineReader
{
    [Test]
    public void TestEmpty()
    {
        var lineReader = new LineReader("");
        Assert.Null(lineReader.ReadLine().Text);
    }

    [Test]
    public void TestLinesOnlyLf()
    {
        var lineReader = new LineReader("\n\n\n");
        Assert.AreEqual(string.Empty, lineReader.ReadLine().ToString());
        Assert.AreEqual(1, lineReader.SourcePosition);
        Assert.AreEqual(string.Empty, lineReader.ReadLine().ToString());
        Assert.AreEqual(2, lineReader.SourcePosition);
        Assert.AreEqual(string.Empty, lineReader.ReadLine().ToString());
        Assert.Null(lineReader.ReadLine().Text);
    }

    [Test]
    public void TestLinesOnlyCr()
    {
        var lineReader = new LineReader("\r\r\r");
        Assert.AreEqual(string.Empty, lineReader.ReadLine().ToString());
        Assert.AreEqual(1, lineReader.SourcePosition);
        Assert.AreEqual(string.Empty, lineReader.ReadLine().ToString());
        Assert.AreEqual(2, lineReader.SourcePosition);
        Assert.AreEqual(string.Empty, lineReader.ReadLine().ToString());
        Assert.Null(lineReader.ReadLine().Text);
    }

    [Test]
    public void TestLinesOnlyCrLf()
    {
        var lineReader = new LineReader("\r\n\r\n\r\n");
        Assert.AreEqual(string.Empty, lineReader.ReadLine().ToString());
        Assert.AreEqual(2, lineReader.SourcePosition);
        Assert.AreEqual(string.Empty, lineReader.ReadLine().ToString());
        Assert.AreEqual(4, lineReader.SourcePosition);
        Assert.AreEqual(string.Empty, lineReader.ReadLine().ToString());
        Assert.Null(lineReader.ReadLine().Text);
    }

    [Test]
    public void TestNoEndOfLine()
    {
        var lineReader = new LineReader("123");
        Assert.AreEqual("123", lineReader.ReadLine().ToString());
        Assert.Null(lineReader.ReadLine().Text);
    }

    [Test]
    public void TestLf()
    {
        var lineReader = new LineReader("123\n");
        Assert.AreEqual("123", lineReader.ReadLine().ToString());
        Assert.AreEqual(4, lineReader.SourcePosition);
        Assert.Null(lineReader.ReadLine().Text);
    }

    [Test]
    public void TestLf2()
    {
        // When limited == true, we limit the internal buffer exactly after the first new line char '\n'
        var lineReader = new LineReader("123\n456");
        Assert.AreEqual("123", lineReader.ReadLine().ToString());
        Assert.AreEqual(4, lineReader.SourcePosition);
        Assert.AreEqual("456", lineReader.ReadLine().ToString());
        Assert.Null(lineReader.ReadLine().Text);
    }

    [Test]
    public void TestCr()
    {
        var lineReader = new LineReader("123\r");
        Assert.AreEqual("123", lineReader.ReadLine().ToString());
        Assert.AreEqual(4, lineReader.SourcePosition);
        Assert.Null(lineReader.ReadLine().Text);
    }

    [Test]
    public void TestCr2()
    {
        var lineReader = new LineReader("123\r456");
        Assert.AreEqual("123", lineReader.ReadLine().ToString());
        Assert.AreEqual(4, lineReader.SourcePosition);
        Assert.AreEqual("456", lineReader.ReadLine().ToString());
        Assert.Null(lineReader.ReadLine().Text);
    }

    [Test]
    public void TestCrLf()
    {
        // When limited == true, we limit the internal buffer exactly after the first new line char '\r'
        // and we check that we don't get a new line for `\n` 
        var lineReader = new LineReader("123\r\n");
        Assert.AreEqual("123", lineReader.ReadLine().ToString());
        Assert.AreEqual(5, lineReader.SourcePosition);
        Assert.Null(lineReader.ReadLine().Text);
    }

    [Test]
    public void TestCrLf2()
    {
        var lineReader = new LineReader("123\r\n456");
        Assert.AreEqual("123", lineReader.ReadLine().ToString());
        Assert.AreEqual(5, lineReader.SourcePosition);
        Assert.AreEqual("456", lineReader.ReadLine().ToString());
        Assert.Null(lineReader.ReadLine().Text);
    }
}