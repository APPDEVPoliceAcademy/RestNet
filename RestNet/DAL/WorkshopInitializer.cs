using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using RestNet.Models;

namespace RestNet.DAL
{
    public class WorkshopInitializer : System.Data.Entity.DropCreateDatabaseAlways<WorkshopContext>
    {
        protected override void Seed(WorkshopContext context)
        {
            SHA256 hasher = new SHA256Managed();
            const string loremipsum =
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim.";
            

            var users = new List<User>()
            {
                new User()
                {
                    Login = "aaa@wp.pl",
                    Name = "Czesc",
                    Password = Encoding.UTF8.GetString(hasher.ComputeHash(Encoding.UTF8.GetBytes("Grzes"))),
                    Surname = "Janek",
                    Unit = Unit.Nord,
                    Birthday = new DateTime(1996, 02, 15),
                    Rights = Rights.admin

                }
            };

            users.ForEach(user => context.Users.Add(user));

            var workshops = new List<Workshop>()
            {
                new Workshop()
                {
                    Title = "PR for beginners",
                    Description = loremipsum,
                    Coach = "Andrzej Nowak",
                    Date = new DateTime(2014, 06, 13),
                    Place = "Windesheim",
                    Users = null,
                    EvaluationUri = "https://response.questback.com/politieacademiecod/l3suj4uuxh",
                    NumberOfSpots = 10
                },

                new Workshop()
                {
                    Title = "Team Project",
                    ShortDescription = "latestTest",
                    Description = loremipsum,
                    Date = new DateTime(2019, 06, 21),
                    Coach = "Tadeusz Sznuk",
                    Place = "on-line",
                    Users = null,
                    EvaluationUri = "https://response.questback.com/politieacademiecod/l3suj4uuxh",
                    NumberOfSpots = 15
                    
                },
                new Workshop()
                {
                    Title = "Team Building",
                    ShortDescription = "firstAlphabeticalPlace",
                    Description = loremipsum,
                    Coach = "Andrzej Norek",
                    Place = "Apeldorn",
                    Date = new DateTime(2018, 06, 21),
                    Users = null,
                    EvaluationUri = "https://response.questback.com/politieacademiecod/l3suj4uuxh",
                    NumberOfSpots = 20,
                    
                },

                new Workshop()
                {
                    Title = "Leadership in practise",
                    ShortDescription = "lastAlphabeticalPlace",
                    Description = loremipsum,
                    Coach = "Andrzej Nowak",
                    Date = new DateTime(2018, 06, 10),
                    Place = "Zwolle",
                    Users = null,
                    EvaluationUri = "https://response.questback.com/politieacademiecod/l3suj4uuxh",
                    NumberOfSpots = 45
                },
                new Workshop()
                {
                    Title = "Motivation",
                    ShortDescription = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit",
                    Description = loremipsum,
                    Date = new DateTime(2018, 09, 21),

                    Coach = "Andrzej Norek",
                    Place = "on-line",
                    Users = null,
                    EvaluationUri = "https://response.questback.com/politieacademiecod/l3suj4uuxh",
                    NumberOfSpots = 90
                }
            };
            workshops.ForEach(workshop => context.Workshops.Add(workshop));



            try
            {
                context.SaveChanges();

            }   

            catch (DbEntityValidationException e)
            {
                Console.WriteLine("ELKO");
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            var selectedWorkshop = context.Workshops.First(workshop => workshop.Title == "Team Building");

            selectedWorkshop.Files.Add(new Link() { Uri = "https://www.pdf-archive.com/2016/05/02/lorem-ipsum/lorem-ipsum.pdf" });
            selectedWorkshop.Files.Add(new Link() { Uri = "http://szgrabowski.kis.p.lodz.pl/zpo17/lab03.pdf" });
            selectedWorkshop.Files.Add(new Link() { Uri = "http://szgrabowski.kis.p.lodz.pl/zpo17/lab04.pdf" });
            try
            {
                context.SaveChanges();

            }

            catch (DbEntityValidationException e)
            {
                Console.WriteLine("ELKO");
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }

    }
}