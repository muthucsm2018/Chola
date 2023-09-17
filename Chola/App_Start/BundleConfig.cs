using System.Web;
using System.Web.Optimization;

namespace Chola
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
             bundles.Add(new ScriptBundle("~/assetsjs").Include(
                      "~/plugins/jQuery/jQuery-2.1.4.min.js",
                      "~/plugins/jQueryUI/jquery-ui.js",
                      "~/vendor/bootstrap/js/bootstrap.min.js",
                      "~/plugins/slimScroll/jquery.slimscroll.min.js",
                      "~/assets/js/app.min.js",
                      "~/plugins/datepicker/bootstrap-datepicker.js",
                      "~/plugins/daterangepicker/moment.min.js",
                      "~/plugins/daterangepicker/daterangepicker.js",
                      "~/plugins/input-mask/jquery.inputmask.js",
                      "~/plugins/input-mask/jquery.inputmask.date.extensions.js",
                      "~/plugins/input-mask/jquery.inputmask.extensions.js",
                      "~/plugins/iCheck/icheck.min.js",
                      "~/assets/js/jquery-1.12.0.min.js"
            ));

            bundles.Add(new ScriptBundle("~/Datatablesjs").Include(
                    "~/plugins/datatables/jquery.dataTables.min.js",
                    "~/plugins/datatables/dataTables.bootstrap.min.js",
                    "~/assets/plugins/datatables/dataTables.buttons.min.js",
                    "~/assets/plugins/datatables/buttons.flash.min.js",
                    "~/assets/plugins/datatables/jszip.min.js",
                    "~/assets/plugins/datatables/pdfmake.min.js",
                    "~/assets/plugins/datatables/vfs_fonts.js",
                    "~/assets/plugins/datatables/buttons.html5.min.js",
                    "~/assets/plugins/datatables/buttons.print.min.js",
                    "~/vendor/footable/js/footable.js"
            ));          

            bundles.Add(new Bundle("~/assets/css/css").Include(
                      "~/vendor/bootstrap/css/bootstrap.min.css",
                      "~/assets/css/font-awesome.min.css",
                      "~/assets/css/icons.css",
                      "~/plugins/datatables/dataTables.bootstrap.css",
                      "~/vendor/footable/css/footable.bootstrap.min.css",
                      "~/plugins/datepicker/datepicker3.css",
                      "~/plugins/daterangepicker/daterangepicker-bs3.css",
                      "~/assets/css/skin-blue.min.css",
                      "~/assets/css/CSAdmin.min.css",
                      "~/assets/css/mystyle.css",
                      "~/assets/css/jqueryui.css",
                      "~/Content/Custom.css",
                      "~/plugins/iCheck/square/blue.css",
                      "~/plugins/iCheck/icheck.min.js"));
        }
        
    }
}
