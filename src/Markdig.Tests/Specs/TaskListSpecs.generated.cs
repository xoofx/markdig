// Generated: 2019-04-05 16:06:14

// --------------------------------
//            Task Lists
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.TaskLists
{
    [TestFixture]
    public class TestExtensionsTaskLists
    {
        // # Extensions
        // 
        // Adds support for task lists:
        // 
        // ## TaskLists
        //  
        // A task list item consist of `[ ]` or `[x]` or `[X]` inside a list item (ordered or unordered)
        [Test]
        public void ExtensionsTaskLists_Example001()
        {
            // Example 1
            // Section: Extensions / TaskLists
            //
            // The following Markdown:
            //     - [ ] Item1
            //     - [x] Item2
            //     - [ ] Item3
            //     - Item4
            //
            // Should be rendered as:
            //     <ul class="contains-task-list">
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" /> Item1</li>
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> Item2</li>
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" /> Item3</li>
            //     <li>Item4</li>
            //     </ul>

            Console.WriteLine("Example 1\nSection Extensions / TaskLists\n");
            TestParser.TestSpec("- [ ] Item1\n- [x] Item2\n- [ ] Item3\n- Item4", "<ul class=\"contains-task-list\">\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" /> Item1</li>\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" checked=\"checked\" /> Item2</li>\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" /> Item3</li>\n<li>Item4</li>\n</ul>", "tasklists|advanced");
        }

        // A task is not recognized outside a list item:
        [Test]
        public void ExtensionsTaskLists_Example002()
        {
            // Example 2
            // Section: Extensions / TaskLists
            //
            // The following Markdown:
            //     [ ] This is not a task list
            //
            // Should be rendered as:
            //     <p>[ ] This is not a task list</p>

            Console.WriteLine("Example 2\nSection Extensions / TaskLists\n");
            TestParser.TestSpec("[ ] This is not a task list", "<p>[ ] This is not a task list</p>", "tasklists|advanced");
        }
    }
}
