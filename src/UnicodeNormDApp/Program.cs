using System.Diagnostics;
using System.Text;

namespace UnicodeNormDApp;

class Program
{
    static async Task Main(string[] args)
    {
        using var httpClient = new HttpClient();
        var data = await httpClient.GetStringAsync("http://www.unicode.org/Public/UCD/latest/ucd/NormalizationTest.txt");

        var stringReader = new StringReader(data);

        var sep = new char[] {';'};
        var spaceSpec = new char[] {' '};
        string line;
        int count = 0;
        int min = int.MaxValue;
        int max = int.MinValue;
        var values = new Dictionary<char, string>();
        var builder = new StringBuilder();
        while ((line = stringReader.ReadLine()) != null)
        {
            if (line.StartsWith("#") || line.StartsWith("@"))
            {
                continue;
            }
            var commentIndex = line.IndexOf('#');
            var dataLine = commentIndex > 0 ? line.Substring(0, commentIndex) : line;

            var columns = dataLine.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            if (columns.Length < 4)
            {
                continue;
            }

            // Skip multi code point
            if (columns[0].IndexOf(' ') > 0)
            {
                continue;
            }

            var source = Convert.ToInt32(columns[0], 16);
            if (source < min)
            {
                min = source;
            }
            if (source > max)
            {
                max = source;
            }

            var column4Space = columns[4].Split(spaceSpec, StringSplitOptions.RemoveEmptyEntries);
            builder.Clear();
            for (int i = 0; i < column4Space.Length; i++)
            {
                var nfdFirst = Convert.ToInt32(column4Space[i], 16);
                // We support only single char codepoints
                string unicodeString = char.ConvertFromUtf32(nfdFirst);
                // We restrict to ascii only
                if (unicodeString.Length == 1 && nfdFirst > 32 && nfdFirst < 127)
                {
                    builder.Append(unicodeString[0]);
                }
            }
            var str = builder.ToString();
            var sourceString = char.ConvertFromUtf32(source);
            // We don't keep spaces
            if (sourceString.Length == 1 && str.Length > 0 && !values.ContainsKey(sourceString[0]))
            {
                //Trace.WriteLine(columns[0] + "/" + source + ": " + char.ConvertFromUtf32(source) + " => " + (char)nfdFirst);
                count++;
                values.Add(sourceString[0], str);
            }
        }

        //var newValues = new Dictionary<int, char>(values.Count)
        //{
        //    {15, 'a'}
        //}.ToFrozenDictionary();
        Trace.WriteLine($"CodeToAscii = new Dictionary<char, string>({values.Count})");
        Trace.WriteLine("{");
        foreach (var pair in values)
        {
            var escape = pair.Value.Replace("\\", @"\\").Replace("\"", "\\\"");
            Trace.WriteLine($"    {{'{pair.Key}',\"{escape}\"}},");
        }
        Trace.WriteLine("}.ToFrozenDictionary();");

        //Trace.WriteLine("count: " + count);
        //Trace.WriteLine("max: " + max);
        //Trace.WriteLine("min: " + min);
    }

}
