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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public Task<List<User>> GetAllUsersAsync()
       =>_userRepository.GetAllUsersAsync();

        public Task<User> GetUserByIdAsync(int userId)
        => _userRepository.GetUserByIdAsync(userId);

        public User GetUserByUsername(string username)
     =>   _userRepository.GetUserByUsername(username);
    }
}
