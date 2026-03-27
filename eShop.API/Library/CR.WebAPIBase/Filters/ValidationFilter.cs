using CR.WebAPIBase.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CR.WebAPIBase.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
            var response = new ApiResponse<object>
            {
                Success = false,
                Errors = errors,
                StatusCode = 422
            };
            context.Result = new UnprocessableEntityObjectResult(response);
        }
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        
    }
}