using System.Web;
using System.Web.Optimization;

namespace CV.UI.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //BundleTable.EnableOptimizations = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/library/Scripts/Componentes/jquery-{version}.js",
                        "~/library/Scripts/Componentes/jquery.inputmask.js",
                        "~/library/Scripts/Componentes/jquery.inputmask.extensions.js",
                        "~/library/Scripts/Componentes/jquery.inputmask.numeric.extensions.js",
                        "~/library/Scripts/Componentes/jquery.inputmask.date.extensions.js",
                        "~/library/Scripts/Componentes/jquery-ui.min.js",
                        "~/library/Scripts/angular/moment-with-locales.min.js",
                        "~/library/Scripts/Componentes/jquery-ui.min.js",
                        "~/library/Scripts/Componentes/jquery.signalR-2.0.0.js",
                        "~/library/Scripts/Componentes/load-image.all.min.js",
                        "~/library/Scripts/Componentes/VideoUploader.js",
                        "~/library/Scripts/Componentes/jszip.js",
                         "~/library/Scripts/Componentes/slick.js",
                         "~/library/Scripts/Componentes/csswatch.js",
                         "~/library/Scripts/Componentes/fullcalendar.js"

                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/library/Scripts/Componentes/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                    "~/library/Scripts/app.js",
                    "~/library/Scripts/app/app-module.js",
                    "~/library/Scripts/app/app-route.js",
                    "~/library/Scripts/app/Home/home-module.js",
                   "~/library/Scripts/app/Home/SignalR-factory.js",
                    "~/library/Scripts/app/Home/home-controller.js",
                    "~/library/Scripts/app/Home/home-routes.js",
                    "~/library/Scripts/app/Home/auth-factory.js",
                    "~/library/Scripts/app/Home/dominio-factory.js",
                    "~/library/Scripts/app/Home/account-controler.js",
                    "~/library/Scripts/app/Home/login-controller.js",
                    "~/library/Scripts/app/Home/alteraSenha-controller.js",
                    "~/library/Scripts/app/Sistema/Sistema-module.js",
                    "~/library/Scripts/app/Sistema/Sistema-routes.js",
                    "~/library/Scripts/app/Directive/error-manager-directive.js",
                    "~/library/Scripts/app/error-factory.js",
                     "~/library/Scripts/app/Directive/tradutor-directive.js",
                     "~/library/Scripts/app/Directive/on-long-press-directive.js",
                    "~/library/Scripts/app/Directive/inputMask-directive.js",
                    "~/library/Scripts/app/Directive/emptyToNullDirective.js")
                    );

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/library/Scripts/Componentes/bootstrap.js",
                      "~/library/Scripts/Componentes/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                    "~/library/Scripts/angular/angular.min.js",
                     "~/library/Scripts/angular/angular-sanitize.min.js",
                     "~/library/Scripts/angular/angular-resource.min.js",
                    "~/library/Scripts/angular/angular-cookies.min.js",
                    "~/library/Scripts/angular/angular-ui-router.min.js",
                    "~/library/Scripts/angular/angular-animate.min.js",
                    "~/library/Scripts/angular/ui-bootstrap-tpls.min.js",
                    "~/library/Scripts/angular/ocLazyLoad.min.js",
                    "~/library/Scripts/angular/angular-moment.min.js",
                    "~/library/Scripts/angular/datetimepicker.js",
                    "~/library/Scripts/angular/ui-grid.min.js",
                    "~/library/Scripts/angular/angular-translate.min.js",
                    "~/library/Scripts/angular/sortable.js",
                    "~/library/Scripts/angular/ivh-treeview.min.js",
                    "~/library/Scripts/angular/ng-file-upload-all.min.js",
                    "~/library/Scripts/angular/angular-file-saver.bundle.min.js",
                    "~/library/Scripts/angular/ngContextMenu.js",
                    "~/library/Scripts/angular/ng-google-chart.js",
                    "~/library/Scripts/angular/calendar.js",
                    "~/library/Scripts/angular/ngInfiniteScroll.js",
                    "~/library/Scripts/angular/angular-multi-select-tree-0.1.0.js",
                    "~/library/Scripts/angular/angular-multi-select-tree-0.1.0.tpl.js",
                    "~/library/Scripts/angular/ng-map.min.js",
                    "~/library/Scripts/angular/angular-slick.js",
                    "~/library/Scripts/angular/ng-image-gallery.js"
                   )
                  );



            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/library/css/font-awesome.css", new CssRewriteUrlTransform())
                .Include("~/library/css/famfamfam-flags.css", new CssRewriteUrlTransform())
                .Include("~/library/css/bootstrap.min.css", new CssRewriteUrlTransform())
                 .Include("~/library/css/ui-grid.min.css", new CssRewriteUrlTransform())
                .Include(
                      "~/library/css/site.css",
                      "~/library/css/datetimepicker.css",
                      "~/library/css/skins/skin-blue-light.css",
                      "~/library/css/ivh-treeview.min.css",
                      "~/library/css/ivh-treeview-theme-basic.css",
                      "~/library/css/bootstrap-tagsinput.css",
                      "~/library/css/fullcalendar.min.css",
                      "~/library/css/angular-multi-select-tree-0.1.0.css",
                      "~/library/css/slick.css",
                      "~/library/css/slick-theme.css",
                      "~/library/css/ng-image-gallery.css"

                      )

            );

        }
    }
}