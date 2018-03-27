using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using RestNet.DAL;
using RestNet.Models;

namespace RestNet.Service
{
    public class UserService
    {
        private WorkshopContext db = new WorkshopContext();

        private static SHA256 hasher = new SHA256Managed();

        public User GetUserByCredentials(string _login, string password)
        {


            var newUser = db.Users.FirstOrDefault(user => user.Login == _login);
            if (newUser == null) return null;
            byte[] passBytes = Encoding.UTF8.GetBytes(password);
            string passHash = Encoding.UTF8.GetString(hasher.ComputeHash(passBytes));
            if (newUser.Password == passHash)
            {
                return newUser; 
            }
            else
            {
                return null;
            }
        }
    }
}