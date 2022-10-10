using Microsoft.AspNetCore.Mvc;
using NewApiDemo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewApiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiGatewayController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public ApiGatewayController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Catcher Wong", "James Li" };
        }

        [HttpGet]
        [Route("GetAllCustomer")]
        public IActionResult GetAllCustomer()
        {
            var res = _customerService.GetAllCustomers();
            return Ok(res);
        }
    }
}
