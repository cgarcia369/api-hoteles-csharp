using AutoMapper;
using Hoteles.Contracs;
using Hoteles.Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoteles.Data.Models;
using Microsoft.AspNetCore.Authorization;


namespace Hoteles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOFWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
                
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _unitOfWork.Countries.GetAll();
            var results = _mapper.Map<IList<CountryDTO>>(countries);
            return Ok(results);
        }

        [Authorize]
        [HttpGet("{id:int}", Name = "GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {
            var country = await _unitOfWork.Countries.Get(c=>c.Id == id, new List<string>() {"Hotels"});
            if (country == null)
            {
                return NotFound();
            }
            var result = _mapper.Map<CountryDTO>(country);
            return Ok(result);
        }
        
        [Authorize(Roles = "Administrator")]
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
            var country = _mapper.Map<Country>(countryDto);
            await _unitOfWork.Countries.Insert(country);
            await _unitOfWork.Save();
            return CreatedAtRoute("GetCountry", new { id = country.Id}, country);
            
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateCountryDTO updateCountryDto)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError ($"Invalid UPDATE attemp in {nameof (UpdateCountryDTO)}");
                return BadRequest(ModelState);
            }
            var country = await _unitOfWork.Countries.Get(c => c.Id == id);
            if (country == null)
            {
                _logger.LogError($"Invalid UPDATE attemp in {nameof(UpdateCountryDTO)}");
                return BadRequest("Submitted data is invalid");
            }
            _mapper.Map(updateCountryDto, country);
            _unitOfWork.Countries.Update(country);
            await _unitOfWork.Save();
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
            {
                _logger. LogError($"Something Wen Wrong in the {nameof (Delete) }");
                return BadRequest();
            }
            var country = await _unitOfWork.Countries.Get(c => c.Id == id);
            if (country == null)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(Delete)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Countries.Delete(id);
            await _unitOfWork.Save();
            return NoContent();
        }
    }
}
