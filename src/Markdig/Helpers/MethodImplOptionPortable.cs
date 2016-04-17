// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Runtime.CompilerServices;

namespace Markdig.Helpers
{
    /// <summary>
    /// Internal helper to allow to declare a method using AggressiveInlining without being .NET 4.0+
    /// </summary>
    internal static class MethodImplOptionPortable
    {
        public const MethodImplOptions AggressiveInlining = (MethodImplOptions)256;
    }
}