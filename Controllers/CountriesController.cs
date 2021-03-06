using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Data;
using HotelListing.Core.Models.Country;
using AutoMapper;
using HotelListing.Core.IRepository;
using Microsoft.AspNetCore.Authorization;
using HotelListing.Core.Exceptions;
using HotelListing.Core.Models;

namespace HotelListing.Controllers 
{
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]

    public class CountriesController : ControllerBase
    {
        
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger _logger;

        public CountriesController(IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesController> logger)
        {
            
            _mapper = mapper;
            this._countriesRepository = countriesRepository;
            this._logger = logger;
        }

        // GET: api/Countries
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _countriesRepository.GetAllAsync<GetCountryDto>(); 
            return Ok(countries);
        }

        // GET: api/Countries?StartIndex=0&pagesize=15&pagenumber=1
        [HttpGet]
        public async Task<ActionResult<PagedResult<GetCountryDto>>> GetCountries([FromQuery] QueryParameters queryParameters)
        {
            var pagedCountriesResult = await _countriesRepository.GetAllAsync<GetCountryDto>(queryParameters);
            return Ok(pagedCountriesResult); 
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _countriesRepository.GetDetails(id);
            return Ok(country);
        }

        // PUT: api/Countries/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry([FromRoute] int id, [FromBody] UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                return BadRequest("Invalid Record Id");
            }
            try
            {
                await _countriesRepository.UpdateAsync<UpdateCountryDto>(id, updateCountryDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!await CountryExists(id))
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

        // POST: api/Countries
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CountryDto>> PostCountry([FromBody] CreateCountryDto createCountry)
        {

            var country = await _countriesRepository.AddAsync<CreateCountryDto, CountryDto>(createCountry); 

            return Ok(country); 
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            await _countriesRepository.DeleteAsync(id); 
            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            return await _countriesRepository.Exists(id); 
        }
    }
}
