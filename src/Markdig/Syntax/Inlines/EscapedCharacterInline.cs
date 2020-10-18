using System.Diagnostics;

namespace Markdig.Syntax.Inlines
{
    [DebuggerDisplay("\\{Character}")]
    public class EscapedCharacterInline : LeafInline
    {
        public char Character { get; set; }

        public override string ToString() => $@"\\{Character}";
    }
}
