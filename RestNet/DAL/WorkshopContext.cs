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

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Workshop> Workshops { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}