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
        [HttpPost]
        [Route("api/workshops/{id}")]
        public IHttpActionResult AddSingleUser([FromUri] int id)
        {
            var user = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var userId = Int32.Parse(user.FindFirstValue("UserId"));


            var userObject = db.Users.First(user1 => user1.ID == userId);
            var selectedWorkshop = db.Workshops.First(workshop => workshop.Id == id);

            selectedWorkshop.Users.Add(userObject);
            userObject.Workshops.Add(selectedWorkshop);

            var updated = db.SaveChanges();
            if (updated == 0) return BadRequest("Couldn't save new info to database");
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("api/workshops/me")]
        public IHttpActionResult GetForSingleUser()
        {
            var _user = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var id = Int32.Parse(_user.FindFirstValue("UserId"));


            var workshops = db.Users.First(user => user.ID == id).Workshops.Select(workshop => new WorkshopShortDTO()
            {
                Id = workshop.Id,
                Coach = workshop.Coach,
                Place = workshop.Place,
                ShortDescription = workshop.ShortDescription,
                Title = workshop.Title,
                Date = workshop.Date
            }).ToList();
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
            return NotFound();
        }


    }

}