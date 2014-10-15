using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CafeProject.MobileWebApplication
{
    public static class ControllerExtensions
    {
        public static ContentResult HttpBadRequest(this Controller controller)
        {
            HttpContext.Current.Response.StatusCode = 400;
            ContentResult result = new ContentResult();
            result.Content = "HTTP 400. Bad request.";
            result.ContentType = "text/html";
            result.ContentEncoding = System.Text.Encoding.UTF8;

            return result;
        }

        public static ContentResult HttpBadRequest(this Controller controller, string message)
        {
            HttpContext.Current.Response.StatusCode = 400;
            ContentResult result = new ContentResult();
            result.Content = message;
            result.ContentType = "text/html";
            result.ContentEncoding = System.Text.Encoding.UTF8;

            return result;
        }
    }
}