using System.Web.Mvc;

namespace NPlant.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Default()
        {
            return View();
        }
    }
}