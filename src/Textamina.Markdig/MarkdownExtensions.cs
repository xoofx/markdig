


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
            // If the pipeline doesn't this extension, we can add it
            if (!pipeline.Extensions.Contains<PipeTableExtension>())
            {
                // We don't care about order, so we can add it directly
                var extension = new PipeTableExtension();
                pipeline.Extensions.Add(extension);
            }
            return pipeline;
        }

        public static MarkdownPipeline EnableFootnoteExtensions(this MarkdownPipeline pipeline)
        {
            // If the pipeline doesn't this extension, we can add it
            if (!pipeline.Extensions.Contains<FootnoteExtension>())
            {
                // We don't care about order, so we can add it directly
                var extension = new FootnoteExtension();
                pipeline.Extensions.Add(extension);
            }
            return pipeline;
        }

        public static MarkdownPipeline EnableSoftlineBreakAsHardlineBreak(this MarkdownPipeline pipeline)
        {
            // If the pipeline doesn't this extension, we can add it
            if (!pipeline.Extensions.Contains<SoftlineBreakAsHardlineExtension>())
            {
                // We don't care about order, so we can add it directly
                var extension = new SoftlineBreakAsHardlineExtension();
                pipeline.Extensions.Add(extension);
            }
            return pipeline;
        }
    }
}