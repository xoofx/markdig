using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public sealed class TestDebuggerDisplay
{
    private static readonly Regex ExpressionMemberRegex = new(@"\{([A-Za-z_]\w*)", RegexOptions.Compiled);

    private static IEnumerable<Type> SyntaxNodeTypes()
    {
        return typeof(MarkdownObject).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && (typeof(Block).IsAssignableFrom(type) || typeof(Inline).IsAssignableFrom(type)))
            .OrderBy(type => type.FullName);
    }

    [TestCaseSource(nameof(SyntaxNodeTypes))]
    public void SyntaxNodeHasDebuggerDisplay(Type type)
    {
        var declaringType = type;
        DebuggerDisplayAttribute? attribute = null;
        while (declaringType is not null && attribute is null)
        {
            attribute = declaringType.GetCustomAttribute<DebuggerDisplayAttribute>(inherit: false);
            declaringType = declaringType.BaseType;
        }

        Assert.IsNotNull(attribute, $"{type.FullName} has no [DebuggerDisplay] on itself or a base type");

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        foreach (Match match in ExpressionMemberRegex.Matches(attribute!.Value))
        {
            var memberName = match.Groups[1].Value;
            var members = type.GetMember(memberName, flags);
            for (var baseType = type.BaseType; members.Length == 0 && baseType is not null; baseType = baseType.BaseType)
            {
                members = baseType.GetMember(memberName, flags);
            }

            Assert.IsNotEmpty(members, $"[DebuggerDisplay(\"{attribute.Value}\")] on {type.FullName} references unknown member {memberName}");
        }
    }
}
