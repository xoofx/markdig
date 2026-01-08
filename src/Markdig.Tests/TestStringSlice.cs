// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
#if TEST_NETSTANDARD_BUILD_IN_MODERN_NET
using Markdig.Polyfills.System.Text;
#else
using System.Text;
#endif
using System.Threading.Tasks;

namespace Markdig.Tests;

[TestFixture]
internal class TestStringSlice
{
    // TODO: Add more tests for StringSlice

    // Old versions of modern .NET (that meets NETCOREAPP but not the following #if) try to load the polyfilled Rune in .NET Standard 2.x build of Markdig while they have built-in Rune too
    // Adjust this condition to match the minimal modern (not .NET Standard or Framework) .NET target framework of Markdig (not Markdig.Tests)
#if NET8_0_OR_GREATER

    [Test]
    public void TestRuneBmp()
    {
        var slice = new StringSlice("01234");

        Assert.AreEqual((Rune)'0', slice.CurrentRune);
        Assert.AreEqual((Rune)'1', slice.NextRune());
        Assert.AreEqual((Rune)'2', slice.NextRune());
        Assert.AreEqual((Rune)'2', slice.CurrentRune);
        Assert.AreEqual("234", slice.ToString());
        Assert.AreEqual((Rune)'3', slice.PeekRuneExtra(1));
        Assert.AreEqual((Rune)'4', slice.PeekRuneExtra(2));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(3));
        Assert.AreEqual((Rune)'1', slice.PeekRuneExtra(-1));
        Assert.AreEqual((Rune)'0', slice.PeekRuneExtra(-2));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(-3));
        Assert.AreEqual((Rune)'0', slice.RuneAt(0));
        Assert.AreEqual((Rune)'1', slice.RuneAt(1));
        Assert.AreEqual((Rune)'2', slice.RuneAt(2));
        Assert.AreEqual((Rune)'3', slice.RuneAt(3));
        Assert.AreEqual((Rune)'4', slice.RuneAt(4));
    }

    [Test]
    public void TestRuneSupplementaryOnly()
    {
        var slice = new StringSlice("𝟎𝟏𝟐𝟑𝟒");

        // 𝟎 = U+1D7CE, 𝟐 = U+1D7D0
        Assert.AreEqual((Rune)0x1D7CE, slice.CurrentRune); // 𝟎
        Assert.AreEqual((Rune)0x1D7CF, slice.NextRune()); // 𝟏
        Assert.AreEqual((Rune)0x1D7D0, slice.NextRune()); // 𝟐
        Assert.AreEqual((Rune)0x1D7D0, slice.CurrentRune); // 𝟐
        Assert.AreEqual("𝟐𝟑𝟒", slice.ToString());
        // CurrentRune occupies 2 `char`s, so next Rune starts at index 2
        Assert.AreEqual((Rune)0x1D7D1, slice.PeekRuneExtra(2)); // 𝟑
        Assert.AreEqual((Rune)0x1D7D2, slice.PeekRuneExtra(4)); // 𝟒
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(6));
        Assert.AreEqual((Rune)0x1D7CF, slice.PeekRuneExtra(-1)); // 𝟏
        Assert.AreEqual((Rune)0x1D7CE, slice.PeekRuneExtra(-3)); // 𝟎
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(-5));
        Assert.AreEqual((Rune)0x1D7CE, slice.RuneAt(0)); // 𝟎
        Assert.AreEqual((Rune)0x1D7CF, slice.RuneAt(2)); // 𝟏
        Assert.AreEqual((Rune)0x1D7D0, slice.RuneAt(4)); // 𝟐
        Assert.AreEqual((Rune)0x1D7D1, slice.RuneAt(6)); // 𝟑
        Assert.AreEqual((Rune)0x1D7D2, slice.RuneAt(8)); // 𝟒
        // The following usages are not expected. You should take into consideration the `char`s that the Rune you just acquired occupies.
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(-4));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(-2));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(1));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(3));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(5));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(1));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(3));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(5));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(7));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(9));
    }

    [Test]
    public void TestRuneIsolatedHighSurrogate()
    {
        var slice = new StringSlice("\ud800\ud801\ud802\ud803\ud804");
        Assert.AreEqual((Rune)'\0', slice.CurrentRune);
        Assert.AreEqual((Rune)'\0', slice.NextRune());
        Assert.AreEqual('\ud801', slice.CurrentChar);
        Assert.AreEqual((Rune)'\0', slice.NextRune());
        Assert.AreEqual('\ud802', slice.CurrentChar);
        Assert.AreEqual((Rune)'\0', slice.CurrentRune);
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(-3));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(-2));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(-1));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(1));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(2));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(3));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(0));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(1));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(2));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(3));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(4));
    }

    [Test]
    public void TestRuneIsolatedLowSurrogate()
    {
        var slice = new StringSlice("\udc00\udc01\udc02\udc03\udc04");
        Assert.AreEqual((Rune)'\0', slice.CurrentRune);
        Assert.AreEqual((Rune)'\0', slice.NextRune());
        Assert.AreEqual('\udc01', slice.CurrentChar);
        Assert.AreEqual((Rune)'\0', slice.NextRune());
        Assert.AreEqual('\udc02', slice.CurrentChar);
        Assert.AreEqual((Rune)'\0', slice.CurrentRune);
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(-3));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(-2));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(-1));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(1));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(2));
        Assert.AreEqual((Rune)'\0', slice.PeekRuneExtra(3));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(0));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(1));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(2));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(3));
        Assert.AreEqual((Rune)'\0', slice.RuneAt(4));
    }
#endif
}
