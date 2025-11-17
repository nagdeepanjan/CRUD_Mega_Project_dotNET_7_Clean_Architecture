using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_Last.Filters.ResultFilter
{
    public class TokenResultFilter : IResultFilter
    {

        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Cookies.Append("Auth-Key", "A100");
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            return; //throw new NotImplementedException();
        }

        
    }
}
