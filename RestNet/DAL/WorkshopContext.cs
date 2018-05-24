using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using RestNet.Models;

namespace RestNet.DAL
{
    public class WorkshopContext : DbContext
    {
        public WorkshopContext() : base("WorkshopContext")
        {
            Database.SetInitializer<WorkshopContext>(new CreateDatabaseIfNotExists<WorkshopContext>());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Workshop> Workshops { get; set; }
        public DbSet<Link> Links { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<User>().HasMany<Workshop>(user => user.Workshops).WithMany(workshop => workshop.Users)
                .Map(configuration =>
                {
                    configuration.MapLeftKey("UserID");
                    configuration.MapRightKey("WorkshopID");
                    configuration.ToTable("UserWorkshop");
                });

        }
    }
}