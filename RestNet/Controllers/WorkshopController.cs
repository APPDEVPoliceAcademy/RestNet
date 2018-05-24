using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using RestNet.DAL;
using RestNet.Models;
using Swashbuckle.Swagger.Annotations;

namespace RestNet.Controllers
{
    /// <summary>
    /// Controller for workshops
    /// </summary>
    public class WorkshopController : ApiController
    {
        private WorkshopContext db = new WorkshopContext();


        /// <summary>
        /// Enrolls user for given workshop.
        /// </summary>
        /// <remarks>
        /// Need to be authenticated.
        /// User is taken from Bearer token.
        /// </remarks>
        /// <param name="id">Id of workshop</param>
        /// <returns>Nothing</returns>
        /// <response code="200">User is sucesfully enrolled</response>
        /// <response code="400">User is not enrolled</response>  
        [Authorize]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/workshops/enroll/{id}")]
        public IHttpActionResult AddSingleUser([FromUri] int id)
        {
            
            var user = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var userId = Int32.Parse(user.FindFirstValue("UserId"));
            int updated = 0;
            using (var dbContext = new WorkshopContext())
            {
                var currentUser = dbContext.Users.Single(user1 => user1.ID == userId);
                var currentWorkshop = dbContext.Workshops.Single(workshop => workshop.Id == id);
                currentUser.Workshops.Add(currentWorkshop);
                updated = dbContext.SaveChanges();
            }
            
            if (updated == 0) return BadRequest("Couldn't save new info to database");
            return Ok();
        }

        /// <summary>
        /// Marks that user evaulated workshop
        /// </summary>
        /// <remarks>
        /// Need to be authenticated.
        /// User is taken from Bearer token.
        /// </remarks>
        /// <param name="id">Id of workshop</param>
        /// <returns>Nothing</returns>
        /// <response code="200">Workshop is sucesfully evaluated by user</response>
        /// <response code="400">Workshop is not evaluated</response>  
        [Authorize]
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("api/workshops/evaluate/{id}")]
        public IHttpActionResult EvaluteSingleUser([FromUri] int id)
        {
            var user = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var userId = Int32.Parse(user.FindFirstValue("UserId"));


            var userObject = db.Users.First(user1 => user1.ID == userId);
            var selectedWorkshop = db.Workshops.First(workshop => workshop.Id == id);

            selectedWorkshop.EvaluatedUsers.Add(userObject);

            var updated = db.SaveChanges();
            if (updated == 0) return BadRequest("Couldn't save new info to database");
            return Ok();
        }

        /// <summary>
        /// Disenrolls user form given workshop
        /// </summary>
        /// <remarks>
        /// Need to be authenticated.
        /// User is taken from Bearer token.
        /// </remarks>
        /// <param name="id">Id of workshop</param>
        /// <returns>Nothing</returns>
        /// <response code="200">User is sucesfully disenrolled</response>
        /// <response code="400">User is not disenrolled</response>  
        [Authorize]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/workshops/disenroll/{id}")]
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

