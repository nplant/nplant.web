using System;
using System.IO;
using NPlant.Web.Models.Samples;

namespace NPlant.Web.Services
{
    public class DiagramRunScope : MarshalByRefObject, ICompilationService
    {
        private AppDomain _localAppDomain;
        private CompilationService _service;
        private readonly DiagramRunGuard _guard;

        public DiagramRunScope(DiagramRunGuard guard)
        {
            _guard = guard;
            string appDomain = "CompilationServiceAppDomain" + Guid.NewGuid().ToString().GetHashCode().ToString("x");

            var domainSetup = new AppDomainSetup
            {
                ApplicationName = appDomain,
                ApplicationBase = Path.GetDirectoryName(new Uri(typeof(WebRoot).Assembly.CodeBase).LocalPath)
            };

            _localAppDomain = AppDomain.CreateDomain(appDomain, null, domainSetup);
 
            string path = new Uri(typeof(CompilationService).Assembly.CodeBase).LocalPath;

            _service = (CompilationService)_localAppDomain.CreateInstanceFrom(path, typeof(CompilationService).FullName).Unwrap();
        }

        public void Dispose()
        {
            if (_service != null)
            {
                _service.Dispose();
                _service = null;
            }

            if (_localAppDomain != null)
            {
                AppDomain.Unload(_localAppDomain);
                _localAppDomain = null;
            }
        }

        public bool Successful { get { return _service.Successful; } }
        public string Message { get { return _service.Message; } }
        public CompileError[] CompilationErrors { get { return _service.CompilationErrors; } }

        public bool Compile(string code)
        {
            var compile = _service.Compile(code);

            if(compile)
                _guard.Add(_service.AssemblyPath);

            return compile;
        }

        public string Run()
        {
            return _service.Run();
        }

        public int AppDomainId { get { return _service.AppDomainId; } }
    }
}