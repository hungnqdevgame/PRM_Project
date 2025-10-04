using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
        Task<User> GetUserByIdAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
    }
}
