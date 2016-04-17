// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Diagnostics;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.SmartyPants
{
    /// <summary>
    /// An inline for SmartyPant.
    /// </summary>
    [DebuggerDisplay("SmartyPant {ToString()}")]
    public class SmartyPant : LeafInline
    {
        public char OpeningCharacter { get; set; }

        public SmartyPantType Type { get; set; }

        /// <summary>
        /// Converts this instance to a literal text.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Type)
            {
                case SmartyPantType.Quote:
                case SmartyPantType.LeftQuote:
                case SmartyPantType.RightQuote:
                    return "'";
                case SmartyPantType.DoubleQuote:
                    return "\"";
                case SmartyPantType.LeftDoubleQuote:
                    return OpeningCharacter == '`' ? "``" : "\"";
                case SmartyPantType.RightDoubleQuote:
                    return OpeningCharacter == '\'' ? "''" : "\"";
                case SmartyPantType.Dash2:
                    return "--";
                case SmartyPantType.Dash3:
                    return "--";
                case SmartyPantType.LeftAngleQuote:
                    return "<<";
                case SmartyPantType.RightAngleQuote:
                    return ">>";
            }
            return OpeningCharacter != 0 ? OpeningCharacter.ToString() : string.Empty;
        }
    }
}