namespace Textamina.Markdig.Syntax
{
    public sealed class BlankLineBlock : Block
    {
        public BlankLineBlock() : base(null)
        {
        }

        public static readonly BlankLineBlock Instance = new BlankLineBlock();
    }
}