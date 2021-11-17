using AutoMapper;
using Hoteles.Contracs;
using Hoteles.Data.DTOs;
using Hoteles.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Hoteles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOFWork _unitOFWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOFWork unitOFWork, ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOFWork = unitOFWork;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<HotelController>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery]RequestParams requestParams)
        {
            try
            {
                var hotels = await _unitOFWork.Hotels.GetPagedList(requestParams,new List<string>(){"Country"});
                var results = _mapper.Map<IList<HotelDTO>>(hotels);
                return Ok(new
                {
                    Data = results,
                    Metadata = new MetaData()
                    {
                        Count = hotels.Count,
                        PageCount = hotels.PageCount,
                        PageNumber = hotels.PageNumber,
                        PageSize = hotels.PageSize,
                        HasNextPage = hotels.HasNextPage,
                        HasPreviousPage = hotels.HasPreviousPage,
                        IsFirstPage = hotels.IsFirstPage,
                        IsLastPage = hotels.IsLastPage,
                        TotalItemCount = hotels.TotalItemCount,
                        FirstItemOnPage = hotels.FirstItemOnPage,
                        LastItemOnPage = hotels.LastItemOnPage,
                    }
                });
            }
            catch (Exception e)
            {

                _logger.LogError(e, $"Something wen wrong in the {nameof(Get)}");
                return StatusCode(500, "Internal Server error, please try Again later.");
            }
        }

        // GET api/<HotelController>/5
        [Authorize]
        [HttpGet("{id:int}",Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var hotel = await _unitOFWork.Hotels.Get(h => h.Id == id, new List<string> { "Country" });
                if (hotel == null)
                {
                    return NotFound();
                }
                var result = _mapper.Map<HotelDTO>(hotel);
                return Ok(result);
            }
            catch (Exception e)
            {

                _logger.LogError(e, $"Something wen wrong in the {nameof(Get)}");
                return StatusCode(500, "Internal Server error, please try Again later.");
            }
        }
        
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateHotelDTO hotelDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attemp in {nameof(CreateHotelDTO)}");
                return BadRequest(ModelState);
            }
            try
            {
                var hotel = _mapper.Map<Hotel>(hotelDto);
                await _unitOFWork.Hotels.Insert(hotel);
                await _unitOFWork.Save();
                return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);
            }
            catch (Exception e)
            {

                _logger.LogError(e, $"Something Wen Wrong in the {nameof(CreateHotelDTO)}");
                return StatusCode(500, "Internal server Error");
            }
        }
        
        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateHotelDTO updateHotelDto)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger. LogError ($"Invalid UPDATE attempt in {nameof (UpdateHotelDTO)}");
                return BadRequest (ModelState);
            }

            try
            {
                var hotel = await _unitOFWork.Hotels.Get(h => h.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotelDTO)}");
                    return BadRequest("Submitted date is invalid");
                }

                _mapper.Map(updateHotelDto, hotel);
                _unitOFWork.Hotels.Update(hotel);
                await _unitOFWork.Save();
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError (e, $"Something ien Wrong in the {nameof (UpdateHotelDTO)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }
            
            
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
            {
                _logger. LogError($"Something Wen Wrong in the {nameof (Delete)}");
                return BadRequest();
            }

            try
            {
                var hotel = await _unitOFWork.Hotels.Get(h => h.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"Invalid DELETE attempt in {nameof(Delete)}");
                    return BadRequest("Submitted data is invalid");
                }

                await _unitOFWork.Hotels.Delete(id);
                await _unitOFWork.Save();
                return NoContent();
            }
            catch (Exception e)
            {
                _logger. LogError(e, $"Something ien Wrong in the {nameof (Delete)}");
                return BadRequest();
            }
        }
    }
}
