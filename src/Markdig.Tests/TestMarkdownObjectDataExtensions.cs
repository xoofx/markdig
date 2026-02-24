using Markdig.Syntax;

namespace Markdig.Tests;

[TestFixture]
public sealed class TestMarkdownObjectDataExtensions
{
    [Test]
    public void CanSetAndGetTypedDataUsingTypeKey()
    {
        var block = new ParagraphBlock();

        block.SetData(42);

        Assert.That(block.GetData<int>(), Is.EqualTo(42));
        Assert.That(block.GetData<string>(), Is.Null);
    }

    [Test]
    public void CanUseTypedDataKey()
    {
        var block = new ParagraphBlock();
        var key = new DataKey<string>();

        block.SetData<string>(key, "value");

        Assert.That(block.GetData<string>(key), Is.EqualTo("value"));
        Assert.That(block.TryGetData<string>(key, out var output), Is.True);
        Assert.That(output, Is.EqualTo("value"));
    }

    [Test]
    public void TryGetDataReturnsFalseForTypeMismatch()
    {
        var block = new ParagraphBlock();
        var key = new object();

        block.SetData(key, 123);

        Assert.That(block.TryGetData<string>(key, out var output), Is.False);
        Assert.That(output, Is.Null);
    }
}
