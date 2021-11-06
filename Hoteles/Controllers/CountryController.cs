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
using Hoteles.Data.Models;


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

        [HttpGet("{id:int}", Name = "GetCountry")]
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
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCountryDTO countryDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attemp in {nameof(CreateCountryDTO)}");
                return BadRequest(ModelState);
            }
            try
            {
                var country = _mapper.Map<Country>(countryDto);
                await _unitOFWork.Countries.Insert(country);
                await _unitOFWork.Save();
                return CreatedAtRoute("GetCountry", new { id = country.Id}, country);
            }
            catch (Exception e)
            {

                _logger.LogError(e, $"Something Wen Wrong in the {nameof(CreateCountryDTO)}");
                return StatusCode(500, "Internal server Error");
            }
        }
    }
}
