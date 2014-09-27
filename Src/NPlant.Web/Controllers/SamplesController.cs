using System;
using System.Web.Mvc;
using NPlant.Web.Models.Samples;
using NPlant.Web.Services;

namespace NPlant.Web.Controllers
{
    [Serializable]
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
            string notation = null;
            CompileResult model;

            using (var factory = new CompilationServiceFactory())
            {
                var compilationService = factory.Create();

                if (compilationService.Compile(code))
                {
                    notation = compilationService.Run();
                }

                model = new CompileResult
                {
                    Successful = compilationService.Successful,
                    Message = compilationService.Message,
                    CompilationErrors = compilationService.CompilationErrors
                };
            }

            string url = null;

            if(model.Successful && !notation.IsNullOrEmpty())
                url = new PlantUmlUrl(notation).GetUrl();

            model.Url = url;

            return Json(model);
        }

        [HttpGet]
        public JsonResult Source(string src)
        {
            return Json(new { src = ManifestReader.GetSource(src) }, JsonRequestBehavior.AllowGet);
        }
    }
}