using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public static class HTMLExtensions
    {
        private static readonly HtmlContentBuilder _emptyBuilder = new HtmlContentBuilder();

        public static IHtmlContent BuildBreadcrumbNavigation(this IHtmlHelper helper)
        {            string controllerName = helper.ViewContext.RouteData.Values["controller"].ToString();
            string actionName = helper.ViewContext.RouteData.Values["action"].ToString();

            var breadcrumb = new HtmlContentBuilder()
                                .AppendHtml(helper.ActionLink("Home", "Index", "Home", null, new { @class = "breadcrumb" }))
                                .AppendHtml(helper.ActionLink(Titleize(controllerName),
                                                          "Index", controllerName, null, new { @class = "breadcrumb" }));


            if (helper.ViewContext.RouteData.Values["action"].ToString() != "Index")
            {
                breadcrumb.AppendHtml(helper.ActionLink(Titleize(actionName), actionName, controllerName, null, new { @class = "breadcrumb" }));
            }

            return breadcrumb.AppendHtml("");
        }

        private static string Titleize(string s)
        {
            return s;
        }
    }
}
