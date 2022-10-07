using NewApiDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewApiDemo.Interfaces
{
  public  interface ICustomerService
    {
        public Task<List<Customer>> GetAllCustomers();
    }
}
