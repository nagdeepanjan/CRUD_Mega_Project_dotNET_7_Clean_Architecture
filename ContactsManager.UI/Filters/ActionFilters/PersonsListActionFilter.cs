using CRUD_Last.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CRUD_Last.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private ILogger<PersonsListActionFilter> _iLogger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> iLogger)
        {
            _iLogger = iLogger;
        }
        
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _iLogger.LogWarning("This is an action executing filter");

            //Checking model validation
            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Country),
                        nameof(PersonResponse.Address),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth)
                    };

                    if (searchByOptions.Any(s => s == searchBy)==false)
                    {
                        _iLogger.LogInformation($"Searching by {searchBy}");
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                    }
                }
            }

            //To be used in OnActionExecuted action filter method later
            context.HttpContext.Items["arguments"] = context.ActionArguments;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _iLogger.LogWarning("This is an action executed filter");

            var personsController = (PersonsController)context.Controller;
            IDictionary<string, object?>? parameters = (IDictionary<string, object?> ?) context.HttpContext.Items["arguments"];
            if (parameters is not null)
            {
                if (parameters.ContainsKey("searchBy"))
                    personsController.ViewData["searchBy"]= parameters["searchBy"];

                if (parameters.ContainsKey("searchString"))
                    personsController.ViewData["searchString"] = parameters["searchString"];
                
            }

        }
    }
}
