using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Data;
using HotelListing.IRepository;
using AutoMapper;
using HotelListing.Models.Hotel;
using Microsoft.AspNetCore.Authorization;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetHotelDto>>> GetHotels()
        {
            var hotels = await _hotelsRepository.GetAllAsync(); 
            var hotelDtos = _mapper.Map<List<HotelDto>>(hotels);
            return Ok(hotelDtos); 
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetHotelDto>> GetHotel(int id)
        {
            var hotel = await _hotelsRepository.GetAsync(id);

            if (hotel == null)
            {
                return NotFound();
            }

            var hotelDto = _mapper.Map<GetHotelDto>(hotel); 

            return Ok(hotelDto);
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

            var hotel = _mapper.Map<Hotel>(updateHotelDto); 
           
            try
            {
                await _hotelsRepository.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!await HotelExistsAsync(id))
                {
                    return NotFound(); 
                }

                throw;
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
            var hotel = await _hotelsRepository.GetAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            await _hotelsRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> HotelExistsAsync(int id)
        {
            return await _hotelsRepository.Exists(id);
        }
    }
}
