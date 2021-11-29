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
    public class ValidateHotelExistsAttribute : IAsyncActionFilter
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly ILogger<ValidateCountryExistsAttribute> _logger;

        public ValidateHotelExistsAttribute(IUnitOFWork unitOfWork, ILogger<ValidateCountryExistsAttribute> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var trackChanges = context.HttpContext.Request.Method.Equals("PUT");
            var id = (int)context.ActionArguments["id"];
            var hotel = await _unitOfWork.Hotels.Get(h => h.Id == id);

            if (hotel == null)
            {
                _logger.LogError($"country with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("hotel", hotel);
                await next();
            }
        }
    }
}
