using Markdig.Helpers;
using System.Text;

namespace Markdig.Tests;

[TestFixture]
public class TestStringLineGroup
{
    private static string ToString(ICharIterator text)
    {
        var chars = new StringBuilder();
        while (text.CurrentChar != '\0')
        {
            chars.Append(text.CurrentChar);
            text.NextChar();
        }
        return chars.ToString();
    }

    [Test]
    public void TestStringLineGroupCharIteratorAtCapacity()
    {
        string str = "ABCDEFGHI";
        var text = new StringLineGroup(1)
        {
            // Will store the following line at capacity
            new StringSlice(str, NewLine.CarriageReturnLineFeed) { Start = 0, End = 2 },
        };

        var iterator = text.ToCharIterator();
        var chars = ToString(iterator);
        TextAssert.AreEqual("ABC\r\n", chars.ToString());
        TextAssert.AreEqual("ABC", text.ToString());
    }

    [Test]
    public void TestStringLineGroupCharIteratorForcingIncreaseCapacity()
    {
        string str = "ABCDEFGHI";
        var text = new StringLineGroup(1)
        {
            // Will store the following line at capacity
            new StringSlice(str, NewLine.CarriageReturnLineFeed) { Start = 0, End = 2 },

            // Will force increase capacity to 2 and store the line at capacity
            new StringSlice(str, NewLine.CarriageReturnLineFeed) { Start = 3, End = 3 },
        };

        var iterator = text.ToCharIterator();
        var chars = ToString(iterator);
        TextAssert.AreEqual("ABC\r\nD\r\n", chars.ToString());
        TextAssert.AreEqual("ABC\r\nD", text.ToString());
    }

}
