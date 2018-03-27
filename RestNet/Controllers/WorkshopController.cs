using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using RestNet.DAL;
using RestNet.Models;

namespace RestNet.Controllers
{
    public class WorkshopController : ApiController
    {
        private WorkshopContext db = new WorkshopContext();


        [AllowAnonymous]
        [HttpGet]
        [Route("api/workshops/all")]
        public IHttpActionResult GetAll()
        {
            var allShort = db.Workshops.Select(workshop => new WorkshopShortDTO()
            {
                Coach = workshop.Coach,
                Id = workshop.Id,
                Place = workshop.Place,
                ShortDescription = workshop.ShortDescription,
                Title = workshop.Title,
                Date = workshop.Date
                
            }).ToList();

            return Ok(allShort);
        }

        [Authorize]
        [HttpGet]
        [Route("api/workshops/me")]
        public IHttpActionResult GetForSingleUser()
        {
            var _user = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var id = Int32.Parse(_user.FindFirstValue("UserId"));

            var workshops =  db.Workshops.Where(workshop => workshop.Users.Any(user => user.ID == id) == true).Select( workshop => new WorkshopShortDTO()
            {
                Id = workshop.Id,
                Coach = workshop.Coach,
                Place = workshop.Place,
                ShortDescription = workshop.ShortDescription,
                Title = workshop.Title,
                Date = workshop.Date
            } ).ToList();

            if (workshops.Any()) return Ok(workshops);
            else return NotFound();

            
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/workshops/{id}")]
        public IHttpActionResult GetGivenWorkshop([FromUri] int id)
        {
            var returnWorkshop = db.Workshops.Where(workshop => workshop.Id == id).Select(workshop => new WorkshopDTO()
            {
                Id = workshop.Id,
                Coach = workshop.Coach,
                Place = workshop.Place,
                ShortDescription = workshop.ShortDescription,
                Title = workshop.Title,
                Date = workshop.Date,
                Description = workshop.Description
            }).ToList();
            if (returnWorkshop.Any()) return Ok(returnWorkshop.First());
            else return NotFound();
        }


    }

}