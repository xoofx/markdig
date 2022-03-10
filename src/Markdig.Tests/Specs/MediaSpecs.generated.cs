
// --------------------------------
//               Media
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.Media
{
    [TestFixture]
    public class TestExtensionsMediaLinks
    {
        // # Extensions
        // 
        // Adds support for media links:
        // 
        // ## Media links
        // 
        // Allows to embed audio/video links to popular website:
        [Test]
        public void ExtensionsMediaLinks_Example001()
        {
            // Example 1
            // Section: Extensions / Media links
            //
            // The following Markdown:
            //     ![youtube.com](https://www.youtube.com/watch?v=mswPy5bt3TQ)
            //     
            //     ![youtube.com with t](https://www.youtube.com/watch?v=mswPy5bt3TQ&t=100)
            //     
            //     ![youtu.be](https://youtu.be/mswPy5bt3TQ)
            //     
            //     ![youtu.be with t](https://youtu.be/mswPy5bt3TQ?t=100)
            //     
            //     ![youtube.com/embed 1](https://www.youtube.com/embed/mswPy5bt3TQ?start=100&rel=0)
            //     
            //     ![youtube.com/embed 2](https://www.youtube.com/embed?listType=playlist&list=PLC77007E23FF423C6)
            //     
            //     ![vimeo](https://vimeo.com/8607834)
            //     
            //     ![static mp4](https://sample.com/video.mp4)
            //     
            //     ![yandex.ru](https://music.yandex.ru/album/411845/track/4402274)
            //     
            //     ![ok.ru](https://ok.ru/video/26870090463)
            //
            // Should be rendered as:
            //     <p><iframe src="https://www.youtube.com/embed/mswPy5bt3TQ" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>
            //     <p><iframe src="https://www.youtube.com/embed/mswPy5bt3TQ?start=100" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>
            //     <p><iframe src="https://www.youtube.com/embed/mswPy5bt3TQ" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>
            //     <p><iframe src="https://www.youtube.com/embed/mswPy5bt3TQ?start=100" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>
            //     <p><iframe src="https://www.youtube.com/embed/mswPy5bt3TQ?start=100&amp;rel=0" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>
            //     <p><iframe src="https://www.youtube.com/embed?listType=playlist&amp;list=PLC77007E23FF423C6" class="youtube" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>
            //     <p><iframe src="https://player.vimeo.com/video/8607834" class="vimeo" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>
            //     <p><video width="500" height="281" controls=""><source type="video/mp4" src="https://sample.com/video.mp4"></source></video></p>
            //     <p><iframe src="https://music.yandex.ru/iframe/#track/4402274/411845/" class="yandex" width="500" height="281" frameborder="0"></iframe></p>
            //     <p><iframe src="https://ok.ru/videoembed/26870090463" class="odnoklassniki" width="500" height="281" frameborder="0" allowfullscreen=""></iframe></p>

            TestParser.TestSpec("![youtube.com](https://www.youtube.com/watch?v=mswPy5bt3TQ)\n\n![youtube.com with t](https://www.youtube.com/watch?v=mswPy5bt3TQ&t=100)\n\n![youtu.be](https://youtu.be/mswPy5bt3TQ)\n\n![youtu.be with t](https://youtu.be/mswPy5bt3TQ?t=100)\n\n![youtube.com/embed 1](https://www.youtube.com/embed/mswPy5bt3TQ?start=100&rel=0)\n\n![youtube.com/embed 2](https://www.youtube.com/embed?listType=playlist&list=PLC77007E23FF423C6)\n\n![vimeo](https://vimeo.com/8607834)\n\n![static mp4](https://sample.com/video.mp4)\n\n![yandex.ru](https://music.yandex.ru/album/411845/track/4402274)\n\n![ok.ru](https://ok.ru/video/26870090463)", "<p><iframe src=\"https://www.youtube.com/embed/mswPy5bt3TQ\" class=\"youtube\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n<p><iframe src=\"https://www.youtube.com/embed/mswPy5bt3TQ?start=100\" class=\"youtube\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n<p><iframe src=\"https://www.youtube.com/embed/mswPy5bt3TQ\" class=\"youtube\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n<p><iframe src=\"https://www.youtube.com/embed/mswPy5bt3TQ?start=100\" class=\"youtube\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n<p><iframe src=\"https://www.youtube.com/embed/mswPy5bt3TQ?start=100&amp;rel=0\" class=\"youtube\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n<p><iframe src=\"https://www.youtube.com/embed?listType=playlist&amp;list=PLC77007E23FF423C6\" class=\"youtube\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n<p><iframe src=\"https://player.vimeo.com/video/8607834\" class=\"vimeo\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>\n<p><video width=\"500\" height=\"281\" controls=\"\"><source type=\"video/mp4\" src=\"https://sample.com/video.mp4\"></source></video></p>\n<p><iframe src=\"https://music.yandex.ru/iframe/#track/4402274/411845/\" class=\"yandex\" width=\"500\" height=\"281\" frameborder=\"0\"></iframe></p>\n<p><iframe src=\"https://ok.ru/videoembed/26870090463\" class=\"odnoklassniki\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>", "medialinks|advanced+medialinks", context: "Example 1\nSection Extensions / Media links\n");
        }
    }
}
