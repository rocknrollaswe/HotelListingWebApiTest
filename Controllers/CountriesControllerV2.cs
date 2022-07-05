﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Data;
using HotelListing.Core.Models.Country;
using AutoMapper;
using HotelListing.Core.IRepository;
using Microsoft.AspNetCore.Authorization;
using HotelListing.Core.Exceptions;
using Microsoft.AspNetCore.OData.Query;

namespace HotelListing.Controllers 
{
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [ApiVersion("2.0")]

    public class CountriesV2Controller : ControllerBase
    {
        
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger _logger;

        public CountriesV2Controller(IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesController> logger)
        {
            
            _mapper = mapper;
            this._countriesRepository = countriesRepository;
            this._logger = logger;
        }

        // GET: api/Countries
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _countriesRepository.GetAllAsync<GetCountryDto>(); 
            //var records = _mapper.Map<List<GetCountryDto>>(countries);
            return Ok(countries);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _countriesRepository.GetDetails(id);

            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountry), id); 
            }

            var countryDto = _mapper.Map<CountryDto>(country);

            return Ok(countryDto);
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
                if (!await CountryExists(id))
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
        public async Task<ActionResult<Country>> PostCountry([FromBody] CreateCountryDto createCountry)
        {

            var country = _mapper.Map<Country>(createCountry);

            var added = await _countriesRepository.AddAsync(country);

            var addedDto = _mapper.Map<CreateCountryDto>(added);

            return Ok(addedDto); 
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
