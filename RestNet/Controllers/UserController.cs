using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Swashbuckle.Swagger.Annotations;

namespace RestNet.Controllers
{
    public class UserController : ApiController
    {
        private WorkshopContext db = new WorkshopContext();

        private SHA256 hasher = new SHA256Managed();

        /// <summary>
        /// Add user to database
        /// </summary>
        /// <remarks>
        ///
        /// </remarks>
        /// <param name="credentials">User credentials</param>
        /// <returns>Token information for newly created user</returns>
        /// <response code="200">Token information for newly created user</response>
        /// <response code="400">Login is already taken</response>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/user/add")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TokenInfo))]
        public IHttpActionResult CreateUser([FromBody] UserCredentials credentials)
        {
            if (db.Users.Count(user => user.Login == credentials.Login) > 0)
            {
                return BadRequest("Login bestaat al");
            }
            else
            {
                byte[] passBytes = Encoding.UTF8.GetBytes(credentials.Password);
                var user = new User()
                {
                    Login = credentials.Login,
                    Name = "",
                    Surname = "",
                    Unit = Unit.Nord,
                    Password = Encoding.UTF8.GetString(hasher.ComputeHash(passBytes)),
                    Birthday = DateTime.Now
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
                var tokenExpiration = TimeSpan.FromHours(2);

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

        /// <summary>
        /// Get information about user
        /// </summary>
        /// <remarks>
        /// Need to be authenticated. User is taken from bearer token
        /// </remarks>
        /// <returns>Information about user</returns>
        /// <response code="200">Data transfer object for user</response>
        /// <response code="400">User not found</response>
        [HttpGet]
        [Route("api/user/me")]
        [Authorize()]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserDTO))]
        public IHttpActionResult GetUserInformation()
        {
            var _user = (System.Security.Claims.ClaimsIdentity) User.Identity;
            var id = Int32.Parse(_user.FindFirstValue("UserId"));
            var currentUser = db.Users.FirstOrDefault(user => user.ID == id);
            if (currentUser == null)
            {
                return BadRequest("Geen gebruiker");
            }
            else
            {
                return Ok(new UserDTO()
                {
                    Name = currentUser.Name,
                    Surname = currentUser.Surname,
                    Unit = currentUser.Unit,
                    Birthday = currentUser.Birthday
                });
            }
        }


        /// <summary>
        /// Update information about user
        /// </summary>
        /// <remarks>
        /// Need to be authenticated. User is taken from bearer token. All fields will be overriden.
        /// </remarks>
        /// <param name="userData">Newly provided user data</param>
        /// <returns>Nothing</returns>
        /// <response code="200">Data sucesfully updated</response>
        /// <response code="400">User not found</response>
        [HttpPost]
        [Route("api/user/me")]
        [Authorize()]
        public IHttpActionResult UpdateUserInformation([FromBody] UserDTO userData)
        {
            var _user = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var id = Int32.Parse(_user.FindFirstValue("UserId"));
            var currentUser = db.Users.FirstOrDefault(user => user.ID == id);
            if (currentUser == null)
            {
                return BadRequest("Geen gebruiker");
            }
            else
            {
                currentUser.Name = userData.Name;
                currentUser.Surname = userData.Surname;
                currentUser.Unit = userData.Unit;
                currentUser.Birthday = userData.Birthday;
                var updated = db.SaveChanges();
                if (updated == 0) return BadRequest("Informatie kon niet worden opgeslagen");
                else return Ok();
            }
        }

    }
}