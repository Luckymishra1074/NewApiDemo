using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NewApiDemo.Identity;
using NewApiDemo.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewApiDemo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly HealthCheckService _healthCheckService;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration,ILogger<AdminController> logger,HealthCheckService healthCheckService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _healthCheckService = healthCheckService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User Registered Successfully" });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            _logger.LogInformation("Login start");
            var user = await userManager.FindByNameAsync(model.Username);
            _logger.LogInformation($"user info :{user}");
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
{
new Claim(ClaimTypes.Name, user.UserName),
new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
};

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Key").Value));

                var token = new JwtSecurityToken(
                issuer:_configuration.GetSection("Issuer").Value,
                audience: _configuration.GetSection("Audience").Value,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("GetAllUser")]
        public IActionResult GetAllUser()
        {
            var users = userManager.Users;
            return Ok(users);
        }

        [HttpGet]
        [Route("GetUserById")]
        public IActionResult GetUserById(string Username)
        {
            var users = userManager.Users.Where(x=> x.UserName==Username);
            return Ok(users);
        }

       
        [HttpGet]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUserAsync(string Username)
        {
            var users =await userManager.FindByNameAsync(Username);
            if (users!=null)
            {
                var status = await userManager.DeleteAsync(users);
                if (!status.Succeeded)
                {
                    return Ok(new { message = "User not Deleted " });
                }
                
            }
            return Ok(new { message = "User  Deleted Successfully" });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("CheckHealth")]
        public async Task<IActionResult> CheckHealthAsync()
        {
            var result =await _healthCheckService.CheckHealthAsync();

            return result.Status == HealthStatus.Healthy ? Ok(result.Status) : StatusCode((int)HttpStatusCode.ServiceUnavailable, result);
        }
    }
}
