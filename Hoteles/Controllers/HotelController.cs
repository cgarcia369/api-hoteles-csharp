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
        [HttpGet("{id:int}")]
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
    }
}
