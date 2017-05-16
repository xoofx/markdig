using System;
using System.Collections.Generic;
using System.Text;
using Markdig.Extensions.TaskLists;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Markdig.Extensions.JiraLinks
{
    /// <summary>
    /// Finds and replaces JIRA links inline
    /// </summary>
    public class JiraLinkInlineParser : InlineParser
    {
        public JiraLinkInlineParser()
        {
            //look for uppercase chars at the start (for the project key)
            OpeningCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (!slice.PeekCharExtra(-1).IsWhiteSpaceOrZero())
            {
                return false; //require whitespace/nothing before
            }

            var key = String.Empty;
            var issue = String.Empty;
            var current = slice.CurrentChar;

            //read as many uppercase characters as required - project key
            while (current.IsAlphaUpper())
            {
                key += current;
                current = slice.NextChar();
            }

            //require a '-' between key and issue number
            if (!current.Equals('-'))
            {
                return false;
            }

            current = slice.NextChar();

            //read as many numbers as required - issue number
            while (current.IsDigit()) 
            {
                issue += current;
                current = slice.NextChar();
            }

            if (!current.IsWhiteSpaceOrZero()) //must be followed by whitespace
            {
                return false;
            }

            int line;
            int column;
            processor.Inline = new JiraLink() //create the link at the relevant position
            {
                Span =
                {
                    Start = processor.GetSourcePosition(slice.Start, out line, out column)
                },
                Line = line,
                Column = column,
                Issue = issue,
                Key = key
            };

            processor.Inline.Span.End = processor.Inline.Span.Start + issue.Length + key.Length + 1; //+1 for the '-'

            return true;
        }
    }

}
