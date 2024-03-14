using System.Text;

namespace SpecFileGen;

class Program
{
    static readonly string SpecificationsDirectory =
        Path.GetFullPath(
            Path.Combine(
                Path.GetDirectoryName(typeof(Spec).Assembly.Location),
                "../../../../Markdig.Tests/"));

    enum RendererType
    {
        Html,
        Normalize,
        PlainText,
        Roundtrip
    }

    class Spec
    {
        public readonly string Name;
        public readonly string Path;
        public readonly string OutputPath;
        public readonly string Extensions;
        public readonly RendererType RendererType;
        public int TestCount;

        public Spec(string name, string fileName, string extensions, RendererType rendererType = RendererType.Html)
        {
            Name = name;
            Path = SpecificationsDirectory;
            if (rendererType == RendererType.Html) Path += "Specs";
            else if (rendererType == RendererType.Normalize) Path += "NormalizeSpecs";
            else if (rendererType == RendererType.PlainText) Path += "PlainTextSpecs";
            else if (rendererType == RendererType.Roundtrip) Path += "RoundtripSpecs";
            Path += "/" + fileName;
            OutputPath = System.IO.Path.ChangeExtension(Path, "generated.cs");
            Extensions = extensions;
            RendererType = rendererType;
        }
    }
    class NormalizeSpec : Spec
    {
        public NormalizeSpec(string name, string fileName, string extensions)
            : base(name, fileName, extensions, rendererType: RendererType.Normalize) { }
    }
    class PlainTextSpec : Spec
    {
        public PlainTextSpec(string name, string fileName, string extensions)
            : base(name, fileName, extensions, rendererType: RendererType.PlainText) { }
    }
    class RoundtripSpec : Spec
    {
        public RoundtripSpec(string name, string fileName, string extensions)
            : base(name, fileName, extensions, rendererType: RendererType.Roundtrip) { }
    }

    // NOTE: Beware of Copy/Pasting spec files - some characters may change (non-breaking space into space)!
    static readonly Spec[] Specs = new[]
    {
        new Spec("CommonMarkSpecs",     "CommonMark.md",                ""),
        new Spec("Alert Blocks",        "AlertBlockSpecs.md",           "advanced"),
        new Spec("Pipe Tables",         "PipeTableSpecs.md",            "pipetables|advanced"),
        new Spec("GFM Pipe Tables",     "PipeTableGfmSpecs.md",         "gfm-pipetables"),
        new Spec("Footnotes",           "FootnotesSpecs.md",            "footnotes|advanced"),
        new Spec("Generic Attributes",  "GenericAttributesSpecs.md",    "attributes|advanced"),
        new Spec("Emphasis Extra",      "EmphasisExtraSpecs.md",        "emphasisextras|advanced"),
        new Spec("Hardline Breaks",     "HardlineBreakSpecs.md",        "hardlinebreak|advanced+hardlinebreak"),
        new Spec("Grid Tables",         "GridTableSpecs.md",            "gridtables|advanced"),
        new Spec("Custom Containers",   "CustomContainerSpecs.md",      "customcontainers+attributes|advanced"),
        new Spec("Definition Lists",    "DefinitionListSpecs.md",       "definitionlists+attributes|advanced"),
        new Spec("Emoji",               "EmojiSpecs.md",                "emojis|advanced+emojis"),
        new Spec("Abbreviations",       "AbbreviationSpecs.md",         "abbreviations|advanced"),
        new Spec("List Extras",         "ListExtraSpecs.md",            "listextras|advanced"),
        new Spec("Math",                "MathSpecs.md",                 "mathematics|advanced"),
        new Spec("Bootstrap",           "BootstrapSpecs.md",            "bootstrap+pipetables+figures+attributes"),
        new Spec("Media",               "MediaSpecs.md",                "medialinks|advanced+medialinks"),
        new Spec("Smarty Pants",        "SmartyPantsSpecs.md",          "pipetables+smartypants|advanced+smartypants"),
        new Spec("Auto Identifiers",    "AutoIdentifierSpecs.md",       "autoidentifiers|advanced"),
        new Spec("Task Lists",          "TaskListSpecs.md",             "tasklists|advanced"),
        new Spec("Diagrams",            "DiagramsSpecs.md",             "diagrams|advanced"),
        new Spec("No Html",             "NoHtmlSpecs.md",               "nohtml"),
        new Spec("Yaml",                "YamlSpecs.md",                 "yaml"),
        new Spec("Auto Links",          "AutoLinks.md",                 "autolinks|advanced"),
        new Spec("Jira Links",          "JiraLinks.md",                 "jiralinks"),
        new Spec("Globalization",       "GlobalizationSpecs.md",        "globalization+advanced+emojis"),
        new Spec("Figures, Footers and Cites", "FigureFooterAndCiteSpecs.md", "figures+footers+citations|advanced"),

        new NormalizeSpec("Headings", "Headings.md", ""),

        new PlainTextSpec("Sample", "SamplePlainText.md", ""),

        new RoundtripSpec("Roundtrip", "CommonMark.md", ""),
    };

