using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestNet.Models
{
    /// <summary>
    /// Data transfer object of short information about workshop
    /// </summary>
    public class WorkshopShortDTO
    {
        /// <summary>
        /// Id of workshops
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Title of workshop
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Short description of workshops
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Date of workshop
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Person Conducting Workshop
        /// </summary>
        public string Coach { get; set; }
        /// <summary>
        /// Location of workshop
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// Is user enrolled on this workshop
        /// </summary>
        public bool IsEnrolled { get; set; }
        /// <summary>
        /// Is the workshops evaluated by user
        /// </summary>
        public bool IsEvaluated { get; set; }
        /// <summary>
        /// Total number of spots
        /// </summary>
        public int NumberOfSpots { get; set; }
        /// <summary>
        /// Number of taken spots
        /// </summary>
        public int TakenSpots { get; set; }
        
    }
}