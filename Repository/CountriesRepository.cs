using HotelListing.Data;
using HotelListing.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Repository
{
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private HotelListingDbContext _context;
        public CountriesRepository(HotelListingDbContext context) : base(context)
        {
            this._context = context; 
        }

        public async Task<Country> GetDetails(int id)
        {
            return await _context.Countries.Include(q => q.Hotels).FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
