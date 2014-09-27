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
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            };

            _localAppDomain = AppDomain.CreateDomain(appDomain, null, domainSetup);
 
            DumpAssemblies();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

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

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                Assembly assembly = Assembly.Load(args.Name);
                if (assembly != null)
                    return assembly;
            }
            catch (ReflectionTypeLoadException)
            {
                // ignore load error 
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                    throw;
                // ignore load error 
            }

            string[] parts = args.Name.Split(',');
            string file = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + parts[0].Trim() + ".dll";

            return Assembly.LoadFrom(file);
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