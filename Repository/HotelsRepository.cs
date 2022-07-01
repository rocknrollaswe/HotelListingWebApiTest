using HotelListing.Data;
using HotelListing.IRepository;

namespace HotelListing.Repository
{
    public class HotelsRepository : GenericRepository<Hotel>, IHotelsRepository
    {
        private HotelListingDbContext _context;
        public HotelsRepository(HotelListingDbContext context) : base(context)
        {
            this._context = context; 
        }
    }
}
