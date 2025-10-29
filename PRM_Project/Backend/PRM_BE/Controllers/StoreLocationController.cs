using BLL.IService;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRM_BE.DTO;

namespace PRM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreLocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        public StoreLocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost("CreateLocation")]
        public async Task<IActionResult> CreateLocation([FromBody] StoreLocationDTO location)
        {
            var newLocation = new StoreLocation()   
            {
                Longitude = location.Longitude,
                Latitude = location.Latitude,
                Address = location.Address
            };
            var created = await _locationService.CreateAsync(newLocation);
            // Return the entity returned by repository (includes generated LocationId)
            return Ok(created);
        }

        [HttpGet("GetLocationById/{id}")]
        public async Task<IActionResult> GetLocationById(string id)
        {
            var location = await _locationService.GetByIdAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return Ok(location);

        }

        [HttpPut("UpdateLocation")]
        public async Task<IActionResult> UpdateLocation([FromBody] StoreLocationDTO location)
        {
            var existLocation = await _locationService.GetByIdAsync(location.Id);
            existLocation.Longitude = location.Longitude;
            existLocation.Latitude = location.Latitude;
            existLocation.Address = location.Address;
           await _locationService.UpdateAsync(existLocation);
          
            return Ok(existLocation);
        }

        [HttpDelete("DeleteLocation")]
        public async Task<IActionResult> DeleteLocation(int locationId)
        {
            var result = await _locationService.DeleteAsync(locationId);
            return Ok(result);
        }

        [HttpGet("GetAllLocations")]
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _locationService.GetAllAsync();
            return Ok(locations);
        }
    }
}
