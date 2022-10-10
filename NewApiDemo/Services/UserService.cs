using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewApiDemo.DbContext;
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

        AuthenticationResponse GenerateRefreshToken(User user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string access_Token);
      //  object GetSavedRefreshTokens(string username, object refresh_Token);


        //// for saving tokens
        ///
        //Task<bool> IsValidUserAsync(User users);

        //RefreshToken AddUserRefreshTokens(RefreshToken user);

        //RefreshToken GetSavedRefreshTokens(string username, string refreshtoken);

        //void DeleteUserRefreshTokens(string username, string refreshToken);

        //int SaveCommit();
    }

    public class UserService : IUserService
    {
        private readonly AppSetting _appSettings;
        private readonly IConfiguration _Configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        public UserService(IOptions<AppSetting> appsettings , IConfiguration configuration,ApplicationDbContext applicationDbContext)
        {
           // _appSettings = appsettings.Value;
            _Configuration = configuration;
            _applicationDbContext = applicationDbContext;
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

        public AuthenticationResponse GenerateRefreshToken(User user)
        {
            var token= GenerateJWTToken(user);
            return new AuthenticationResponse(user, token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var Key = Encoding.UTF8.GetBytes(_Configuration.GetSection("Key").Value);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }


            return principal;
        }



        //public RefreshToken AddUserRefreshTokens(RefreshToken user)
        //{
        //    _db.UserRefreshToken.Add(user);
        //    return user;
        //}

        //public void DeleteUserRefreshTokens(string username, string refreshToken)
        //{
        //    var item = _db.UserRefreshToken.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken);
        //    if (item != null)
        //    {
        //        _db.UserRefreshToken.Remove(item);
        //    }
        //}

        //public RefreshToken GetSavedRefreshTokens(string username, string refreshToken)
        //{
        //    return _db.UserRefreshToken.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken && x.IsActive == true);
        //}

        //public int SaveCommit()
        //{
        //    return _applicationDbContext.SaveChanges();
        //}

        //public async Task<bool> IsValidUserAsync(User users)
        //{
        //    var u = _userManager.Users.FirstOrDefault(o => o.UserName == users.FirstName);
        //    var result = await _userManager.CheckPasswordAsync(u, users.Password);
        //    return result;

        //}
    }


}
