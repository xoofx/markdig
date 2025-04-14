// Based on code at located at https://github.com/dotnet/runtime/blob/79ae74f5ca5c8a6fe3a48935e85bd7374959c570/src/libraries/System.Private.CoreLib/src/System/Diagnostics/CodeAnalysis/NullableAttributes.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Diagnostics.CodeAnalysis;

#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP3_1_OR_GREATER
internal sealed class DoesNotReturnAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class NotNullWhenAttribute : Attribute
{
    public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

    public bool ReturnValue { get; }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
internal sealed class AllowNullAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
internal sealed class MaybeNullAttribute : Attribute { }
#endif

#if !NET5_0_OR_GREATER
internal sealed class MemberNotNullAttribute : Attribute
{
    public MemberNotNullAttribute(string member) => Members = [member];

    public MemberNotNullAttribute(params string[] members) => Members = members;

    public string[] Members { get; }
}
#endif
