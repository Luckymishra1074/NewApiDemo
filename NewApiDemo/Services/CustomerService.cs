using NewApiDemo.Interfaces;
using NewApiDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewApiDemo.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly List<Customer> customers = new List<Customer>();

        public CustomerService()
        {
            customers.Add( new Customer() { Id = Guid.NewGuid(), FirstName = "user1", LastName = "test", EmailAddress = "user1@gmailcom" } );
            customers.Add(new Customer() { Id = Guid.NewGuid(), FirstName = "user2", LastName = "test", EmailAddress = "user2@gmailcom" } );
        }
        public Task<List<Customer>> GetAllCustomers()
        {
            return Task.FromResult(customers);
        }
    }
}
