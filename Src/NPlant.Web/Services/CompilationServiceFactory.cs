using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NPlant.Web.Services
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class CompilationServiceFactory : MarshalByRefObject, IDisposable
    {
        private AppDomain _localAppDomain;
        
        public CompilationService Create()
        {
            string appDomain = "CompilationServiceAppDomain" + Guid.NewGuid().ToString().GetHashCode().ToString("x");

            AppDomainSetup domainSetup = new AppDomainSetup
            {
                ApplicationName = appDomain,
                ApplicationBase = Path.GetDirectoryName(new Uri(typeof(WebRoot).Assembly.CodeBase).LocalPath)
            };

            _localAppDomain = AppDomain.CreateDomain(appDomain, null, domainSetup);
 

            string path = new Uri(typeof(CompilationService).Assembly.CodeBase).LocalPath;

            var instance = (CompilationService)_localAppDomain.CreateInstanceFrom(path, typeof(CompilationService).FullName).Unwrap();

            DumpAssemblies();

            return instance;
        }

        private void DumpAssemblies()
        {
            var assemblies = _localAppDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                Debug.WriteLine(assembly.GetName().FullName);
            }
        }

        public void Dispose()
        {
            if (_localAppDomain != null)
            {
                AppDomain.Unload(_localAppDomain);
                _localAppDomain = null;
            }            
        }
    }
}