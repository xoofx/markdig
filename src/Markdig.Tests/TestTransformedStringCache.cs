using Markdig.Helpers;

namespace Markdig.Tests;

public class TestTransformedStringCache
{
    [Test]
    public void GetRunsTransformationCallback()
    {
        var cache = new TransformedStringCache(static s => "callback-" + s);

        Assert.AreEqual("callback-foo", cache.Get("foo"));
        Assert.AreEqual("callback-bar", cache.Get("bar"));
        Assert.AreEqual("callback-baz", cache.Get("baz"));
    }

    [Test]
    public void CachesTransformedInstance()
    {
        var cache = new TransformedStringCache(static s => "callback-" + s);

        string transformedBar = cache.Get("bar");
        Assert.AreSame(transformedBar, cache.Get("bar"));

        string transformedFoo = cache.Get("foo".AsSpan());
        Assert.AreSame(transformedFoo, cache.Get("foo"));

        Assert.AreSame(cache.Get("baz"), cache.Get("baz".AsSpan()));

        Assert.AreSame(transformedBar, cache.Get("bar"));
        Assert.AreSame(transformedFoo, cache.Get("foo"));
        Assert.AreSame(transformedBar, cache.Get("bar".AsSpan()));
        Assert.AreSame(transformedFoo, cache.Get("foo".AsSpan()));
    }

    [Test]
    public void DoesNotCacheEmptyInputs()
    {
        var cache = new TransformedStringCache(static s => new string('a', 4));

        string cached = cache.Get("");
        string cached2 = cache.Get("");
        string cached3 = cache.Get(ReadOnlySpan<char>.Empty);

        Assert.AreEqual("aaaa", cached);
        Assert.AreEqual(cached, cached2);
        Assert.AreEqual(cached, cached3);

        Assert.AreNotSame(cached, cached2);
        Assert.AreNotSame(cached, cached3);
        Assert.AreNotSame(cached2, cached3);
    }

    [Test]
    [TestCase(TransformedStringCache.InputLengthLimit, true)]
    [TestCase(TransformedStringCache.InputLengthLimit + 1, false)]
    public void DoesNotCacheLongInputs(int length, bool shouldBeCached)
    {
        var cache = new TransformedStringCache(static s => "callback-" + s);

        string input = new string('a', length);

        string cached = cache.Get(input);
        string cached2 = cache.Get(input);

        Assert.AreEqual("callback-" + input, cached);
        Assert.AreEqual(cached, cached2);

        if (shouldBeCached)
        {
            Assert.AreSame(cached, cached2);
        }
        else
        {
            Assert.AreNotSame(cached, cached2);
        }
    }

    [Test]
    public void CachesAtMostNEntriesPerCharacter()
    {
        var cache = new TransformedStringCache(static s => "callback-" + s);

        int limit = TransformedStringCache.MaxEntriesPerCharacter;

        string[] a = Enumerable.Range(1, limit + 1).Select(i => $"a{i}").ToArray();
        string[] cachedAs = a.Select(a => cache.Get(a)).ToArray();

        for (int i = 0; i < limit; i++)
        {
            Assert.AreSame(cachedAs[i], cache.Get(a[i]));
        }

        Assert.AreNotSame(cachedAs[limit], cache.Get(a[limit]));

        Assert.AreSame(cache.Get("b1"), cache.Get("b1"));
    }
}
