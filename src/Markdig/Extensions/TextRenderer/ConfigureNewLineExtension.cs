using Markdig.Renderers;

namespace Markdig.Extensions.TextRenderer
{
    /// <summary>
    /// Extension that allows setting line-endings for any IMarkdownRenderer
    /// that inherits from <see cref="Markdig.Renderers.TextRendererBase"/>
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class ConfigureNewLineExtension : IMarkdownExtension
    {
        private readonly string newLine;

        public ConfigureNewLineExtension(string newLine)
        {
            this.newLine = newLine;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var textRenderer = renderer as TextRendererBase;
            if (textRenderer == null)
            {
                return;
            }

            textRenderer.Writer.NewLine = newLine;
        }
    }
}
