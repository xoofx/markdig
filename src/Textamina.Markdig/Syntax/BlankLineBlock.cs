// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Syntax
{
    public sealed class BlankLineBlock : Block
    {
        public BlankLineBlock() : base(null)
        {
        }

        public static readonly BlankLineBlock Instance = new BlankLineBlock();
    }
}