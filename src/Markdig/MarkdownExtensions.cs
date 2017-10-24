// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using Markdig.Extensions.Abbreviations;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Extensions.AutoLinks;
using Markdig.Extensions.Bootstrap;
using Markdig.Extensions.Citations;
using Markdig.Extensions.CustomContainers;
using Markdig.Extensions.DefinitionLists;
using Markdig.Extensions.Diagrams;
using Markdig.Extensions.Emoji;
using Markdig.Extensions.EmphasisExtras;
using Markdig.Extensions.Figures;
using Markdig.Extensions.Footers;
using Markdig.Extensions.Footnotes;
using Markdig.Extensions.GenericAttributes;
using Markdig.Extensions.Hardlines;
using Markdig.Extensions.JiraLinks;
using Markdig.Extensions.ListExtras;
using Markdig.Extensions.Mathematics;
using Markdig.Extensions.MediaLinks;
using Markdig.Extensions.NoRefLinks;
using Markdig.Extensions.PragmaLines;
using Markdig.Extensions.SelfPipeline;
using Markdig.Extensions.SmartyPants;
using Markdig.Extensions.NonAsciiNoEscape;
using Markdig.Extensions.Tables;
using Markdig.Extensions.TaskLists;
using Markdig.Extensions.Yaml;
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
        /// Adds the specified extension to the extensions collection.
        /// </summary>
        /// <typeparam name="TExtension">The type of the extension.</typeparam>
        /// <returns>The instance of <see cref="MarkdownPipelineBuilder" /></returns>
        public static MarkdownPipelineBuilder Use<TExtension>(this MarkdownPipelineBuilder pipeline) where TExtension : class, IMarkdownExtension, new()
        {
            pipeline.Extensions.AddIfNotAlready<TExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses all extensions except the BootStrap, Emoji, SmartyPants and soft line as hard line breaks extensions.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseAdvancedExtensions(this MarkdownPipelineBuilder pipeline)
        {
            return pipeline
                .UseAbbreviations()
                .UseAutoIdentifiers()
                .UseCitations()
                .UseCustomContainers()
                .UseDefinitionLists()
                .UseEmphasisExtras()
                .UseFigures()
                .UseFooters()
                .UseFootnotes()
                .UseGridTables()
                .UseMathematics()
                .UseMediaLinks()
                .UsePipeTables()
                .UseListExtras()
                .UseTaskLists()
                .UseDiagrams()
                .UseAutoLinks()
                .UseGenericAttributes(); // Must be last as it is one parser that is modifying other parsers
        }
        
        /// <summary>
        /// Uses this extension to enable autolinks from text `http://`, `https://`, `ftp://`, `mailto:`, `www.xxx.yyy`
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseAutoLinks(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<AutoLinkExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses this extension to disable URI escape with % characters for non-US-ASCII characters in order to workaround a bug under IE/Edge with local file links containing non US-ASCII chars. DO NOT USE OTHERWISE.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseNonAsciiNoEscape(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<NonAsciiNoEscapeExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses YAML frontmatter extension that will parse a YAML frontmatter into the MarkdownDocument. Note that they are not rendered by any default HTML renderer.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseYamlFrontMatter(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<YamlFrontMatterExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the self pipeline extension that will detect the pipeline to use from the markdown input that contains a special tag. See <see cref="SelfPipelineExtension"/>
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="defaultTag">The default tag to use to match the self pipeline configuration. By default, <see cref="SelfPipelineExtension.DefaultTag"/>, meaning that the HTML tag will be &lt;--markdig:extensions--&gt;</param>
        /// <param name="defaultExtensions">The default extensions to configure if no pipeline setup was found from the Markdown document</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseSelfPipeline(this MarkdownPipelineBuilder pipeline, string defaultTag = SelfPipelineExtension.DefaultTag, string defaultExtensions = null)
        {
            if (pipeline.Extensions.Count != 0)
            {
                throw new InvalidOperationException("The SelfPipeline extension cannot be used with other extensions");
            }

            pipeline.Extensions.Add(new SelfPipelineExtension(defaultTag, defaultExtensions));
            return pipeline;
        }

        /// <summary>
        /// Uses pragma lines to output span with an id containing the line number (pragma-line#line_number_zero_based`)
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UsePragmaLines(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<PragmaLineExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the diagrams extension
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseDiagrams(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<DiagramExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses precise source code location (useful for syntax highlighting).
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UsePreciseSourceLocation(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.PreciseSourceLocation = true;
            return pipeline;
        }

        /// <summary>
        /// Uses the task list extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseTaskLists(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<TaskListExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the custom container extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseCustomContainers(this MarkdownPipelineBuilder pipeline)
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
        public static MarkdownPipelineBuilder UseMediaLinks(this MarkdownPipelineBuilder pipeline, MediaOptions options = null)
        {
            if (!pipeline.Extensions.Contains<MediaLinkExtension>())
            {
                pipeline.Extensions.Add(new MediaLinkExtension(options));
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
        public static MarkdownPipelineBuilder UseAutoIdentifiers(this MarkdownPipelineBuilder pipeline, AutoIdentifierOptions options = AutoIdentifierOptions.Default)
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
        /// Uses the bootstrap extension.
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
        public static MarkdownPipelineBuilder UseMathematics(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<MathExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the figure extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseFigures(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<FigureExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the custom abbreviation extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseAbbreviations(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<AbbreviationExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the definition lists extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseDefinitionLists(this MarkdownPipelineBuilder pipeline)
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
        public static MarkdownPipelineBuilder UsePipeTables(this MarkdownPipelineBuilder pipeline, PipeTableOptions options = null)
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
        public static MarkdownPipelineBuilder UseGridTables(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<GridTableExtension>();
            return pipeline;
        }


        /// <summary>
        /// Uses the cite extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseCitations(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<CitationExtension>();
            return pipeline;
        }

        /// <summary>
        /// Uses the footer extension.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseFooters(this MarkdownPipelineBuilder pipeline)
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
        public static MarkdownPipelineBuilder UseEmphasisExtras(this MarkdownPipelineBuilder pipeline, EmphasisExtraOptions options = EmphasisExtraOptions.Default)
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
        public static MarkdownPipelineBuilder UseListExtras(this MarkdownPipelineBuilder pipeline)
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
        /// Add rel=nofollow to all links rendered to HTML.
        /// </summary>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        public static MarkdownPipelineBuilder UseNoFollowLinks(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<NoFollowLinksExtension>();
            return pipeline;
        }

        /// <summary>
        /// Automatically link references to JIRA issues
        /// </summary>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="options">Set of required options</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder UseJiraLinks(this MarkdownPipelineBuilder pipeline, JiraLinkOptions options)
        {
            if (!pipeline.Extensions.Contains<JiraLinkExtension>())
            {
                pipeline.Extensions.Add(new JiraLinkExtension(options));
            }
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
        /// <param name="pipeline">The pipeline (e.g: advanced for <see cref="UseAdvancedExtensions"/>, pipetables+gridtables for <see cref="UsePipeTables"/> and <see cref="UseGridTables"/></param>
        /// <param name="extensions">The extensions to activate as a string</param>
        /// <returns>The modified pipeline</returns>
        public static MarkdownPipelineBuilder Configure(this MarkdownPipelineBuilder pipeline, string extensions)
        {
            if (extensions == null)
            {
                return pipeline;
            }

            // TODO: the extension string should come from the extension itself instead of this hardcoded switch case.

            foreach (var extension in extensions.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries))
            {
                switch (extension.ToLowerInvariant())
                {
                    case "common":
                        break;
                    case "advanced":
                        pipeline.UseAdvancedExtensions();
                        break;
                    case "pipetables":
                        pipeline.UsePipeTables();
                        break;
                    case "emphasisextras":
                        pipeline.UseEmphasisExtras();
                        break;
                    case "listextras":
                        pipeline.UseListExtras();
                        break;
                    case "hardlinebreak":
                        pipeline.UseSoftlineBreakAsHardlineBreak();
                        break;
                    case "footnotes":
                        pipeline.UseFootnotes();
                        break;
                    case "footers":
                        pipeline.UseFooters();
                        break;
                    case "citations":
                        pipeline.UseCitations();
                        break;
                    case "attributes":
                        pipeline.UseGenericAttributes();
                        break;
                    case "gridtables":
                        pipeline.UseGridTables();
                        break;
                    case "abbreviations":
                        pipeline.UseAbbreviations();
                        break;
                    case "emojis":
                        pipeline.UseEmojiAndSmiley();
                        break;
                    case "definitionlists":
                        pipeline.UseDefinitionLists();
                        break;
                    case "customcontainers":
                        pipeline.UseCustomContainers();
                        break;
                    case "figures":
                        pipeline.UseFigures();
                        break;
                    case "mathematics":
                        pipeline.UseMathematics();
                        break;
                    case "bootstrap":
                        pipeline.UseBootstrap();
                        break;
                    case "medialinks":
                        pipeline.UseMediaLinks();
                        break;
                    case "smartypants":
                        pipeline.UseSmartyPants();
                        break;
                    case "autoidentifiers":
                        pipeline.UseAutoIdentifiers();
                        break;
                    case "tasklists":
                        pipeline.UseTaskLists();
                        break;
                    case "diagrams":
                        pipeline.UseDiagrams();
                        break;
                    case "nofollowlinks":
                        pipeline.UseNoFollowLinks();
                        break;
                    case "nohtml":
                        pipeline.DisableHtml();
                        break;
                    case "yaml":
                        pipeline.UseYamlFrontMatter();
                        break;
                    case "nonascii-noescape":
                        pipeline.UseNonAsciiNoEscape();
                        break;
                    case "autolinks":
                        pipeline.UseAutoLinks();
                        break;
                    default:
                        throw new ArgumentException($"Invalid extension `{extension}` from `{extensions}`", nameof(extensions));
                }
            }
            return pipeline;
        }
    }
}
