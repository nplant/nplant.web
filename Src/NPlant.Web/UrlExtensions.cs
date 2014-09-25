using System.Web.Mvc;

namespace NPlant.Web
{
    public static class UrlExtensions
    {
        public static string SamplesCompile(this UrlHelper helper)
        {
            return helper.RouteUrl("Default", new {controller = "Samples", action = "Compile"});
        }

        public static string SamplesSource(this UrlHelper helper, string id)
        {
            return helper.RouteUrl("Default", new { controller = "Samples", action = "Source", src = id });
        }
    }
}