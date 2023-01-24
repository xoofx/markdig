// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Tests;

[TestFixture]
public class TestOrderedList
{
    [Test]
    public void TestReplace()
    {
        var list = new OrderedList<ITest>
        {
            new A(),
            new B(),
            new C(),
        };

        // Replacing B with D. Order should now be A, D, B.
        var result = list.Replace<B>(new D());
        Assert.That(result, Is.True);
        Assert.That(list.Count, Is.EqualTo(3));
        Assert.That(list[0], Is.InstanceOf<A>());
        Assert.That(list[1], Is.InstanceOf<D>());
        Assert.That(list[2], Is.InstanceOf<C>());

        // Replacing B again should fail, as it's no longer in the list.
        Assert.That(list.Replace<B>(new D()), Is.False);
    }

    #region Test fixtures
    private interface ITest { }
    private class A : ITest { }
    private class B : ITest { }
    private class C : ITest { }
    private class D : ITest { }
    #endregion
}