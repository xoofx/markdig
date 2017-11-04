using Markdig.Renderers.Normalize;

namespace Markdig.Extensions.TaskLists
{
    public class NormalizeTaskListRenderer : NormalizeObjectRenderer<TaskList>
    {
        protected override void Write(NormalizeRenderer renderer, TaskList obj)
        {
            renderer.Write("[");
            renderer.Write(obj.Checked ? "X" : " ");
            renderer.Write("]");
        }
    }
}
