using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class ContainerLinkInline : ContainerInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        private class ParserInternal : InlineParser
        {
            public ParserInternal()
            {
                FirstChars = new[] {'[', ']', '!'};
            }

            public override bool Match(MatchInlineState state)
            {
                var lines = state.Lines;

                var c = lines.Current;

                bool isImage = false;
                if (c == '!')
                {
                    isImage = true;
                    c = lines.NextChar();
                    if (c != '[')
                    {
                        return false;
                    }
                }

                if (c == '[')
                {
                    state.Inline = new LinkDelimiterInline(this)
                    {
                        Type = DelimiterType.Open,
                        IsImage = isImage
                    };
                    return true;
                }
                else if (c == ']')
                {
                    if (state.Inline != null)
                    {
                        LinkDelimiterInline firstParent = null;
                        LinkDelimiterInline lastParent = null;
                        foreach (var parent in state.Inline.FindParentOfType<LinkDelimiterInline>())
                        {
                            lastParent = parent;
                            if (firstParent == null)
                            {
                                firstParent = parent;
                            }
                            else
                            {
                                // We don't need to go through the whole hierarchy
                                break;
                            }
                        }

                        // This will be matched as a literal
                        if (firstParent != null)
                        {
                            // TODO: continue parsing of ]


                            // We have a nested [ ]
                            if (firstParent != lastParent)
                            {
                                // firstParent.Remove();
                                // The opening [ will be transformed to a literal followed by all the childrens of the [ 

                                var literal = new LiteralInline() {Content = firstParent.IsImage ? "![" : "["};
                                firstParent.ReplaceBy(literal);

                                var child = firstParent.FirstChild;
                                var lastChild = child;
                                while (child != null)
                                {
                                    lastChild = child;
                                    var nextChild = child.NextSibling;
                                    child.Remove();
                                    literal.InsertAfter(child);
                                    child = nextChild;
                                }

                                state.Inline = lastChild;
                            }
                            else
                            {

                                // TODO: continue parsing of ]
                                
                            }
                            return true;
                        }
                    }
                    return false;



                    // Match a close tag



                    
                }


                // We don't have an emphasis
                return false;
            }
        }
    }


    public class TextLinkInline : ContainerLinkInline
    {
        
    }


    public class ImageLinkInline : ContainerLinkInline
    {
        
    }
}