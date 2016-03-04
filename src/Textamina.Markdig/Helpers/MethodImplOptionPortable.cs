// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Runtime.CompilerServices;

namespace Textamina.Markdig.Helpers
{
    internal static class MethodImplOptionPortable
    {
        public const MethodImplOptions AggressiveInlining = (MethodImplOptions)256;
    }
}