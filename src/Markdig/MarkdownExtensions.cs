// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using Markdig.Extensions.Abbreviations;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Extensions.Bootstrap;
using Markdig.Extensions.Cites;
using Markdig.Extensions.CustomContainers;
using Markdig.Extensions.DefinitionLists;
using Markdig.Extensions.Emoji;
using Markdig.Extensions.EmphasisExtra;
using Markdig.Extensions.Figures;
using Markdig.Extensions.Footers;
using Markdig.Extensions.Footnotes;
using Markdig.Extensions.GenericAttributes;
using Markdig.Extensions.Hardlines;
using Markdig.Extensions.ListExtra;
using Markdig.Extensions.Mathematics;
using Markdig.Extensions.Medias;
using Markdig.Extensions.SmartyPants;
using Markdig.Extensions.Tables;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;

namespace Markdig
{
    /// <summary>
    /// Provides extension methods for <see cref="MarkdownPipeline"/> to enable several Markdown extensions.
    /// </summary>
    public static class MarkdownExtensions
    {
        /// <summary>
        /// Uses all extensions except the Emoji, SmartyPants and soft line as hard line breaks.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseAdvancedExtensions(this MarkdownPipelineBuilder pipeline)
        {
            return pipeline
                .UseAbbreviation()
                .UseAutoIdentifier()
                .UseBootstrap()
                .UseCite()
                .UseCustomContainer()
                .UseDefinitionList()
                .UseEmphasisExtra()
                .UseFigure()
                .UseFooter()
                .UseFootnotes()
                .UseGridTable()
                .UseMath()
                .UseMedia()
                .UsePipeTable()
                .UseGenericAttributes(); // Must be last as it is one parser that is modifying other parsers
        }

        /// <summary>
        /// Uses the custom container extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseCustomContainer(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<CustomContainerExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the media extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// The modified pipeline
        /// </returns>
        public static MarkdownPipelineBuilder UseMedia(this MarkdownPipelineBuilder pipeline, MediaOptions options = null)
        {
            if (!pipeline.Extensions.Contains<MediaExtension>())
            {
                pipeline.Extensions.Add(new MediaExtension(options));
            }
            return pipeline;
        }

        /// <summary>
        /// Uses the auto-identifier extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// The modified pipeline
        /// </returns>
        public static MarkdownPipelineBuilder UseAutoIdentifier(this MarkdownPipelineBuilder pipeline, AutoIdentifierOptions options = AutoIdentifierOptions.Default)
        {
            if (!pipeline.Extensions.Contains<AutoIdentifierExtension>())
            {
                pipeline.Extensions.Add(new AutoIdentifierExtension(options));
            }
            return pipeline;
        }

        /// <summary>
        /// Uses the SmartyPants extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// The modified pipeline
        /// </returns>
        public static MarkdownPipelineBuilder UseSmartyPants(this MarkdownPipelineBuilder pipeline, SmartyPantOptions options = null)
        {
            if (!pipeline.Extensions.Contains<SmartyPantsExtension>())
            {
                pipeline.Extensions.Add(new SmartyPantsExtension(options));
            }
            return pipeline;
        }

        /// <summary>
        /// Uses the boostrap extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseBootstrap(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<BootstrapExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the math extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseMath(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<MathExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the figure extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseFigure(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<FigureExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the custom abbreviation extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseAbbreviation(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<AbbreviationExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the definition lists extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseDefinitionList(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<DefinitionListExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the pipe table extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// The modified pipeline
        /// </returns>
        public static MarkdownPipelineBuilder UsePipeTable(this MarkdownPipelineBuilder pipeline, PipeTableOptions options = null)
        {
            if (!pipeline.Extensions.Contains<PipeTableExtension>())
            {
                pipeline.Extensions.Add(new PipeTableExtension(options));
            }
            return pipeline;
        }

        /// <summary>
        /// Uses the grid table extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseGridTable(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<GridTableExtension>();
            return pipeline;
        }


