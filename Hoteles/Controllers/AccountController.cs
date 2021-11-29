using AutoMapper;
using Hoteles.Data.DTOs;
using Hoteles.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Hoteles.Services.contracts;
using Microsoft.AspNetCore.Authorization;

namespace Hoteles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<ApiUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;

        public AccountController(UserManager<ApiUser> userManager, ILogger<AccountController> logger, IMapper mapper, IAuthManager authManager)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _authManager = authManager;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)
        {
            _logger.LogInformation($"Registration Attemp for {userDto.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = _mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;

                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return BadRequest(ModelState);
                }

                await _userManager.AddToRolesAsync(user, userDto.Roles);
                return Accepted();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something Went Wrong in the {nameof(Register)}");
                return Problem($"Something  Went Wrong in the {nameof(Register)}", statusCode: 500);
            }
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDto)
        {
            _logger.LogInformation($"Login Attemp for {loginUserDto.Email}");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (!await _authManager.ValidateUser(loginUserDto))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
                var rol = await _userManager.GetRolesAsync(user);
                
                return Accepted(new {Token = await _authManager.CreateToken(),User = new {
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    rol = rol[0]
                }});

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Something Went Wrong in the {nameof(Login)}");
                return Problem($"Something  Went Wrong in the {nameof(Login)}", statusCode: 500);
            }
        }

        [HttpGet]
        [Authorize]
        public async  Task<IActionResult> VerifyToken()
        {
            var identityName = HttpContext.User.Identity?.Name;
            ApiUser user;
            if (identityName != null)
            {
                user = await _userManager.FindByEmailAsync(identityName);
                var rol = await _userManager.GetRolesAsync(user);
                return Ok(new
                {
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    rol = rol[0]
                });
            }

            return BadRequest();
        }

    }
}
