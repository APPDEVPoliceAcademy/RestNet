using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestNet.Models
{
    public class WorkshopDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Coach { get; set; }
        public string Place { get; set; }
        public DateTime Date { get; set; }
        public bool IsEnrolled { get; set; }
        public bool IsEvaluated { get; set; }
        public string EvaluationUri { get; set; }
        public int NumberOfSpots { get; set; }
        public int TakenSpots { get; set; }
    }
}