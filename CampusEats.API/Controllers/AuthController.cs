using CampusEats.API.Configurations;
using CampusEats.API.DTOs.Auth;
using CampusEats.Core.Entities;
using CampusEats.Core.Enums;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CampusEats.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtConfig _jwtConfig;

        public AuthController(
            IUserService userService,
            IOptions<JwtConfig> jwtConfig)
        {
            _userService = userService;
            _jwtConfig = jwtConfig.Value;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var user = await _userService.AuthenticateAsync(request.Username, request.Password);
                var token = GenerateJwtToken(user);

                return Ok(new LoginResponseDto
                {
                    Token = token,
                    Username = user.Username,
                    FullName = user.FullName,
                    UserType = user.UserType.ToString()
                });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    FullName = request.FullName,
                    Phone = request.Phone,
                    UserType = Enum.Parse<UserType>(request.UserType)
                };

                var registeredUser = await _userService.RegisterUserAsync(user, request.Password);

                return Ok(new RegisterResponseDto
                {
                    UserId = registeredUser.UserId,
                    Username = registeredUser.Username,
                    Email = registeredUser.Email,
                    FullName = registeredUser.FullName,
                    UserType = registeredUser.UserType.ToString()
                });
            }
            catch (DuplicateUserException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while registering the user." });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtConfig.ExpirationInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
