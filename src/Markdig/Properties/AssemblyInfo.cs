using System.Resources;
using System.Reflection;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Markdig")]
[assembly: AssemblyDescription("A fast, powerfull, CommonMark compliant, extensible Markdown processor for .NET")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Markdig")]
[assembly: AssemblyCopyright("Copyright © 2016 - Alexandre MUTEL")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

[assembly: AssemblyVersion(Markdig.Markdown.Version)]
[assembly: AssemblyFileVersion(Markdig.Markdown.Version)]

namespace Markdig
{
    public static partial class Markdown
    {
        public const string Version = "0.3.2";
    }
}