using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RestNet.Models
{
    public enum Rights
    {
        user, admin
    }

    public class User
    {
        public int ID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Unit { get; set; }
        public Rights? Rights { get; set; }
    }
}