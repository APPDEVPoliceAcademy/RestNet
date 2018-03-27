using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestNet.Models
{
    public class WorkshopShortDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public DateTime Date { get; set; }
        public string Coach { get; set; }
        public string Place { get; set; }
    }
}