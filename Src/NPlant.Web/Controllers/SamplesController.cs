using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.CSharp;
using NPlant.Generation.ClassDiagraming;
using NPlant.Samples;
using NPlant.Web.Models.Samples;
using NPlant.Web.Services;

namespace NPlant.Web.Controllers
{
    public class SamplesController : Controller
    {
        public ActionResult Default()
        {
            var model = ManifestReader.GetSamplesListModel();

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Compile(string code)
        {
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateExecutable = false, GenerateInMemory = true
            };

            parameters.ReferencedAssemblies.Add(typeof(ClassDiagram).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(SamplesRoot).Assembly.Location);

            code = "using NPlant.Samples;" + Environment.NewLine + code;
            
            var results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors != null && results.Errors.HasErrors)
            {
                return Json(new CompileResult
                {
                    Successful = false,
                    Errors = BuildErrors(results.Errors)
                });
                
            }

            //TODO:  need to add error handling here to disallow anything about exactly 1 diagram
            var diagramType = results.CompiledAssembly.GetTypes().FirstOrDefault(x => typeof(ClassDiagram).IsAssignableFrom(x));

            var diagram = (ClassDiagram)Activator.CreateInstance(diagramType);
            var text = BufferedClassDiagramGenerator.GetDiagramText(diagram);

            var url = new PlantUmlUrl(text);
            
            return Json(new CompileResult
            {
                Successful = true,
                Url = url.GetUrl()
            });
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

        [HttpGet]
        public JsonResult Source(string src)
        {
            return Json(new { src = ManifestReader.GetSource(src) }, JsonRequestBehavior.AllowGet);
        }
    }
}