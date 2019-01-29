// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;

namespace Markdig.Tests
{
    /// <summary>
    /// Pretty text assert from https://gist.github.com/Haacked/1610603
    /// Modified version to only print +-10 characters around the first diff
    /// </summary>
    public static class TextAssert
    {
        public enum DiffStyle
        {
            Full,
            Minimal
        }

        public static void AreEqual(string expectedValue, string actualValue)
        {
            AreEqual(expectedValue, actualValue, DiffStyle.Full, Console.Out);
        }

        public static void AreEqual(string expectedValue, string actualValue, DiffStyle diffStyle)
        {
            AreEqual(expectedValue, actualValue, diffStyle, Console.Out);
        }

        public static void AreEqual(string expectedValue, string actualValue, DiffStyle diffStyle, TextWriter output)
        {
            if (actualValue == null || expectedValue == null)
            {
                Assert.AreEqual(expectedValue, actualValue);
                return;
            }

            if (actualValue.Equals(expectedValue, StringComparison.Ordinal))
            {
                return;
            }

            Console.WriteLine();
            output.WriteLine("Index    Expected     Actual");
            output.WriteLine("----------------------------");
            int maxLen = Math.Max(actualValue.Length, expectedValue.Length);
            int minLen = Math.Min(actualValue.Length, expectedValue.Length);

            if (diffStyle != DiffStyle.Minimal)
            {
                int startDifferAt = 0;
                for (int i = 0; i < maxLen; i++)
                {
                    if (i >= minLen || actualValue[i] != expectedValue[i])
                    {
                        startDifferAt = i;
                        break;
                    }
                }

                var endDifferAt = Math.Min(startDifferAt + 10, maxLen);
                startDifferAt = Math.Max(startDifferAt - 10, 0);

                bool isFirstDiff = true;
                for (int i = startDifferAt; i < endDifferAt; i++)
                {
                    if (i >= minLen || actualValue[i] != expectedValue[i])
                    {
                        output.WriteLine("{0,-3} {1,-3}    {2,-4} {3,-3}   {4,-4} {5,-3}",
                            i < minLen && actualValue[i] == expectedValue[i] ? " " : isFirstDiff  ? ">>>": "***",
                            // put a mark beside a differing row
                            i, // the index
                            i < expectedValue.Length ? ((int) expectedValue[i]).ToString() : "",
                            // character decimal value
                            i < expectedValue.Length ? expectedValue[i].ToSafeString() : "", // character safe string
                            i < actualValue.Length ? ((int) actualValue[i]).ToString() : "", // character decimal value
                            i < actualValue.Length ? actualValue[i].ToSafeString() : "" // character safe string
                            );

                        isFirstDiff = false;
                    }
                }
                //output.WriteLine();
            }

            Assert.AreEqual(expectedValue, actualValue);
        }

        private static string ToSafeString(this char c)
        {
            if (char.IsControl(c) || char.IsWhiteSpace(c))
            {
                switch (c)
                {
                    case '\b':
                        return @"\b";
                    case '\r':
                        return @"\r";
                    case '\n':
                        return @"\n";
                    case '\t':
                        return @"\t";
                    case '\a':
                        return @"\a";
                    case '\v':
                        return @"\v";
                    case '\f':
                        return @"\f";
                    default:
                        return $"\\u{(int) c:X};";
                }
            }
            return c.ToString(CultureInfo.InvariantCulture);
        }
    }
}