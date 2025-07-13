using backend_dotnet.Core.Constants;
using backend_dotnet.Core.Dtos.Auth;
using backend_dotnet.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //Route -> Seed Roles to DB
        [HttpPost("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            var seedResult = await _authService.SeedRolesAsync();
            return StatusCode(seedResult.StatusCode, seedResult.Message);
        }


        //Route -> Register User
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var registerResult = await _authService.RegisterAsync(registerDto);
            return StatusCode(registerResult.StatusCode, registerResult.Message);
        }

        //Route -> Login User
        [HttpPost("login")]
        public async Task<ActionResult<LoginServiceResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var loginResult = await _authService.LoginAsync(loginDto);
            if (loginResult is null)
            {
                return Unauthorized("Invalid credentials");
            }
            return Ok(loginResult);
        }

        //Route -> Update User Role
        //An Owner can change everything
        //An Admin can change just User to Manager or reverse
        //Manager and User Roles don't have access to this endpoint
        [HttpPost("update-role")]
        [Authorize(Roles = StaticUserRoles.OwnerAdmin)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            var updateResult = await _authService.UpdateRoleAsync(User, updateRoleDto);
            if (updateResult.IsSucceed)
            {
                return Ok(updateResult.Message);
            }
            else
            {
                return StatusCode(updateResult.StatusCode, updateResult.Message);
            }
        }

        //Route -> Get data of a user from it's Token
        [HttpPost("me")]
        public async Task<ActionResult<LoginServiceResponseDto>> Me([FromBody] MeDto meDto)
        {
            try
            {

                var meResult = await _authService.MeAsync(meDto);
                if (meResult is not null)
                {
                    return Ok(meResult);
                }
                return Unauthorized("Invalid token");
            }
            catch (Exception)
            {
                return Unauthorized("Invalid token");
            }
        }

        //Route -> Get Users List
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserInfoResult>>> GetUsersList()
        {
            var usersList = await _authService.GetUsersListAsync();
            return Ok(usersList);
        }

        //Route -> Get User Details by UserName
        [HttpGet("users/{userName}")]
        public async Task<ActionResult<UserInfoResult>> GetUserDetailsByUserName(string userName)
        {
            var userDetails = await _authService.GetUserDetailsByUserNameAsync(userName);
            if (userDetails is not null)
            {
                return Ok(userDetails);
            }
            return NotFound($"User with username {userName} not found");
        }

        //Route -> Get UserName List
        [HttpGet("usernames")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserNameList()
        {
            var userNameList = await _authService.GetUserNameListAsync();
            return Ok(userNameList);
        }
    }
}
