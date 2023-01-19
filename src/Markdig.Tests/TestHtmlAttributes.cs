// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers.Html;

namespace Markdig.Tests;

[TestFixture()]
public class TestHtmlAttributes
{
    [Test]
    public void TestAddClass()
    {
        var attributes = new HtmlAttributes();
        attributes.AddClass("test");
        Assert.NotNull(attributes.Classes);
        Assert.AreEqual(new List<string>() { "test" }, attributes.Classes);

        attributes.AddClass("test");
        Assert.AreEqual(1, attributes.Classes.Count);

        attributes.AddClass("test1");
        Assert.AreEqual(new List<string>() { "test", "test1" }, attributes.Classes);
    }

    [Test]
    public void TestAddProperty()
    {
        var attributes = new HtmlAttributes();
        attributes.AddProperty("key1", "1");
        Assert.NotNull(attributes.Properties);
        Assert.AreEqual(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("key1", "1") }, attributes.Properties);

        attributes.AddPropertyIfNotExist("key1", "1");
        Assert.NotNull(attributes.Properties);
        Assert.AreEqual(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("key1", "1") }, attributes.Properties);

        attributes.AddPropertyIfNotExist("key2", "2");
        Assert.AreEqual(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("key1", "1"), new KeyValuePair<string, string>("key2", "2") }, attributes.Properties);
    }

    [Test]
    public void TestCopyTo()
    {
        var from = new HtmlAttributes();
        from.AddClass("test");
        from.AddProperty("key1", "1");

        var to = new HtmlAttributes();
        from.CopyTo(to);

        Assert.True(ReferenceEquals(from.Classes, to.Classes));
        Assert.True(ReferenceEquals(from.Properties, to.Properties));

        //          From: Classes      From: Properties     To: Classes     To: Properties
        // test1:        null                null              null             null
        from = new HtmlAttributes();
        to = new HtmlAttributes();
        from.CopyTo(to, false, false);
        Assert.Null(to.Classes);
        Assert.Null(to.Properties);

        // test2:      ["test"]            ["key1", "1"]       null             null
        from = new HtmlAttributes();
        to = new HtmlAttributes();
        from.AddClass("test");
        from.AddProperty("key1", "1");
        from.CopyTo(to, false, false);
        Assert.AreEqual(new List<string>() { "test" }, to.Classes);
        Assert.AreEqual(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("key1", "1")}, to.Properties);

        // test3:        null                null            ["test"]       ["key1", "1"]       
        from = new HtmlAttributes();
        to = new HtmlAttributes();
        to.AddClass("test");
        to.AddProperty("key1", "1");
        from.CopyTo(to, false, false);
        Assert.AreEqual(new List<string>() { "test" }, to.Classes);
        Assert.AreEqual(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("key1", "1") }, to.Properties);

        // test4:      ["test1"]           ["key2", "2"]     ["test"]       ["key1", "1"]       
        from = new HtmlAttributes();
        to = new HtmlAttributes();
        from.AddClass("test1");
        from.AddProperty("key2", "2");
        to.AddClass("test");
        to.AddProperty("key1", "1");
        from.CopyTo(to, false, false);
        Assert.AreEqual(new List<string>() { "test", "test1" }, to.Classes);
        Assert.AreEqual(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("key1", "1"), new KeyValuePair<string, string>("key2", "2") }, to.Properties);
    }
}