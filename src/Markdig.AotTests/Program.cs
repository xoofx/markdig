// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig;

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

var html = Markdown.ToHtml("# Title\n\n| a | b |\n|---|---|\n| 1 | 2 |\n\nSee https://example.com", pipeline);

Console.Write(html);

return html.Contains("<table>") && html.Contains("<a href=\"https://example.com\">") ? 0 : 1;
