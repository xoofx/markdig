// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;

namespace Textamina.Markdig.Extensions.SmartyPants
{
    /// <summary>
    /// The options used for <see cref="SmartyPantsExtension"/>.
    /// </summary>
    public class SmartyPantOptions
    {
        public SmartyPantOptions()
        {
            Mapping = new Dictionary<SmartyPantType, string>()
            {
                {SmartyPantType.Quote, "'"},
                {SmartyPantType.DoubleQuote, "\""},
                {SmartyPantType.LeftQuote, "&lsquo;"},
                {SmartyPantType.RightQuote, "&rsquo;"},
                {SmartyPantType.LeftDoubleQuote, "&ldquo;"},
                {SmartyPantType.RightDoubleQuote, "&rdquo;"},
                {SmartyPantType.LeftAngleQuote, "&laquo;"},
                {SmartyPantType.RightAngleQuote, "&raquo;"},
                {SmartyPantType.Ellipsis, "&hellip;"},
                {SmartyPantType.Dash2, "&ndash;"},
                {SmartyPantType.Dash3, "&mdash;"},
            };
        }

        public Dictionary<SmartyPantType, string> Mapping { get; }
    }
}