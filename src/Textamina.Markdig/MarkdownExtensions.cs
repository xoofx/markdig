


using Textamina.Markdig.Extensions;
using Textamina.Markdig.Extensions.Footnotes;
using Textamina.Markdig.Extensions.Tables;

namespace Textamina.Markdig
{
    public static class MarkdownExtensions
    {
        public static MarkdownPipeline EnableAllExtensions(this MarkdownPipeline pipeline)
        {
            return pipeline
                .EnablePipeTable()
                .EnableSoftlineBreakAsHardlineBreak();
        }

        public static MarkdownPipeline EnablePipeTable(this MarkdownPipeline pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<PipeTableExtension>();
            return pipeline;
        }

        public static MarkdownPipeline EnableFootnoteExtensions(this MarkdownPipeline pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<FootnoteExtension>();
            return pipeline;
        }

        public static MarkdownPipeline EnableSoftlineBreakAsHardlineBreak(this MarkdownPipeline pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<SoftlineBreakAsHardlineExtension>();
            return pipeline;
        }

        public static MarkdownPipeline EnableStrikethroughSuperAndSubScript(this MarkdownPipeline pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<StrikethroughSuperAndSubScriptExtension>();
            return pipeline;
        }
    }
}