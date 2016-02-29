namespace Textamina.Markdig
{
    public interface IMarkdownExtension
    {
        void Setup(MarkdownPipeline pipeline);
    }
}