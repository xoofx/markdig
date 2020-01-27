#if NET35
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    internal class ExcludeFromCodeCoverageAttribute : Attribute
    {

    }
}
#endif