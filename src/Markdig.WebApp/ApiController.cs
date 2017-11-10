using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;

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
                if (text == null)
                {
                    text = string.Empty;
                }

                if (text.Length > 1000)
                {
                    text = text.Substring(0, 1000);
                }

                var pipeline = new MarkdownPipelineBuilder().Configure(extension).Build();
                var result = Markdown.ToHtml(text, pipeline);

                return new {name = "markdig", html = result, version = Markdown.Version};
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
