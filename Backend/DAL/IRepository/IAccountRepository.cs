using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IAccountRepository
    {
        Task<User> RegisterAsync(string username, string password, string email, string phone, string address);
        Task<string> LoginAsync(string username, string password);
    }
}
