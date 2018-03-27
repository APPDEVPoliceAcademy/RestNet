using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using RestNet.DAL;
using RestNet.Models;
using RestNet.Service;

namespace RestNet.Controllers
{
    public class UserController : ApiController
    {
        private WorkshopContext db = new WorkshopContext();

        private SHA256 hasher = new SHA256Managed();
        [AllowAnonymous]
        [HttpPost]
        [Route("api/user/add")]
        public IHttpActionResult createUser([FromBody] UserCredentials credentials)
        {
            if (db.Users.Count(user => user.Login == credentials.Login) > 0)
            {
                return BadRequest("Login already taken");
            }
            else
            {
                byte[] passBytes = Encoding.UTF8.GetBytes(credentials.Password);
                var user = new User()
                {
                    Login = credentials.Login,
                    Name = "",
                    Surname = "",
                    Unit = "",
                    Password = Encoding.UTF8.GetString(hasher.ComputeHash(passBytes))
                };
                db.Users.Add(user);
                db.SaveChanges();
                // Generate token

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim("UserID", user.ID.ToString()),
                    new Claim(ClaimTypes.Role, Rights.user.ToString())
                };

                ClaimsIdentity oAutIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                var tokenExpiration = TimeSpan.FromDays(2);

                var props = new AuthenticationProperties()
                {
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration)
                };

                var ticket = new AuthenticationTicket(oAutIdentity, props);

                var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);

                JObject tokenResponse = new JObject()
                {
                    new JProperty("access_token", accessToken),
                    new JProperty("token_type", "bearer"),
                    new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString())
                };
                

                return Ok(tokenResponse);

            }
        }

        

        [HttpGet]
        [Route("api/user/me")]
        [Authorize()]
        public IHttpActionResult GetMe()
        {
            var _user = (System.Security.Claims.ClaimsIdentity) User.Identity;
            var id = Int32.Parse(_user.FindFirstValue("UserId"));
            var currentUser = db.Users.FirstOrDefault(user => user.ID == id);
            if (currentUser == null)
            {
                return BadRequest("No user");
            }
            else
            {
                return Ok(new UserDTO()
                {
                    Name = currentUser.Name,
                    Surname = currentUser.Surname,
                    Unit = currentUser.Unit
                });
            }
        }

        [HttpPost]
        [Route("api/user/me")]
        [Authorize()]
        public IHttpActionResult UpdateMe([FromBody] UserDTO userData)
        {
            var _user = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var id = Int32.Parse(_user.FindFirstValue("UserId"));
            var currentUser = db.Users.FirstOrDefault(user => user.ID == id);
            if (currentUser == null)
            {
                return BadRequest("No user");
            }
            else
            {
                currentUser.Name = userData.Name;
                currentUser.Surname = userData.Surname;
                currentUser.Unit = userData.Unit;
                var updated = db.SaveChanges();
                if (updated == 0) return BadRequest("Couldn't save new info to database");
                else return Ok();
            }
        }

    }
}