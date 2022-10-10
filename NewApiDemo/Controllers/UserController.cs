using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

       
        //[HttpGet]
        //[Route("GetAll")]
        //public IActionResult GetAll()
        //{
        //    var emp = _UserService.GetAll();
        //    return Ok(emp);
        //}

    //    [HttpPost]
    //    [Route("RefreshToken")]
    //    [AllowAnonymous]
    //    public IActionResult RefreshToken(AuthenticationResponse token)
    //    {
    //        var principal = _UserService.GetPrincipalFromExpiredToken(token.Token);
    //        var username = principal.Identity?.Name;

    //        //retrieve the saved refresh token from database
    //        var savedRefreshToken = _UserService.GetSavedRefreshTokens(username, token.Refresh_Token);

    //        if (savedRefreshToken.RefreshToken != token.Refresh_Token)
    //        {
    //            return Unauthorized("Invalid attempt!");
    //        }

    //        var newJwtToken = jWTManager.GenerateRefreshToken(username);

    //        if (newJwtToken == null)
    //        {
    //            return Unauthorized("Invalid attempt!");
    //        }

    //        // saving refresh token to the db
    //        UserRefreshTokens obj = new UserRefreshTokens
    //        {
    //            RefreshToken = newJwtToken.Refresh_Token,
    //            UserName = username
    //        };

    //        userServiceRepository.DeleteUserRefreshTokens(username, token.Refresh_Token);
    //        userServiceRepository.AddUserRefreshTokens(obj);
    //        userServiceRepository.SaveCommit();

    //        return Ok(newJwtToken);
    //    }
    //}
    }
}
