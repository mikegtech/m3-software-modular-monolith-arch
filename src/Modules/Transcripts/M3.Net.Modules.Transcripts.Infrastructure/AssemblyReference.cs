using System.Reflection;

namespace M3.Net.Modules.Transcripts.Infrastructure;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
