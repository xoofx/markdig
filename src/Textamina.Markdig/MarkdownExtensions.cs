// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Extensions;
using Textamina.Markdig.Extensions.Attributes;
using Textamina.Markdig.Extensions.Footnotes;
using Textamina.Markdig.Extensions.Tables;

namespace Textamina.Markdig
{
    public static class MarkdownExtensions
    {
        public static MarkdownPipeline UseAllExtensions(this MarkdownPipeline pipeline)
        {
            return pipeline
                .UsePipeTable()
                .UseSoftlineBreakAsHardlineBreak()
                .UseFootnoteExtensions()
                .UseSoftlineBreakAsHardlineBreak()
                .UseAttributes();
        }

        public static MarkdownPipeline UsePipeTable(this MarkdownPipeline pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<PipeTableExtension>();
            return pipeline;
        }

        public static MarkdownPipeline UseFootnoteExtensions(this MarkdownPipeline pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<FootnoteExtension>();
            return pipeline;
        }

        public static MarkdownPipeline UseSoftlineBreakAsHardlineBreak(this MarkdownPipeline pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<SoftlineBreakAsHardlineExtension>();
            return pipeline;
        }

        public static MarkdownPipeline UseStrikethroughSuperAndSubScript(this MarkdownPipeline pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<StrikethroughSuperAndSubScriptExtension>();
            return pipeline;
        }

        public static MarkdownPipeline UseAttributes(this MarkdownPipeline pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<AttributesExtension>();
            return pipeline;
        }
    }
}