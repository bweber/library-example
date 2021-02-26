using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;

namespace Library.Common.Extensions
{
    public class AcceptanceOnlyAttribute : ActionFilterAttribute 
    {
        private readonly IHostEnvironment _environment;

        public AcceptanceOnlyAttribute(IHostEnvironment environment)
        {
            _environment = environment;
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_environment.IsAcceptance()) return;
            
            context.Result = new NotFoundResult();
        }
    }
}