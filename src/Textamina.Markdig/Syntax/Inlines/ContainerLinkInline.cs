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


                                // TODO: This part is not efficient as it is using child.Remove()
                                // We need a method to quickly move all children without having to mess Next/Prev sibling
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
                                var link = firstParent.IsImage ? (ContainerLinkInline)new ImageLinkInline() : new TextLinkInline();

                                // 1. Process all delimiters inside firstParent to convert them to inlines
                                // 2. Replace firstParent with link, and move all child to this one

                                // TODO: continue parsing of ]

                                if (state.Inline == firstParent)
                                {
                                    state.Inline = link;
                                }

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
}