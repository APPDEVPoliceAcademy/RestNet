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

        
        [Authorize]
        [HttpPost]
        [Route("api/workshops/enroll/{id}")]
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
        [HttpDelete]
        [Route("api/workshops/disenroll/{id}")]
        public IHttpActionResult RemoveSingleUser([FromUri] int id)
        {
            var user = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var userId = Int32.Parse(user.FindFirstValue("UserId"));


            var userObject = db.Users.First(user1 => user1.ID == userId);
            var selectedWorkshop = db.Workshops.First(workshop => workshop.Id == id);

            selectedWorkshop.Users.Remove(userObject);
            userObject.Workshops.Remove(selectedWorkshop);

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
                Date = workshop.Date,
                IsEnrolled = true
            }).ToList();
            if (workshops.Any()) return Ok(workshops);
            else return NotFound();       
        }

        [Authorize]
        [HttpGet]
        [Route("api/workshops/all")]
        public IHttpActionResult GetAll()
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
                var allShort = db.Workshops.AsEnumerable().Select(workshop => new WorkshopShortDTO()
                {
                    Coach = workshop.Coach,
                    Id = workshop.Id,
                    Place = workshop.Place,
                    ShortDescription = workshop.ShortDescription,
                    Title = workshop.Title,
                    Date = workshop.Date,
                    IsEnrolled = currentUser.Workshops.Any(workshop1 => workshop1.Id == workshop.Id)
                }).ToList();

                return Ok(allShort);
            }


        }

        [Authorize]
        [HttpGet]
        [Route("api/workshops/{id}")]
        public IHttpActionResult GetGivenWorkshop([FromUri] int id)
        {
            var _user = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var _userid = Int32.Parse(_user.FindFirstValue("UserId"));

            var currentUser = db.Users.FirstOrDefault(user => user.ID == _userid);
            if (currentUser == null)
            {
                return BadRequest("No user");
            }
            else
            {
                var returnWorkshop = db.Workshops.Where(workshop => workshop.Id == id).AsEnumerable().Select(workshop => new WorkshopDTO()
                {
                    Id = workshop.Id,
                    Coach = workshop.Coach,
                    Place = workshop.Place,
                    ShortDescription = workshop.ShortDescription,
                    Title = workshop.Title,
                    Date = workshop.Date,
                    Description = workshop.Description,
                    IsEnrolled = currentUser.Workshops.Any(workshop1 => workshop1.Id == workshop.Id)
                }).ToList();
                if (returnWorkshop.Any()) return Ok(returnWorkshop.First());
                return NotFound();
            }
            
        }

        //==============Admin Part ======================//

        
       [AllowAnonymous]
       [HttpGet]
       [Route("api/admin/workshops/all")]
       public IHttpActionResult GetAllAdmin()
       {
           List<Workshop> allWorkshops;
           try
           {
               allWorkshops = db.Workshops.ToList();
           }
           catch (Exception ex)
           {
               return InternalServerError(ex);
           }
           return Ok(allWorkshops);
       }
       


        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/admin/workshops/add")]
        public IHttpActionResult AddWorkshopAdmin([FromBody] WorkshopEssential workshop)
        {
            var newWorkshop = new Workshop()
            {
                Coach = workshop.Coach,
                Date = workshop.Date,
                Description = workshop.Description,
                Place = workshop.Place,
                ShortDescription = workshop.ShortDescription
            };
            try
            {
                db.Workshops.Add(newWorkshop);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok();

        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/admin/workshops/delete/{id}")]
        public IHttpActionResult DeleteWorkshopAdmin([FromUri] int id)
        {
            try
            {
                var workshop = db.Workshops.FirstOrDefault(workshop1 => workshop1.Id == id);
                if (workshop == null)
                {
                    return NotFound();
                }
                db.Workshops.Remove(workshop);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok();
        }

        [Authorize(Roles = "admin")]
        [HttpPatch]
        [Route("api/admin/workshops/update/{id}")]
        public IHttpActionResult UpdateWorkshopAdmin([FromUri] int id, [FromBody] WorkshopEssential updatedWorkshop)
        {
            try
            {
                var workshop = db.Workshops.FirstOrDefault(workshop1 => workshop1.Id == id);
                if (workshop != null)
                {
                    workshop.Coach = updatedWorkshop.Coach;
                    workshop.Date = updatedWorkshop.Date;
                    workshop.Description = updatedWorkshop.Description;
                    workshop.ShortDescription = updatedWorkshop.ShortDescription;
                    workshop.Place = updatedWorkshop.Place;
                    workshop.Title = updatedWorkshop.Title;
                    return Ok();
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }

}