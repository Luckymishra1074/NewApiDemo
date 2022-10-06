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

       
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            var emp = _UserService.GetAll();
            return Ok(emp);
        }
    }
}
