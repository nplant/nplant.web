using System;
using System.Collections.Generic;
using System.IO;

namespace NPlant.Web.Services
{
    public class DiagramRunGuard : IDisposable
    {
        private readonly List<string> _assemblyPaths = new List<string>();

        public void Dispose()
        {
            foreach (var path in _assemblyPaths)
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        public void Add(string assemblyPath)
        {
            _assemblyPaths.Add(assemblyPath);
        }

        public DiagramRunScope CreateScope()
        {
            return new DiagramRunScope(this);
        }
    }
}