using BLL.IService;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PRM_BE.DTO;

namespace PRM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
       private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        public AuthController(IAccountService accountService,IUserService userService)
        {
            _accountService = accountService;
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterDTO request)
        {
            var user = await _accountService.RegisterAsync(
         request.Username,
         request.Password,
         request.Email,
         request.PhoneNumber,
         request.Address
        );



            return Ok(new
            {
                user.UserId,
                user.Username,
                user.Email,
                user.PhoneNumber,
                user.Address,
                user.Role
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                var token = await _accountService.LoginAsync(request.Username, request.Password);
                var user =  _userService.GetUserByUsername(request.Username);
                return Ok(new
                {
                    userId = user.UserId,
                    token = token
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
