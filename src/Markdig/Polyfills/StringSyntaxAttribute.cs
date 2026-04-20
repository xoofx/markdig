#if !NET7_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
internal sealed class StringSyntaxAttribute : Attribute
{
    public const string Regex = nameof(Regex);
    public const string Uri = nameof(Uri);
    public const string Json = nameof(Json);
    public const string Xml = nameof(Xml);
    public const string CompositeFormat = nameof(CompositeFormat);

    public StringSyntaxAttribute(string syntax)
    {
        Syntax = syntax;
        Arguments = Array.Empty<object?>();
    }

    public StringSyntaxAttribute(string syntax, params object?[] arguments)
    {
        Syntax = syntax;
        Arguments = arguments;
    }

    public string Syntax { get; }

    public object?[] Arguments { get; }
}
#endif