        /// <summary>
        /// Get list of workshops short information on which user is enrolled
        /// </summary>
        /// <remarks>
        /// Need to be authenticated.
        /// User is taken from Bearer token.
        /// </remarks>
        /// <returns>List of workshops</returns>
        /// <response code="200">List of workshops short information on which user is enrolled</response>
        /// <response code="400">Bad authentication token</response>
        /// <response code="404">No workshop found for user</response>  
        [Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/workshops/me")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<WorkshopShortDTO>))]
        public IHttpActionResult GetForSingleUser()
        {
            var _user = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var id = Int32.Parse(_user.FindFirstValue("UserId"));
            var currentUser = db.Users.FirstOrDefault(user => user.ID == id);

            if (currentUser == null)
            {
                return BadRequest("No user");
            }

            var workshops = currentUser.Workshops.AsEnumerable().Select(workshop => new WorkshopShortDTO()
            {
                Id = workshop.Id,
                Coach = workshop.Coach,
                Place = workshop.Place,
                ShortDescription = workshop.ShortDescription,
                Title = workshop.Title,
                Date = workshop.Date,
                IsEnrolled = true,
                IsEvaluated = workshop.EvaluatedUsers.Any(user => user.ID == id),
                NumberOfSpots = workshop.NumberOfSpots,
                TakenSpots = workshop.Users.Count
            }).ToList();
            if (workshops.Any()) return Ok(workshops);
            else return NotFound();       
        }


        /// <summary>
        /// Get list of all workshops short information
        /// </summary>
        /// <remarks>
        /// Need to be authenticated.
        /// User is taken from Bearer token.
        /// </remarks>
        /// <returns>List of workshops</returns>
        /// <response code="200">List of all workshops short information</response>
        /// <response code="400">Bad authentication token</response>
        /// <response code="404">No workshop found for user</response>  
        [Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/workshops/all")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<WorkshopShortDTO>))]
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
                    IsEnrolled = currentUser.Workshops.Any(workshop1 => workshop1.Id == workshop.Id),
                    IsEvaluated = workshop.EvaluatedUsers.Any(user => user.ID == id),
                    NumberOfSpots = workshop.NumberOfSpots,
                    TakenSpots = workshop.Users.Count
                }).ToList();

