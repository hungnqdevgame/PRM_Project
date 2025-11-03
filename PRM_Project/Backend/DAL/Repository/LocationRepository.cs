using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class LocationRepository : ILocationRepository
    {
        private readonly SalesAppDBContext _context;
        public LocationRepository(SalesAppDBContext context)
        {
            _context = context;
        }

        public async Task<StoreLocation> CreateAsync(StoreLocation storeLocation)
        {
            // Add the incoming entity directly so EF Core can set the identity PK
            await _context.StoreLocations.AddAsync(storeLocation);
            await _context.SaveChangesAsync();
            return storeLocation;
        }

        public async Task<string> DeleteAsync(int locationId)
        {
            var locationResult = await _context.StoreLocations.FirstOrDefaultAsync(x => x.LocationId == locationId);
            if (locationResult == null)
            {
                return "Location not found";
            }

            _context.StoreLocations.Remove(locationResult);
            await _context.SaveChangesAsync();
            return "Location deleted";
        }

        public async Task<List<StoreLocation>> GetAllAsync()
            => await _context.StoreLocations.ToListAsync();

        public async Task<StoreLocation> GetByIdAsync(string id)
            => await _context.StoreLocations.FirstOrDefaultAsync(x => x.LocationId.ToString() == id);

        public async Task<StoreLocation> UpdateAsync(StoreLocation location)
        {
            var locationExist = await _context.StoreLocations.FirstOrDefaultAsync(x => x.LocationId == location.LocationId);
            if (locationExist != null)
            {
                locationExist.Latitude = location.Latitude;
                locationExist.Longitude = location.Longitude;
                locationExist.Address = location.Address;
                await _context.SaveChangesAsync();
            }
            return locationExist;
        }
    }
}
