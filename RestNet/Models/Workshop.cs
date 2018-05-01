using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestNet.Models
{
    public class Workshop
    {
        public DateTime LastUpdated = DateTime.Now;
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Coach { get; set; }
        public string Place { get; set; }
        public DateTime Date { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<User> EvaluatedUsers { get; set; } = new List<User>();
        public string EvaluationUri { get; set; }
    }
}