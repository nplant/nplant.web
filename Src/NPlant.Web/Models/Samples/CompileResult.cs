using System;

namespace NPlant.Web.Models.Samples
{
    [Serializable]
    public class CompileResult
    {
        public bool Successful { get; set; }
        public CompileError[] CompilationErrors { get; set; }
        public string Url { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public class CompileError
    {
        public int Column { get; set; }
        public int Line { get; set; }
        public bool IsWarning { get; set; }
        public string ErrorNumber { get; set; }
        public string ErrorText { get; set; }
    }
}