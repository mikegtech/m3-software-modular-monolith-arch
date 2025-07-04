using System;
using System.Reflection;

namespace M3.Net.Modules.Transcripts.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
