using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IService
{
    public interface ILocationService
    {
        Task<StoreLocation> CreateAsync(StoreLocation location);
        Task<StoreLocation> GetByIdAsync(string id);
        Task<StoreLocation> UpdateAsync(StoreLocation location);
        Task<string> DeleteAsync(int locationId);
        Task<List<StoreLocation>> GetAllAsync();
    }
}
