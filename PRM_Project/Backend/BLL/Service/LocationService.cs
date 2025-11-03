using BLL.IService;
using DAL.IRepository;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public Task<StoreLocation> CreateAsync(StoreLocation location)
      => _locationRepository.CreateAsync(location);

        public Task<string> DeleteAsync(int locationId)
     => _locationRepository.DeleteAsync(locationId);

        public Task<List<StoreLocation>> GetAllAsync()
      => _locationRepository.GetAllAsync();

        public Task<StoreLocation> GetByIdAsync(string id)
       => _locationRepository.GetByIdAsync(id);

        public Task<StoreLocation> UpdateAsync(StoreLocation location)
      => _locationRepository.UpdateAsync(location);
    }
}
