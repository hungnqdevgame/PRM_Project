using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface ILocationRepository
    {
        Task<StoreLocation> CreateAsync(StoreLocation storeLocation);
        Task<StoreLocation> GetByIdAsync(string id);
        Task<StoreLocation> UpdateAsync(StoreLocation storeLocation);
        Task<string> DeleteAsync(int locationId);
        Task<List<StoreLocation>> GetAllAsync();
    }
}
