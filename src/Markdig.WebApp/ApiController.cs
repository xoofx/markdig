using Microsoft.AspNetCore.Mvc;

namespace Markdig.WebApp
{
    public class ApiController : Controller
    {
        // GET api/to_html?text=xxx
        [Route("api/to_html")]
        [HttpGet()]
        public object Get([FromQuery] string text)
        {
            if (text == null)
            {
                text = string.Empty;
            }

            if (text.Length > 1000)
            {
                text = text.Substring(0, 1000);
            }

            var pipeline = new MarkdownPipelineBuilder().UseAllExtensions().Build();
            var result = Markdown.ToHtml(text, pipeline);

            return new {name = "markdig", html = result, version = Markdown.Version};}
    }
}
