// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Markdig.Renderers.Normalize
{
    public class NormalizeOptions
    {
        public NormalizeOptions()
        {
            SpaceAfterQuoteBlock = true;
            EmptyLineAfterCodeBlock = true;
            EmptyLineAfterHeading = true;
            EmptyLineAfterThematicBreak = true;
            DefaultListItemCharacter = '-';
        }

        public bool SpaceAfterQuoteBlock { get; set; }

        public bool EmptyLineAfterCodeBlock { get; set; }

        public bool EmptyLineAfterHeading { get; set; }

        public bool EmptyLineAfterThematicBreak { get; set; }

        public char DefaultListItemCharacter { get; set; }
    }
}