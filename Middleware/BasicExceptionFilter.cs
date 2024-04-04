using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AssetLayer.SDK.Basic;

public class BasicExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is BasicError apiEx)
        {
            context.Result = new ObjectResult(apiEx.Message) {
                StatusCode = apiEx.Status
            };
            context.ExceptionHandled = true;
        }
    }
}