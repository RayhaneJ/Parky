using ParkyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        public bool IsUniqueUser(string username);
        public User Authenticate(string username, string password);
        public User Register(string username, string password);
    }
}
