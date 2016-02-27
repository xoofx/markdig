using System.Collections.Generic;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Parsers.Inlines;

namespace Textamina.Markdig.Syntax.Inlines
{
    public class EmphasisInline : ContainerInline
    {
        public char DelimiterChar { get; set; }

        public bool Strong { get; set; }

        public override string ToString()
        {
            return Strong ? "<strong>" : "<em>";
        }

        public static void ProcessEmphasis(Inline root)
        {
            var container = root as ContainerInline;
            if (container == null)
            {
                return;
            }

            var delimiters = new List<EmphasisDelimiterInline>();

            if (container is EmphasisDelimiterInline)
            {
                delimiters.Add((EmphasisDelimiterInline)container);
            }

            // Move current_position forward in the delimiter stack (if needed) until 
            // we find the first potential closer with delimiter * or _. (This will be the potential closer closest to the beginning of the input – the first one in parse order.)
            var child = container.LastChild;
            while (child != null)
            {
                var delimiter = child as EmphasisDelimiterInline;
                if (delimiter != null)
                {
                    delimiters.Add(delimiter);
                }
                var subContainer = child as ContainerInline;
                child = subContainer?.LastChild;
                if (delimiter == null && subContainer is DelimiterInline)
                {
                    subContainer.ReplaceBy(new LiteralInline() {Content = new StringSlice(((DelimiterInline)subContainer).ToLiteral()), IsClosed = true});
                }
            }

            ProcessEmphasis(delimiters);
        }

        private static void ProcessEmphasis(List<EmphasisDelimiterInline> delimiters)
        {
            // Move current_position forward in the delimiter stack (if needed) until 
            // we find the first potential closer with delimiter * or _. (This will be the potential closer closest to the beginning of the input – the first one in parse order.)
            for (int i = 0; i < delimiters.Count; i++)
            {
                var closeDelimiter = delimiters[i];
                if ((closeDelimiter.Type & DelimiterType.Close) != 0 && closeDelimiter.DelimiterCount > 0)
                {
                    while (true)
                    {
                        // Now, look back in the stack (staying above stack_bottom and the openers_bottom for this delimiter type) 
                        // for the first matching potential opener (“matching” means same delimiter).
                        EmphasisDelimiterInline openDelimiter = null;
                        int openDelimiterIndex = -1;
                        for (int j = i - 1; j >= 0; j--)
                        {
                            var previousOpenDelimiter = delimiters[j];
                            if (previousOpenDelimiter.DelimiterChar == closeDelimiter.DelimiterChar &&
                                (previousOpenDelimiter.Type & DelimiterType.Open) != 0 &&
                                previousOpenDelimiter.DelimiterCount > 0)
                            {
                                openDelimiter = previousOpenDelimiter;
                                openDelimiterIndex = j;
                                break;
                            }
                        }

                        if (openDelimiter != null)
                        {
                            process_delims:
                            bool isStrong = openDelimiter.DelimiterCount >= 2 && closeDelimiter.DelimiterCount >= 2;

                            // Insert an emph or strong emph node accordingly, after the text node corresponding to the opener.

                            var emphasis = new EmphasisInline()
                            {
                                DelimiterChar = closeDelimiter.DelimiterChar,
                                Strong = isStrong
                            };


                            var embracer = (ContainerInline)openDelimiter;

                            // Go down to the first emphasis with a lower level
                            while (true)
                            {
                                var previousEmphasis = embracer.FirstChild as EmphasisInline;
                                if (previousEmphasis != null && previousEmphasis.Strong && !isStrong && embracer.FirstChild == embracer.LastChild)
                                {
                                    embracer = previousEmphasis;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // Embrace all delimiters
                            embracer.EmbraceChildrenBy(emphasis);

                            openDelimiter.DelimiterCount -= isStrong ? 2 : 1;
                            closeDelimiter.DelimiterCount -= isStrong ? 2 : 1;

                            // Remove any intermediate emphasis
                            for (int k = i - 1; k >= openDelimiterIndex + 1; k--)
                            {
                                var literalDelimiter = delimiters[k];
                                var literal = new LiteralInline()
                                {
                                    Content = new StringSlice(literalDelimiter.ToLiteral()),
                                    IsClosed = true
                                };

                                literalDelimiter.ReplaceBy(literal);
                                delimiters.RemoveAt(k);
                                i--;
                            }

                            if (closeDelimiter.DelimiterCount == 0)
                            {
                                closeDelimiter.MoveChildrenAfter(emphasis);
                                closeDelimiter.Remove();
                                delimiters.RemoveAt(i);
                                i--;

                                // Remove the open delimiter if it is also empty
                                if (openDelimiter.DelimiterCount == 0)
                                {
                                    openDelimiter.MoveChildrenAfter(openDelimiter);
                                    openDelimiter.Remove();
                                    delimiters.RemoveAt(openDelimiterIndex);
                                    i--;
                                }
                                break;
                            }

                            // The current delimiters are matching
                            if (openDelimiter.DelimiterCount > 0)
                            {
                                goto process_delims;
                            }
                            else
                            {
                                // Remove the open delimiter if it is also empty
                                var firstChild = openDelimiter.FirstChild;
                                firstChild.Remove();
                                openDelimiter.ReplaceBy(firstChild);
                                firstChild.IsClosed = true;
                                closeDelimiter.Remove();
                                firstChild.InsertAfter(closeDelimiter);
                                delimiters.RemoveAt(openDelimiterIndex);
                                i--;
                            }
                        }
                        else if ((closeDelimiter.Type & DelimiterType.Open) == 0)
                        {
                            var literal = new LiteralInline()
                            {
                                Content = new StringSlice(closeDelimiter.ToLiteral()),
                                IsClosed = true
                            };

                            closeDelimiter.ReplaceBy(literal);
                            delimiters.RemoveAt(i);
                            i--;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Any delimiters left must be literal
            for (int i = 0; i < delimiters.Count; i++)
            {
                var delimiter = delimiters[i];
                var literal = new LiteralInline()
                {
                    Content = new StringSlice(delimiter.ToLiteral()),
                    IsClosed = true
                };

                delimiter.ReplaceBy(literal);
            }
            delimiters.Clear();
        }
    }
}