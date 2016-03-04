// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Syntax.Inlines
{
    /// <summary>
    /// A hard line break (Section 6.9 CommonMark specs).
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.Inlines.LeafInline" />
    public class HardlineBreakInline : LineBreakInline
    {
        public override string ToString()
        {
            return "<br />";
        }
    }
}