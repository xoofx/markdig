// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using Markdig.Jira;
using Markdig.Renderers.Normalize;
using Markdig.Renderers.Roundtrip;

namespace Markdig.Tests
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Run NUnit tests runner with this");
            // Uncomment the following line to debug a specific tests more easily
            //var tests = new Specs.CommonMarkV_0_29.TestLeafBlocksSetextHeadings();
            //tests.LeafBlocksSetextHeadings_Example063();


            var parser = new WikiMarkupParser();

            var doc = parser.Parse("This is a *text*\n{noformat}\nYo\n{noformat}\nAnother paragraph\n");

            var writer = new StringWriter();
            var renderer = new RoundtripRenderer(writer);
            
            var pipeline = new MarkdownPipelineBuilder().UseEmphasisExtras().UseCustomContainers().Build();
            pipeline.Setup(renderer);

            renderer.Render(doc);

            var result = writer.ToString();

            Console.WriteLine(result);


        }
    }
}