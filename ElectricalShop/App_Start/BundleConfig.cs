﻿using System.Web;
using System.Web.Optimization;

namespace ElectricalShop
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                      "~/Scripts/js/site.js",
                      "~/Scripts/js/megamenu.js",
                      "~/Scripts/js/simpleCart.min.js",
                      "~/Scripts/js/jquery.easydropdown.js"));

            bundles.Add(new ScriptBundle("~/bundles/search-scripts").Include(
                      "~/Scripts/js/classie1.js",
                      "~/Scripts/js/uisearch.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap.css",
                      "~/Content/css/megamenu.css",
                      "~/Content/site.css",
                      "~/Content/css/style.css"));

            // ADMIN BUNDLES
            bundles.Add(new StyleBundle("~/Content/css-admin").Include(
                      "~/Content/admin/bootstrap/css/bootstrap.min.css",
                      "~/Content/site.css",
                      "~/Content/admin/css/styles.css"));

            bundles.Add(new ScriptBundle("~/bundles/scripts-admin").Include(
                      "~/Scripts/admin/admin.js",
                      "~/Scripts/admin/custom.js"));

        }
    }
}
