using Hoteles.Contracs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hoteles.Filters.Action
{
    public class ValidateCountryExistsAttribute : IAsyncActionFilter
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly ILogger<ValidateCountryExistsAttribute> _logger;

        public ValidateCountryExistsAttribute(IUnitOFWork unitOfWork, ILogger<ValidateCountryExistsAttribute> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var trackChanges = context.HttpContext.Request.Method.Equals("PUT");
            var id = (int)context.ActionArguments["id"];
            var country = await _unitOfWork.Countries.Get(c => c.Id == id);

            if (country == null)
            {
                _logger.LogError($"country with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("country", country);
                await next();
            }
        }
    }
}
