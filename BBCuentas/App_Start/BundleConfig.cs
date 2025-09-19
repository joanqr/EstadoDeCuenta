using System.Web;
using System.Web.Optimization;

namespace MVCSendNotificationKWIC
{
    public class BundleConfig
    {
        
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/selectize").Include(
                      "~/Scripts/selectize.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/loadingoverlay").Include(
                      "~/Scripts/loadingoverlay.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-toggle").Include(
                      "~/Scripts/bootstrap-toggle.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/richtest").Include(
                      "~/Scripts/jquery.richtext.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/swal").Include(
                      "~/Scripts/sweetalert2.all.min.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/selectize.bootstrap3.css",
                      "~/Content/bootstrap-toggle.min.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/richtext.min.css",
                      "~/Content/sweetalert2.min.css"));
        }
    }
}
