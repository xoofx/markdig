using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Markdig.Extensions.AutoIdentifiers;

namespace Markdig.WebApp
{
    public class ApiController : Controller
    {
        [HttpGet()]
        [Route("")]
        public string Empty()
        {
            return string.Empty;
        }

        // GET api/to_html?text=xxx&extensions=advanced
        [Route("api/to_html")]
        [HttpGet()]
        public object Get([FromQuery] string text, [FromQuery] string extension)
        {
            try
            {
                string mdText = System.IO.File.ReadAllText("C:\\Learning\\DocWorks\\documentationmanual\\documentation\\content\\md\\codeTest.md");
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions();
                pipeline.Extensions.Remove(pipeline.Extensions.Find<AutoIdentifierExtension>());
                var htmlString = Markdig.Markdown.ToHtml(mdText, pipeline.Build());

                return new { name = "markdig", html = htmlString, version = Markdown.Version };
            }
            catch (Exception ex)
            {
                return new { name = "markdig", html = "exception: " + GetPrettyMessageFromException(ex), version = Markdown.Version };
            }
        }

        private static string GetPrettyMessageFromException(Exception exception)
        {
            var builder = new StringBuilder();
            while (exception != null)
            {
                builder.Append(exception.Message);
                exception = exception.InnerException;
            }
            return builder.ToString();
        }
    }
}
