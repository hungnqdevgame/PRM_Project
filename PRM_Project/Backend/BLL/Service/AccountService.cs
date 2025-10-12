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
    public class AccountService : IAccountService
    {
        private readonly  IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Task<string> LoginAsync(string username, string password)
       =>_accountRepository.LoginAsync(username, password);

        public Task<User> RegisterAsync(string username, string password, string email, string phone, string address)
       => _accountRepository.RegisterAsync(username, password,email,phone,address);
    }
}
