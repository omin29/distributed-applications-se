using ApplicationService.DTOs;
using ApplicationService.Implementations;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchoolInfoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManagementService _userManagementService;
        private readonly IConfiguration _configuration; //For accessing data from appsettings.json

        public AuthController(IConfiguration configuration)
        {
            _userManagementService = new UserManagementService();
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a user.
        /// </summary>
        /// <param name="userDTO">DTO with the necessary user data</param>
        /// <returns></returns>
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Register(UserDTO userDTO)
        {
            if(userDTO.Validate() == false)
            {
                return BadRequest("Invalid data!");
            }

            if (_userManagementService.Exists(userDTO))
            {
                return BadRequest("This username is taken.");
            }

            if (_userManagementService.Register(userDTO) == false)
            {
                return StatusCode(500, "Registration has failed!");
            }

            return Ok("Registration is successful.");
        }

        /// <summary>
        /// Returns a JWT token for the user if the login is successful.
        /// </summary>
        /// <param name="userDTO">DTO with the necessary user data</param>
        /// <returns></returns>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login(UserDTO userDTO)
        {
            if(_userManagementService.Login(userDTO) == false)
            {
                return BadRequest("Invalid username or password!");
            }

            string token = CreateJwtToken(userDTO);

            return Ok(token);
        }

        private string CreateJwtToken(UserDTO user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:JwtTokenKey").Value!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var jwtToken = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration.GetSection("AppSettings:JwtIssuer").Value!,
                audience: _configuration.GetSection("AppSettings:JwtAudience").Value!,
                expires: DateTime.Now.AddHours(1), 
                signingCredentials: credentials
                );

            string jwt = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return jwt;
        }
    }
}
