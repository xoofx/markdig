using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip
{
    public class EmptyBlockRenderer : RoundtripObjectRenderer<EmptyBlock>
    {
        protected override void Write(RoundtripRenderer renderer, EmptyBlock  noBlocksFoundBlock)
        {
            renderer.RenderLinesAfter(noBlocksFoundBlock);
        }
    }
}
