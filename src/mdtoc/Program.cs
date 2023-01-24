// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace mdtoc;

/// <summary>
/// A tool to generate a markdown TOC from a markdown local file or a github link to a markdown file.
/// </summary>
class Program
{
    static void Error(string message)
    {
        Console.WriteLine(message);
        Environment.Exit(1);
    }

    static void Main(string[] args)
    {
        if (args.Length != 1 || args[0] is "--help" or "-help" or "/?" or "/help")
        {
            Error("Usage: mdtoc [markdown file path | http github URL]");
            return;
        }

        var path = args[0];
        string? markdown = null;
        if (path.StartsWith("https:"))
        {
            if (!Uri.TryCreate(path, UriKind.Absolute, out Uri? uri))
            {
                Error($"Unable to parse Uri `{path}`");
                return;
            }
            // Special handling of github URL to access the raw content instead
            if (uri.Host == "github.com")
            {
                // https://github.com/lunet-io/scriban/blob/master/doc/language.md
                // https://raw.githubusercontent.com/lunet-io/scriban/master/doc/language.md
                var newPath = uri.AbsolutePath;
                var paths = new List<string>(newPath.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries));
                if (paths.Count < 5 || paths[2] != "blob")
                {
                    Error($"Invalid github.com URL `{path}`");
                    return;
                }
                paths.RemoveAt(2); // remove blob
                uri = new Uri($"https://raw.githubusercontent.com/{(string.Join("/", paths))}");
            }

            var httpClient = new HttpClient();
            markdown = httpClient.GetStringAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        else
        {
            markdown = File.ReadAllText(path);
        }

        var pipeline = new MarkdownPipelineBuilder().UseAutoIdentifiers(AutoIdentifierOptions.GitHub).Build();
        var doc = Markdown.Parse(markdown, pipeline);

        // Precomputes the minHeading
        var headings = doc.Descendants<HeadingBlock>().ToList();
        int minHeading = int.MaxValue;
        int maxHeading = int.MinValue;
        foreach (var heading in headings)
        {
            minHeading = Math.Min(minHeading, heading.Level);
            maxHeading = Math.Max(maxHeading, heading.Level);
        }

        var writer = Console.Out;
        // Use this htmlWriter to write content of headings into link label
        var htmlWriter = new HtmlRenderer(writer) {EnableHtmlForInline = true};
        foreach (var heading in headings)
        {
            var indent = heading.Level - minHeading;
            for (int i = 0; i < indent; i++)
            {
                //            - Start Of Heading
                writer.Write("  ");
            }
            writer.Write("- [");
            htmlWriter.WriteLeafInline(heading);
            writer.Write($"](#{heading.GetAttributes().Id})");
            writer.WriteLine();
        }
    }
}
