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
            var workshops = new List<Workshop>()
            {
                new Workshop()
                {
                    Title = "PR for beginners",
                    Description = loremipsum,
                    Coach = "Andrzej Nowak",
                    Date = new DateTime(2014, 06, 13),
                    Place = "Windesheim",
                    Users = null
                },

                new Workshop()
                {
                    Title = "Team Project",
                    ShortDescription = "latestTest",
                    Description = loremipsum,
                    Date = new DateTime(2019, 06, 21),
                    Coach = "Tadeusz Sznuk",
                    Place = "on-line",
                    Users = null
                },
                new Workshop()
                {
                    Title = "Team Building",
                    ShortDescription = "firstAlphabeticalPlace",
                    Description = loremipsum,
                    Coach = "Andrzej Norek",
                    Place = "Apeldorn",
                    Date = new DateTime(2018, 06, 21),
                    Users = null             
                },

                new Workshop()
                {
                    Title = "Leadership in practise",
                    ShortDescription = "lastAlphabeticalPlace",
                    Description = loremipsum,
                    Coach = "Andrzej Nowak",
                    Date = new DateTime(2018, 06, 10),
                    Place = "Zwolle",
                    Users = null
                },
                new Workshop()
                {
                    Title = "Motivation",
                    ShortDescription = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit",
                    Description = loremipsum,
                    Date = new DateTime(2018, 09, 21),

                    Coach = "Andrzej Norek",
                    Place = "on-line",
                    Users = null
                }
            };

            workshops.ForEach(workshop => context.Workshops.Add(workshop));

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