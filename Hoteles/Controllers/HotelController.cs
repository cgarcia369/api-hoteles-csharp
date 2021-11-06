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
        public async Task<IActionResult> Get([FromQuery]RequestParams requestParams)
        {
            try
            {
                var hotels = await _unitOFWork.Hotels.GetPagedList(requestParams);
                var results = _mapper.Map<IList<HotelDTO>>(hotels);
                return Ok(results);
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
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var hotel = await _unitOFWork.Hotels.Get(h => h.Id == id, new List<string> { "Country" });
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
    }
}
