using Microsoft.AspNetCore.Mvc;
using UserMicroservice.Data.DTOs;
using UserMicroservice.Services.UserService;

namespace UserMicroservice.Controllers
{
    public class UserController : Controller
    {
        [ApiController]
        [Route("api/[controller]")]
        public class UsersController : ControllerBase
        {
            private readonly IUserService _userService;

            public UsersController(IUserService userService)
            {
                _userService = userService;
            }

            [HttpPost("register")]
            public async Task<IActionResult> Register([FromForm] RegisterDTO dto)
            {
                var result = await _userService.RegisterAsync(dto);
                return Ok(result);
            }

            [HttpPost("login")]
            public async Task<IActionResult> Token([FromBody] LoginDto dto)
            {
                var token = await _userService.LoginAsync(dto);
                return Ok(new { token });
            }
        }
    }
}
