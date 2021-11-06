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
using Microsoft.AspNetCore.Authorization;


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

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        [Authorize]
        [HttpGet("{id:int}", Name = "GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                var country = await _unitOFWork.Countries.Get(c=>c.Id == id, new List<string>() {"Hotels"});
                if (country == null)
                {
                    return NotFound();
                }
                var result = _mapper.Map<CountryDTO>(country);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something wen wrong in the {nameof(GetCountry)}");
                return StatusCode(500, "Internal Server error, please try Again later."); ;
            }
        }
        
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
