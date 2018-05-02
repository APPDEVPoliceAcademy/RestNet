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
        public bool IsEnrolled { get; set; }
        public bool IsEvaluated { get; set; }
        public int NumberOfSpots { get; set; }
        public int TakenSpots { get; set; }
    }
}