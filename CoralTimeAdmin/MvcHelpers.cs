using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoralTimeAdmin
{
    public static class MvcHelpers
    {
        public static MvcHtmlString ToParagraphs(this HtmlHelper html,
            string value)
        {
            value = html.Encode(value).Replace("\r", string.Empty);
            var arr = value.Split('\n').Where(a => a.Trim() != string.Empty);
            var htmlStr = "<p>" + string.Join("</p><p>", arr) + "</p>";
            return MvcHtmlString.Create(htmlStr);
        }
    }
}