using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdig.Parsers.Inlines;

public class LinkInlineOptions
{
    /// <summary>
    /// Should the link open in a new window when clicked (false by default)
    /// </summary>
    public bool OpenInNewWindow { get; set; }
}