    static void Main()
    {
        int totalTests = 0;
        bool hasErrors = false;

        foreach (var spec in Specs)
        {
            if (!File.Exists(spec.Path))
            {
                EmitError("Could not find the specification file at " + spec.Path);
                hasErrors = true;
                continue;
            }

            string source = ParseSpecification(spec, File.ReadAllText(spec.Path)).Replace("\r\n", "\n", StringComparison.Ordinal);
            totalTests += spec.TestCount;

            if (File.Exists(spec.OutputPath))  // If the source hasn't changed, don't bump the generated tag
            {
                string previousSource = File.ReadAllText(spec.OutputPath).Replace("\r\n", "\n", StringComparison.Ordinal);
                if (previousSource == source && File.GetLastWriteTime(spec.OutputPath) > File.GetLastWriteTime(spec.Path))
                {
                    continue;
                }
                Console.WriteLine($"Spec changed {spec.Path}. Need to regenerate");
            }
            Console.WriteLine($"Generating spec {spec.Name} to {spec.OutputPath}...");
            File.WriteAllText(spec.OutputPath, source);
        }

        if (hasErrors)
        {
            Environment.Exit(1);
        }

        Console.WriteLine("There are {0} spec tests in total", totalTests);
    }
    static void EmitError(string error)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.WriteLine(new string('-', error.Length));
        Console.WriteLine(error);
        Console.WriteLine(new string('-', error.Length));
        Console.WriteLine();
        Console.ForegroundColor = prevColor;
    }

    static readonly StringBuilder StringBuilder = new StringBuilder(1 << 20); // 1 MB
    static void Write(string text)
    {
        StringBuilder.Append(text);
    }
    static void Line(string text = null)
    {
        if (text != null) StringBuilder.Append(text);
        StringBuilder.Append('\n');
    }
    static void Indent(int count = 1)
    {
        StringBuilder.Append(new string(' ', 4 * count));
    }

    static string ParseSpecification(Spec spec, string specSource)
    {
        Line();
        Write("// "); Line(new string('-', 32));
        Write("// "); Write(new string(' ', 16 - spec.Name.Length / 2)); Line(spec.Name);
        Write("// "); Line(new string('-', 32));
        Line();
        Line("using System;");
        Line("using NUnit.Framework;");
        Line();
        Write("namespace Markdig.Tests.Specs.");
        if      (spec.RendererType == RendererType.Normalize) Write("Normalize.");
        else if (spec.RendererType == RendererType.PlainText) Write("PlainText.");
        else if (spec.RendererType == RendererType.Roundtrip) Write("Roundtrip.");
        Line(CompressedName(spec.Name).Replace('.', '_'));
        Line("{");

        var lines = specSource.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

        bool nameChanged = true;
        string name = "";
        string compressedName = "";
        int number = 0;
        int commentOffset = 0, commentEnd = 0, markdownOffset = 0, markdownEnd = 0, htmlOffset = 0, htmlEnd = 0;
        bool first = true;
        LinkedList<(string Heading, string Compressed, int Level)> headings = new LinkedList<(string, string, int)>();
        StringBuilder nameBuilder = new StringBuilder(64);

        int i = 0;
        while (i < lines.Length)
        {
            commentOffset = commentEnd = i;
            while (!lines[i].Equals("```````````````````````````````` example", StringComparison.Ordinal))
            {
                string line = lines[i];
                if (line.Length > 2 && line[0] == '#')
                {
                    int level = line.IndexOf(' ', StringComparison.Ordinal);
                    while (headings.Count != 0)
                    {
                        if (headings.Last.Value.Level < level) break;
                        headings.RemoveLast();
                    }
                    string heading = line.Substring(level + 1);
                    headings.AddLast((heading, CompressedName(heading), level));

                    foreach (var (Heading, _, _) in headings)
                        nameBuilder.Append(Heading + " / ");
                    nameBuilder.Length -= 3;
                    name = nameBuilder.ToString();
                    nameBuilder.Length = 0;

                    foreach (var (_, Compressed, _) in headings)
                        nameBuilder.Append(Compressed);
                    compressedName = nameBuilder.ToString();
                    nameBuilder.Length = 0;

                    nameChanged = true;
                }
                i++;

                if (!IsEmpty(line))
                    commentEnd = i;

                if (i == lines.Length)
                {
                    if (commentOffset != commentEnd)
                    {
                        while (commentOffset < commentEnd && IsEmpty(lines[commentOffset])) commentOffset++;
                        for (i = commentOffset; i < commentEnd; i++)
                        {
                            Indent(2); Write("// "); Line(lines[i]);
                        }
                    }
                    goto End;
                }
            };

            markdownOffset = ++i;
            while (!(lines[i].Length == 1 && lines[i][0] == '.')) i++;
            markdownEnd = i++;

            htmlOffset = i;
            while (!lines[i].Equals("````````````````````````````````", StringComparison.Ordinal)) i++;
            htmlEnd = i++;

            if (nameChanged)
            {
                if (!first)
                {
                    Indent(); Line("}");
                    Line();
                }
                Indent(); Line("[TestFixture]");
                Indent(); Line("public class Test" + compressedName);
                Indent(); Line("{");
                first = false;
                nameChanged = false;
            }
            else Line();

            WriteTest(name, compressedName, ++number, spec.Extensions, lines,
                commentOffset, commentEnd,
                markdownOffset, markdownEnd,
                htmlOffset, htmlEnd,
                spec.RendererType);
        }

    End:
        if (!first)
        {
            Indent(); Line("}");
        }
        Line("}");

        string source = StringBuilder.ToString();
        StringBuilder.Length = 0;

        spec.TestCount = number;
        return source;
    }

    static void WriteTest(string name, string compressedName, int number, string extensions, string[] lines, int commentOffset, int commentEnd, int markdownOffset, int markdownEnd, int htmlOffset, int htmlEnd, RendererType rendererType)
    {
        if (commentOffset != commentEnd)
        {
            while (commentOffset < commentEnd && IsEmpty(lines[commentOffset])) commentOffset++;
            for (int i = commentOffset; i < commentEnd; i++)
            {
                Indent(2); Write("// "); Line(lines[i]);
            }
        }

        Indent(2); Line("[Test]");
        Indent(2); Line("public void " + compressedName + "_Example" + number.ToString().PadLeft(3, '0') + "()");
        Indent(2); Line("{");
        Indent(3); Line("// Example " + number);
        Indent(3); Line("// Section: " + name);

        Indent(3); Line("//");
        Indent(3); Line("// The following Markdown:");
        for (int i = markdownOffset; i < markdownEnd; i++)
        {
            Indent(3); Write("// "); Indent(); Line(lines[i]);
        }

        Indent(3); Line("//");
        Indent(3); Line("// Should be rendered as:");
        for (int i = htmlOffset; i < htmlEnd; i++)
        {
            Indent(3); Write("// "); Indent(); Line(lines[i]);
        }
        if (htmlOffset >= htmlEnd)
        {
            Indent(3); Write("//");
        }
        Line();
        Indent(3);
        if      (rendererType == RendererType.Html)      Write("TestParser");
        else if (rendererType == RendererType.Normalize) Write("TestNormalize");
        else if (rendererType == RendererType.PlainText) Write("TestPlainText");
        else if (rendererType == RendererType.Roundtrip) Write("TestRoundtrip");
        Write(".TestSpec(\"");
        for (int i = markdownOffset; i < markdownEnd; i++)
        {
            Write(Escape(lines[i]));
            if (i != markdownEnd - 1) Write("\\n");
        }
        Write("\", \"");
        for (int i = htmlOffset; i < htmlEnd; i++)
        {
            Write(Escape(lines[i]));
            if (i != htmlEnd - 1) Write("\\n");
        }
        Write("\", \"");
        Write(extensions);
        Line($"\", context: \"Example {number}\\nSection {name}\\n\");");

        Indent(2); Line("}");
    }
    static string Escape(string input)
    {
        return input
            .Replace("→", "\t")
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\0", "\\0")
            .Replace("\a", "\\a")
            .Replace("\b", "\\b")
            .Replace("\f", "\\f")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t")
            .Replace("\v", "\\v")
            ;
    }
    static string CompressedName(string name)
    {
        string compressedName = "";
        foreach (var part in name.Replace(',', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            compressedName += char.IsLower(part[0])
                ? char.ToUpper(part[0]) + (part.Length > 1 ? part.Substring(1) : "")
                : part;
        }
        return compressedName;
    }
    static bool IsEmpty(string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] != ' ') return false;
        }
        return true;
    }
}
