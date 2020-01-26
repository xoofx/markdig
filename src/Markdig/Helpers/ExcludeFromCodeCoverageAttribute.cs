#if NET35 || UAP
using System;

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    internal class ExcludeFromCodeCoverageAttribute : Attribute
    {

    }
}
#endif