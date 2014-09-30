using System;
using NPlant.Web.Models.Samples;

namespace NPlant.Web.Services
{
    public interface ICompilationService : IDisposable
    {
        bool Successful { get; }
        string Message { get; }
        CompileError[] CompilationErrors { get; }
        bool Compile(string code);
        string Run();
        int AppDomainId { get; }
    }
}