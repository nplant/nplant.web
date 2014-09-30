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

            using (var guard = new DiagramRunGuard())
            {
                CompileResult model;
            
                using (var scope = guard.CreateScope())
                {
                    if (scope.Compile(code))
                    {
                        notation = scope.Run();
                    }

                    model = new CompileResult
                    {
                        Successful = scope.Successful,
                        Message = scope.Message,
                        CompilationErrors = scope.CompilationErrors
                    };
                }

                string url = null;

                if(model.Successful && !notation.IsNullOrEmpty())
                    url = new PlantUmlUrl(notation).GetUrl();

                model.Url = url;

                return Json(model);
            }
        }

        [HttpGet]
        public JsonResult Source(string src)
        {
            return Json(new { src = ManifestReader.GetSource(src) }, JsonRequestBehavior.AllowGet);
        }
    }
}