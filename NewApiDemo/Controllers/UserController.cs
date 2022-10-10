using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewApiDemo.Entities;
using NewApiDemo.Models;
using NewApiDemo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewApiDemo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _UserService;
        public UserController(IUserService userService)
        {
            _UserService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate(AuthenticationRequest model) 
        {

           var response= _UserService.authentication(model);
            if (response==null)
            {
                return BadRequest(new {message= "Username or Password is incorrect" });
            }
            return Ok(response);
        }

       
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            var emp = _UserService.GetAll();
            return Ok(emp);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("GetToken")]
        public async Task<IActionResult> GetToken(Login usersdata)
        {
            var validUser = await _UserService.IsValidUserAsync(usersdata);

            if (!validUser)
            {
                return Unauthorized("Incorrect username or password!");
            }

            var token = _UserService.GenerateToken(usersdata.Username);

            if (token == null)
            {
                return Unauthorized("Invalid Attempt!");
            }

            // saving refresh token to the db
            RefreshToken obj = new RefreshToken
            {
                RefreshTokens = token.Refresh_Token,
                UserName = usersdata.Username
            };

            _UserService.AddUserRefreshTokens(obj);
            _UserService.SaveCommit();
            return Ok(token);
        }


        [HttpPost]
        [Route("RefreshToken")]
        [AllowAnonymous]
        public IActionResult RefreshToken(Token token)
        {
            var principal = _UserService.GetPrincipalFromExpiredToken(token.Access_Token);
            var username = principal.Identity?.Name;

            //retrieve the saved refresh token from database
            var savedRefreshToken = _UserService.GetSavedRefreshTokens(username, token.Refresh_Token);

            if (savedRefreshToken.RefreshTokens != token.Refresh_Token)
            {
                return Unauthorized("Invalid attempt!");
            }

            var newJwtToken = _UserService.GenerateRefreshToken(username);

            if (newJwtToken == null)
            {
                return Unauthorized("Invalid attempt!");
            }

            // saving refresh token to the db
            RefreshToken obj = new RefreshToken
            {
                RefreshTokens = newJwtToken.Refresh_Token,
                UserName = username
            };

            _UserService.DeleteUserRefreshTokens(username, token.Refresh_Token);
            _UserService.AddUserRefreshTokens(obj);
            _UserService.SaveCommit();

            return Ok(newJwtToken);
        }



    }
}

