// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Normalize;

/// <summary>
/// An Normalize renderer for a <see cref="HeadingBlock"/>.
/// </summary>
/// <seealso cref="NormalizeObjectRenderer{HeadingBlock}" />
public class HeadingRenderer : NormalizeObjectRenderer<HeadingBlock>
{
    private static readonly string[] HeadingTexts = [
        "#",
        "##",
        "###",
        "####",
        "#####",
        "######",
    ];

    protected override void Write(NormalizeRenderer renderer, HeadingBlock obj)
    {        
        if (obj.Level is > 0 and <= 6)
        {
            renderer.Write(HeadingTexts[obj.Level - 1]);
        }
        else
        {
            renderer.Write('#', obj.Level);            
        }        

        renderer.Write(' ');
        renderer.WriteLeafInline(obj);

        renderer.FinishBlock(renderer.Options.EmptyLineAfterHeading);
    }
}