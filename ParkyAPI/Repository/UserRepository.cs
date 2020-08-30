using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly AppSettings appSettings;

        public UserRepository(ApplicationDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            this.dbContext = dbContext;
            this.appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var user = dbContext.Users.Single(x => x.Username == username && x.Password == password);

            if (user == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = "";

            return user;
        }

        public bool IsUniqueUser(string username)
        {
            var user = dbContext.Users.SingleOrDefault(u => u.Username == username);

            if (user == null)
                return true;

            return false;
        }

        public User Register(string username, string password)
        {
            var user = new User
            {
                Username = username,
                Password = password
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            user.Password = "";

            return user;
        }
    }
}
