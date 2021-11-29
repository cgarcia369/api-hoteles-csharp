using AutoMapper;
using Hoteles.Contracs;
using Hoteles.Data.DTOs;
using Hoteles.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoteles.Filters.Action;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Hoteles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOFWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
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
            var hotels = await _unitOfWork.Hotels.GetPagedList(requestParams,new List<string>(){"Country"});
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

        // GET api/<HotelController>/5
        [Authorize]
        [HttpGet("{id:int}",Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ServiceFilter(typeof(ValidateHotelExistsAttribute))]
        public async Task<IActionResult> Get(int id)
        {
            var hotel = await _unitOfWork.Hotels.Get(h => h.Id == id, new List<string> { "Country" });
            var result = _mapper.Map<HotelDTO>(hotel);
            return Ok(result);
        }
        
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ServiceFilter(typeof(ValidationModel))]
        public async Task<IActionResult> Post([FromBody] CreateHotelDTO hotelDto)
        {
            var hotel = _mapper.Map<Hotel>(hotelDto);
            await _unitOfWork.Hotels.Insert(hotel);
            await _unitOfWork.Save();
            return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);
        }
        
        [Authorize(Roles = "Administrator")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ServiceFilter(typeof(ValidationModel))]
        [ServiceFilter(typeof(ValidateHotelExistsAttribute))]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateHotelDTO updateHotelDto)
        {
            var hotel = await _unitOfWork.Hotels.Get(h => h.Id == id);
            _mapper.Map(updateHotelDto, hotel);
            _unitOfWork.Hotels.Update(hotel);
            await _unitOfWork.Save();
            return NoContent();
        }
            
            
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ServiceFilter(typeof(ValidateHotelExistsAttribute))]
        public async Task<IActionResult> Delete(int id)
        {

            var hotel = await _unitOfWork.Hotels.Get(h => h.Id == id);
            await _unitOfWork.Hotels.Delete(id);
            await _unitOfWork.Save();
            return NoContent();
        }
    }
}
