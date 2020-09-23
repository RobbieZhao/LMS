using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submission = new HashSet<Submission>();
        }

        public int AssignmentId { get; set; }
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public int? MaxPoint { get; set; }
        public string Content { get; set; }
        public DateTime? DueTime { get; set; }

        public virtual AssignmentCategory Category { get; set; }
        public virtual ICollection<Submission> Submission { get; set; }
    }
}