                if (allShort.Any()) return Ok(allShort);
                else return NotFound();
            }


        }


        /// <summary>
        /// Get detailed information about single workshop
        /// </summary>
        /// <remarks>
        /// Need to be authenticated.
        /// User is taken from Bearer token.
        /// </remarks>
        /// <param name="id">Id of workshop</param>
        /// <returns>Single workshop detail</returns>
        /// <response code="200">Single workshop detail</response>
        /// <response code="400">Bad authentication token</response>
        /// <response code="404">No workshop found under this id</response>   
        [Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/workshops/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(WorkshopShortDTO))]
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
                var test = db.Workshops.Where(workshop => workshop.Id == id).AsEnumerable().FirstOrDefault();

                var returnWorkshop = db.Workshops.Where(workshop => workshop.Id == id).AsEnumerable().Select(workshop => new WorkshopDTO()
                {
                    Id = workshop.Id,
                    Coach = workshop.Coach,
                    Place = workshop.Place,
                    ShortDescription = workshop.ShortDescription,
                    Title = workshop.Title,
                    Date = workshop.Date,
                    Description = workshop.Description,
                    IsEnrolled = currentUser.Workshops.Any(workshop1 => workshop1.Id == workshop.Id),
                    IsEvaluated = workshop.EvaluatedUsers.Any(user => user.ID == currentUser.ID),
                    EvaluationUri = workshop.EvaluationUri,
                    NumberOfSpots = workshop.NumberOfSpots,
                    TakenSpots = workshop.Users.Count,
                    Files = workshop.Files
                }).ToList();
                if (returnWorkshop.Any()) return Ok(returnWorkshop.First());
                return NotFound();
            }
            
        }


        /// <summary>
        /// Get list of days with workshops for given year/month
        /// </summary>
        /// <remarks>
        /// Need to be authenticated.
        /// User is taken from Bearer token.
        /// </remarks>
        /// <param name="year">Year from which workshops will be selected</param>
        /// <param name="month">Month from which workshops will be selected</param>
        /// <returns>List of ints representing days with workshops for given year/month</returns>
        /// <response code="200">List of days with workshops for given year/month</response>
        /// <response code="400">Bad authentication token</response>
        /// <response code="404">No workshop found for this year/month combination</response>   
        [Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/workshops/days")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<int>))]
        public IHttpActionResult GetForGivenMonth([FromUri] int year, [FromUri] int month)
        {
            var _user = (System.Security.Claims.ClaimsIdentity) User.Identity;
            var _userid = Int32.Parse(_user.FindFirstValue("UserId"));

            var currentUser = db.Users.FirstOrDefault(user => user.ID == _userid);
            if (currentUser == null)
            {
                return BadRequest("No user");
            }

            var returnWorkshop = db.Workshops
                .Where(workshop => workshop.Date.Year == year && workshop.Date.Month == month)
                .Select(workshop => workshop.Date.Day).ToList();

            if (returnWorkshop.Any()) return Ok(returnWorkshop);
            return NotFound();
        }


        /// <summary>
        /// Get list of workshops short information for given day
        /// </summary>
        /// <remarks>
        /// Need to be authenticated.
        /// User is taken from Bearer token.
        /// </remarks>
        /// <param name="year">Year from which workshops will be selected</param>
        /// <param name="month">Month from which workshops will be selected</param>
        /// <param name="day">Day from which workshops will be selected</param>
        /// <returns>List of workshops short information for given day</returns>
        /// <response code="200">List of workshop for year/month combination</response>
        /// <response code="400">Bad authentication token</response>
        /// <response code="404">No workshop found for this year/month combination</response>   
        [Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/workshops/day")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<WorkshopShortDTO>))]
        public IHttpActionResult GetForGivenDay([FromUri] int year, [FromUri] int month, [FromUri] int day)
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
                var allShort = db.Workshops.Where(workshop => workshop.Date.Year == year &&
                                                              workshop.Date.Month == month &&
                                                              workshop.Date.Day == day).AsEnumerable().Select(workshop => new WorkshopShortDTO()
                {
                    Coach = workshop.Coach,
                    Id = workshop.Id,
                    Place = workshop.Place,
                    ShortDescription = workshop.ShortDescription,
                    Title = workshop.Title,
                    Date = workshop.Date,
                    IsEnrolled = currentUser.Workshops.Any(workshop1 => workshop1.Id == workshop.Id),
                    IsEvaluated = workshop.EvaluatedUsers.Any(user => user.ID == id),
                    NumberOfSpots = workshop.NumberOfSpots,
                    TakenSpots = workshop.Users.Count
                }).ToList();

                return Ok(allShort);
            }
        }

        /// <summary>
        /// Get number of not evaluated workshops by user
        /// </summary>
        /// <remarks>
        /// Need to be authenticated.
        /// User is taken from Bearer token.
        /// </remarks>
        /// <returns>Integer representing number of not evaluated workshops by user</returns>
        /// <response code="200">Number of not evaluated workshops by user</response>
        /// <response code="400">Bad authentication token</response>
        [Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/workshops/nonevaluated")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(int))]
        public IHttpActionResult GetNumberOfNonEvaluated()
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
                var totalCount = currentUser.Workshops.Count(workshop =>
                    workshop.Date < DateTime.Now && !workshop.EvaluatedUsers.Contains(currentUser));
                return Ok(totalCount);
            }
        }

        //==============Admin Part ======================//

        
       [Authorize(Roles = "admin")]
       [System.Web.Http.HttpGet]
       [System.Web.Http.Route("api/admin/workshops/all/{id}")]
       public IHttpActionResult GetAllAdmin([FromUri] int id)
       {
           
           try
           {
               var Workshop = db.Workshops.Where(workshop => workshop.Id == id).AsEnumerable().FirstOrDefault();
               if (Workshop != null)
               {
                   return Ok(Workshop.Users.Count);
               }
               else
               {
                   return Ok(0);
               }
           }
           catch (Exception ex)
           {
               return InternalServerError(ex);
           }
           //return Ok(allWorkshops);
       }
       


        [Authorize(Roles = "admin")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/admin/workshops/add")]
        public IHttpActionResult AddWorkshopAdmin([System.Web.Http.FromBody] WorkshopEssential workshop)
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
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/admin/workshops/delete/{id}")]
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
        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("api/admin/workshops/update/{id}")]
        public IHttpActionResult UpdateWorkshopAdmin([FromUri] int id, [System.Web.Http.FromBody] WorkshopEssential updatedWorkshop)
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