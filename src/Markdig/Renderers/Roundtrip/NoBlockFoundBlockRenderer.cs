using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip
{
    public class NoBlockFoundBlockRenderer : RoundtripObjectRenderer<NoBlocksFoundBlock>
    {
        protected override void Write(RoundtripRenderer renderer, NoBlocksFoundBlock noBlocksFoundBlock)
        {
            renderer.RenderLinesAfter(noBlocksFoundBlock);
        }
    }
}
