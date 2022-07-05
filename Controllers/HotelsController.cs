using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Data;
using HotelListing.Core.IRepository;
using AutoMapper;
using HotelListing.Core.Models.Hotel;
using Microsoft.AspNetCore.Authorization;
using HotelListing.Core.Models;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private IHotelsRepository _hotelsRepository;
        private IMapper _mapper;

        public HotelsController(IMapper mapper, IHotelsRepository hotelsRepository)
        {
            this._mapper = mapper;
            this._hotelsRepository = hotelsRepository;
        }

        // GET: api/Hotels
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<GetHotelDto>>> GetHotels()
        {
            var hotels = await _hotelsRepository.GetAllAsync<GetHotelDto>();
            return Ok(hotels); 
        }


        // GET: api/Hotels?startindex=1&pagesize=1&pagenumber=3
        [HttpGet]
        public async Task<ActionResult<PagedResult<GetHotelDto>>> GetHotels([FromQuery]QueryParameters queryParameters)
        {
            var pagedHotelsResult = await _hotelsRepository.GetAllAsync<GetHotelDto>(queryParameters);
            return Ok(pagedHotelsResult); 
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetHotelDto>> GetHotel(int id)
        {
            var hotel = await _hotelsRepository.GetAsync<GetHotelDto>(id);

            return Ok(hotel);
        }

        // PUT: api/Hotels/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutHotel(int id, UpdateHotelDto updateHotelDto)
        {
            if (id != updateHotelDto.Id)
            {
                return BadRequest("name, address, rating, countryId and hotelId required");
            }

            try
            {
                await _hotelsRepository.UpdateAsync<UpdateHotelDto>(id, updateHotelDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(NoContent());
        }

        // POST: api/Hotels
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CreateHotelDto>> PostHotel(CreateHotelDto createHotelDto)
        {
            var hotel = _mapper.Map<Hotel>(createHotelDto);

            var added = await _hotelsRepository.AddAsync(hotel);

            var addedDto = _mapper.Map<CreateHotelDto>(added);

            return Ok(addedDto);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            await _hotelsRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> HotelExistsAsync(int id)
        {
            return await _hotelsRepository.Exists(id);
        }
    }
}
