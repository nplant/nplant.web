using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using NPlant.Generation.ClassDiagraming;
using NPlant.Samples;
using NPlant.Web.Models.Samples;

namespace NPlant.Web.Services
{
    public class CompilationService : MarshalByRefObject
    {
        private Assembly _assembly;
        private readonly CSharpCodeProvider _codeProvider = new CSharpCodeProvider();
        private bool _successful;
        private string _message;
        private CompileError[] _compilationErrors;

        public bool Successful
        {
            get { return _successful; }
        }

        public string Message
        {
            get { return _message; }
        }

        public CompileError[] CompilationErrors
        {
            get { return _compilationErrors; }
        }

        public bool Compile(string code)
        {
            try
            {
                CompilerParameters parameters = CrateCompilerParameters();
                CompilerResults results = _codeProvider.CompileAssemblyFromSource(parameters, code);

                if (results.Errors != null && results.Errors.HasErrors)
                {
                    _successful = false;
                    _message = "Compilation Failed";
                    _compilationErrors = BuildErrors(results.Errors);
                }
                else
                {
                    _successful = true;
                    _message = "Successful compilation";
                    _assembly = results.CompiledAssembly;
                }
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                    throw;

                _successful = false;
                _message = "Unhandled exception occurred of type {0} was thrown while compiling the code - Message: {1}".FormatWith(ex.GetType().FullName, ex.Message);
            }

            return _assembly != null;
        }

        public string Run()
        {
            try
            {
                var diagramTypes = _assembly.GetTypes().Where(x => typeof (ClassDiagram).IsAssignableFrom(x)).ToArray();

                if (diagramTypes.Length != 1)
                {
                    _successful = false;
                    _message = "Malformed Source Code - Exactly one diagram class was expected, but multiple or none were found";
                }

                var diagram = (ClassDiagram) Activator.CreateInstance(diagramTypes[0]);

                string notation = BufferedClassDiagramGenerator.GetDiagramText(diagram);
                
                _successful = true;
                _message = "Code was compiled and executed successfully";

                return notation;
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                    throw;

                _successful = false;
                _message = "Unhandled exception occurred of type {0} was thrown while running the code - Message: {1}".FormatWith(ex.GetType().FullName, ex.Message);

                return null;
            }
        }

        private static CompilerParameters CrateCompilerParameters()
        {
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };

            parameters.ReferencedAssemblies.Add("system.dll");
            parameters.ReferencedAssemblies.Add(typeof(ClassDiagram).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(SamplesRoot).Assembly.Location);

            return parameters;
        }

        private CompileError[] BuildErrors(CompilerErrorCollection errors)
        {
            var result = errors.Cast<CompilerError>().Select(error => new CompileError
            {
                Column = error.Column,
                Line = error.Line,
                IsWarning = error.IsWarning,
                ErrorNumber = error.ErrorNumber,
                ErrorText = error.ErrorText
            });

            return result.ToArray();
        }
    }
}