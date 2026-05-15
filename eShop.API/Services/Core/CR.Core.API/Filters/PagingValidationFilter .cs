// CR.API/Filters/PagingValidationFilter.cs
using CR.Constants.ErrorCodes;
using CR.DtoBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace CR.API.Filters;

/// <summary>
/// Filter tự động validate PagingRequestBaseDto trên mọi action.
/// Đăng ký 1 lần trong Program.cs thay vì validate thủ công từng service.
/// </summary>
public class PagingValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg is not PagingRequestBaseDto paging) continue;

            var (isValid, error) = paging.Validate();
            if (!isValid)
            {
                context.Result = new BadRequestObjectResult(
                    Result.Failure(ErrorCode.PAGING_INVALID, this.GetType().Name + ": " + error));
                return;
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}