using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ToolsManagementSystem.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/css/bootstrap.css"
                      , "~/Content/css/bootstrap-datetimepicker.min.css"
                      , "~/Content/css/bootstrap.min.css"
                      , "~/Content/css/custom.css"
                      , "~/Content/css/customize.css"
                      , "~/Content/css/select2.min.css"
                      , "~/Content/css/select2-bootstrap.min.css"
                      , "~/Content/css/animate.css"
                      , "~/Content/css/bootstrap-table.css"
                      , "~/Content/fonts/ionicons.min.css"
                      , "~/Content/fonts/font-awesome.min.css"
                      , "~/Content/css/jquery.dataTables.css"
                      , "~/Content/css/bootstrap-table-filter-control.css"
                      , "~/Content/css/jquery-ui.css"

                      ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                         "~/Content/scripts/jquery.js"
                        , "~/Content/scripts/bootstrap.min.js"
                        , "~/Content/scripts/select2.min.js"
                        , "~/Content/scripts/select2.full.js"
                        , "~/Content/scripts/bootstrap-notify.js"
                        , "~/Content/scripts/bootstrap-notify.min.js"
                        , "~/Content/scripts/bootstrap-table.js"
                        , "~/Content/scripts/sorttable.js"
                        , "~/Content/scripts/jquery.dataTables.js"
                        , "~/Content/scripts/bootstrap-table-filter-control.js"
                        , "~/Content/scripts/jquery-ui.js"
                    
                        ));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            "~/Content/scripts/modernizr-2.6.2.js"));




            
        }
    }
}