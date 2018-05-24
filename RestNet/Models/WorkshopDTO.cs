using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestNet.Models
{
    public class WorkshopDTO
    {
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public string Coach { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public bool IsEnrolled { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public bool IsEvaluated { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public string EvaluationUri { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public int NumberOfSpots { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public int TakenSpots { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public virtual ICollection<Link> Files { get; set; }
    }
}