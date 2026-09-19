// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices;

// Required by the compiler for init-only properties on older target frameworks.
internal static class IsExternalInit
{
}
#endif
