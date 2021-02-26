using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Library.Common
{
    [DefaultStatusCode(DefaultStatusCode)]
    public class SeeOtherStatusCode : StatusCodeResult
    {
        private const int DefaultStatusCode = StatusCodes.Status303SeeOther;
        
        public IUrlHelper UrlHelper { get; set; }
        
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
        
        public SeeOtherStatusCode(
            string actionName,
            string controllerName,
            object routeValues)
            : base(DefaultStatusCode)
        {
            ActionName = actionName;
            ControllerName = controllerName;
            RouteValues = routeValues == null ? null : new RouteValueDictionary(routeValues);
        }

        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            base.ExecuteResult(context);

            var request = context.HttpContext.Request;

            var urlHelper = UrlHelper;
            if (urlHelper == null)
            {
                var services = context.HttpContext.RequestServices;
                urlHelper = services.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(context);
            }

            var url = urlHelper.Action(
                ActionName,
                ControllerName,
                RouteValues,
                request.Scheme,
                request.Host.ToUriComponent());

            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("No resources matched specified URL");

            context.HttpContext.Response.Headers[HeaderNames.Location] = url;
        }
    }
}