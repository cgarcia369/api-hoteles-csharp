using AutoMapper;
using Hoteles.Contracs;
using Hoteles.Data.DTOs;
using Hoteles.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hoteles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    public class CountryV2Controller : ControllerBase
    {
        private readonly IUnitOFWork _unitOFWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryV2Controller(IUnitOFWork unitOFWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOFWork = unitOFWork;
            _logger = logger;
            _mapper = mapper;

        }

        [Authorize(Roles = "User")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
        {

            var countries = await _unitOFWork.Countries.GetPagedList(requestParams);
            var results = _mapper.Map<IList<CountryDTO>>(countries);
            return Ok(results);


        }
    }
}
