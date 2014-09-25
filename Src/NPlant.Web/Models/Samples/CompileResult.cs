namespace NPlant.Web.Models.Samples
{
    public class CompileResult
    {
        public bool Successful { get; set; }
        public CompileError[] Errors { get; set; }
        public string Url { get; set; }
    }

    public class CompileError
    {
        public int Column { get; set; }
        public int Line { get; set; }
        public bool IsWarning { get; set; }
        public string ErrorNumber { get; set; }
        public string ErrorText { get; set; }
    }
}