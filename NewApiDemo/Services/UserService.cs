using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewApiDemo.Entities;
using NewApiDemo.Helper;
using NewApiDemo.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewApiDemo.Services
{
    public interface IUserService
    {
        AuthenticationResponse authentication(AuthenticationRequest model);

        User GetById(int userId);

        IEnumerable<User> GetAll();
    }

    public class UserService : IUserService
    {
        private readonly AppSetting _appSettings;
        private readonly IConfiguration _Configuration;
        public UserService(IOptions<AppSetting> appsettings , IConfiguration configuration)
        {
           // _appSettings = appsettings.Value;
            _Configuration = configuration;
        }

        private List<User> _users = new List<User>
        {
            new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
        };
        private object iconfiguration;

        public AuthenticationResponse authentication(AuthenticationRequest model)
        {
            var user = _users.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            if (user==null)
            {
                return null;
            }
            var token = GenerateJWTToken(user);
            return new AuthenticationResponse(user, token);
        }

       

        public string GenerateJWTToken(User user)
        {
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            //    Expires = DateTime.UtcNow.AddDays(7),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //return tokenHandler.WriteToken(token);


            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_Configuration["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
              {
             new Claim(ClaimTypes.Name,user.Username)
              }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public User GetById(int userId)
        {
           return  _users.FirstOrDefault(x => x.Id == userId);
        }
       
        IEnumerable<User> IUserService.GetAll()
        {
            return _users;
        }
    }


}
