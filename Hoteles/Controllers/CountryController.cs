using AutoMapper;
using Hoteles.Contracs;
using Hoteles.Data.DTOs;
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
    public class CountryController : ControllerBase
    {
        private readonly IUnitOFWork _unitOFWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOFWork unitOFWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOFWork = unitOFWork;
            _logger = logger;
            _mapper = mapper;
                
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                var countries = await _unitOFWork.Countries.GetAll();
                var results = _mapper.Map<IList<CountryDTO>>(countries);
                return Ok(results);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something wen wrong in the {nameof(GetCountries)}");
                return StatusCode(500, "Internal Server error, please try Again later.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                var country = await _unitOFWork.Countries.Get(c=>c.Id == id, new List<string>() {"Hotels"});
                var result = _mapper.Map<CountryDTO>(country);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something wen wrong in the {nameof(GetCountry)}");
                return StatusCode(500, "Internal Server error, please try Again later."); ;
            }
        }
    }
}
