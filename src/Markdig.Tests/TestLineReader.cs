// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.IO;
using NUnit.Framework;
using Markdig.Helpers;

namespace Markdig.Tests
{
    /// <summary>
    /// Test for <see cref="LineReader"/>.
    /// </summary>
    [TestFixture]
    public class TestLineReader
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestEmpty(bool limited)
        {
            var lineReader = new LineReader(new StringReader(""), true, limited ? 4 : LineReader.DefaultBufferSize);
            Assert.Null(lineReader.ReadLine());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestLinesOnlyLf(bool limited)
        {
            var lineReader = new LineReader(new StringReader("\n\n\n"), true, limited ? 2 : LineReader.DefaultBufferSize);
            Assert.AreEqual(string.Empty, lineReader.ReadLine());
            Assert.AreEqual(1, lineReader.SourcePosition);
            Assert.AreEqual(string.Empty, lineReader.ReadLine());
            Assert.AreEqual(2, lineReader.SourcePosition);
            Assert.AreEqual(string.Empty, lineReader.ReadLine());
            Assert.Null(lineReader.ReadLine());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestLinesOnlyCr(bool limited)
        {
            var lineReader = new LineReader(new StringReader("\r\r\r"), true, limited ? 2 : LineReader.DefaultBufferSize);
            Assert.AreEqual(string.Empty, lineReader.ReadLine());
            Assert.AreEqual(1, lineReader.SourcePosition);
            Assert.AreEqual(string.Empty, lineReader.ReadLine());
            Assert.AreEqual(2, lineReader.SourcePosition);
            Assert.AreEqual(string.Empty, lineReader.ReadLine());
            Assert.Null(lineReader.ReadLine());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestLinesOnlyCrLf(bool limited)
        {
            var lineReader = new LineReader(new StringReader("\r\n\r\n\r\n"), true, limited ? 3 : LineReader.DefaultBufferSize);
            Assert.AreEqual(string.Empty, lineReader.ReadLine());
            Assert.AreEqual(2, lineReader.SourcePosition);
            Assert.AreEqual(string.Empty, lineReader.ReadLine());
            Assert.AreEqual(4, lineReader.SourcePosition);
            Assert.AreEqual(string.Empty, lineReader.ReadLine());
            Assert.Null(lineReader.ReadLine());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestNoEndOfLine(bool limited)
        {
            var lineReader = new LineReader(new StringReader("123"), true);
            Assert.AreEqual("123", lineReader.ReadLine());
            Assert.Null(lineReader.ReadLine());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestLf(bool limited)
        {
            var lineReader = new LineReader(new StringReader("123\n"), true, limited ? 4 : LineReader.DefaultBufferSize);
            Assert.AreEqual("123", lineReader.ReadLine());
            Assert.AreEqual(4, lineReader.SourcePosition);
            Assert.Null(lineReader.ReadLine());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestLf2(bool limited)
        {
            // When limited == true, we limit the internal buffer exactly after the first new line char '\n'
            var lineReader = new LineReader(new StringReader("123\n456"), true, limited ? 4 : LineReader.DefaultBufferSize);
            Assert.AreEqual("123", lineReader.ReadLine());
            Assert.AreEqual(4, lineReader.SourcePosition);
            Assert.AreEqual("456", lineReader.ReadLine());
            Assert.Null(lineReader.ReadLine());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestCr(bool limited)
        {
            var lineReader = new LineReader(new StringReader("123\r"), true, limited ? 4 : LineReader.DefaultBufferSize);
            Assert.AreEqual("123", lineReader.ReadLine());
            Assert.AreEqual(4, lineReader.SourcePosition);
            Assert.Null(lineReader.ReadLine());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestCr2(bool limited)
        {
            var lineReader = new LineReader(new StringReader("123\r456"), true, limited ? 4 : LineReader.DefaultBufferSize);
            Assert.AreEqual("123", lineReader.ReadLine());
            Assert.AreEqual(4, lineReader.SourcePosition);
            Assert.AreEqual("456", lineReader.ReadLine());
            Assert.Null(lineReader.ReadLine());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestCrLf(bool limited)
        {
            // When limited == true, we limit the internal buffer exactly after the first new line char '\r'
            // and we check that we don't get a new line for `\n` 
            var lineReader = new LineReader(new StringReader("123\r\n"), true, limited ? 4 : LineReader.DefaultBufferSize);
            Assert.AreEqual("123", lineReader.ReadLine());
            Assert.AreEqual(5, lineReader.SourcePosition);
            Assert.Null(lineReader.ReadLine());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestCrLf2(bool limited)
        {
            var lineReader = new LineReader(new StringReader("123\r\n456"), true, limited ? 4 : LineReader.DefaultBufferSize);
            Assert.AreEqual("123", lineReader.ReadLine());
            Assert.AreEqual(5, lineReader.SourcePosition);
            Assert.AreEqual("456", lineReader.ReadLine());
            Assert.Null(lineReader.ReadLine());
        }
    }
}