        /// <summary>
        /// Uses the cite extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseCite(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<CiteExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the footer extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseFooter(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<FooterExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the footnotes extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseFootnotes(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<FootnoteExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the softline break as hardline break extension
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseSoftlineBreakAsHardlineBreak(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<SoftlineBreakAsHardlineExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the strikethrough superscript, subscript, inserted and marked text extensions.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="options">The options to enable.</param>
        /// <returns>
        /// The modified pipeline
        /// </returns>
        public static MarkdownPipelineBuilder UseEmphasisExtra(this MarkdownPipelineBuilder pipeline, EmphasisExtraOptions options = EmphasisExtraOptions.Default)
        {
            if (!pipeline.Extensions.Contains<EmphasisExtraExtension>())
            {
                pipeline.Extensions.Add(new EmphasisExtraExtension(options));
            }
            return pipeline;
        }

        /// <summary>
        /// Uses the list extra extension to add support for `a.`, `A.`, `i.` and `I.` ordered list items.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>
        /// The modified pipeline
        /// </returns>
        public static MarkdownPipelineBuilder UseListExtra(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<ListExtraExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the generic attributes extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseGenericAttributes(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<GenericAttributesExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the emoji and smiley extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseEmojiAndSmiley(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<EmojiExtension>();
            return pipeline;
        }

        /// <summary>
        /// This will disable the HTML support in the markdown processor (for constraint/safe parsing).
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder DisableHtml(this MarkdownPipelineBuilder pipeline)
        {
            var parser = pipeline.BlockParsers.Find<HtmlBlockParser>();
            if (parser != null)
            {
                pipeline.BlockParsers.Remove(parser);
            }

            var inlineParser = pipeline.InlineParsers.Find<AutolineInlineParser>();
            if (inlineParser != null)
            {
                inlineParser.EnableHtmlParsing = false;
            }
            return pipeline;
        }

        /// <summary>
        /// Configures the pipeline using a string that defines the extensions to activate.
        /// </summary>
        /// <param name="pipeline">The pipeline (e.g: advanced for <see cref="UseAdvancedExtensions"/>, pipetables+gridtables for <see cref="UsePipeTable"/> and <see cref="UseGridTable"/></param>
        /// <param name="extensions">The extensions to activate as a string</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder Configure(this MarkdownPipelineBuilder pipeline, string extensions)
        {
            if (extensions == null)
            {
                return pipeline;
            }

            foreach (var extension in extensions.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries))
            {
                switch (extension.ToLowerInvariant())
                {
                    case "advanced":
                        pipeline.UseAdvancedExtensions();
                        break;
                    case "pipetables":
                        pipeline.UsePipeTable();
                        break;
                    case "emphasisextra":
                        pipeline.UseEmphasisExtra();
                        break;
                    case "listextra":
                        pipeline.UseListExtra();
                        break;
                    case "hardlinebreak":
                        pipeline.UseSoftlineBreakAsHardlineBreak();
                        break;
                    case "footnotes":
                        pipeline.UseFootnotes();
                        break;
                    case "footers":
                        pipeline.UseFooter();
                        break;
                    case "cites":
                        pipeline.UseCite();
                        break;
                    case "attributes":
                        pipeline.UseGenericAttributes();
                        break;
                    case "gridtables":
                        pipeline.UseGridTable();
                        break;
                    case "abbreviations":
                        pipeline.UseAbbreviation();
                        break;
                    case "emojis":
                        pipeline.UseEmojiAndSmiley();
                        break;
                    case "definitionlists":
                        pipeline.UseDefinitionList();
                        break;
                    case "customcontainers":
                        pipeline.UseCustomContainer();
                        break;
                    case "figures":
                        pipeline.UseFigure();
                        break;
                    case "math":
                        pipeline.UseMath();
                        break;
                    case "bootstrap":
                        pipeline.UseBootstrap();
                        break;
                    case "medias":
                        pipeline.UseMedia();
                        break;
                    case "smartypants":
                        pipeline.UseSmartyPants();
                        break;
                    case "autoidentifiers":
                        pipeline.UseAutoIdentifier();
                        break;
                    default:
                        throw new ArgumentException($"unknown extension {extension}");
                }
            }
            return pipeline;
        }
    }
}