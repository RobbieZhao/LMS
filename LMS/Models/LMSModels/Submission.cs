using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public string StudentId { get; set; }
        public int AssignmentId { get; set; }
        public DateTime Time { get; set; }
        public int? Score { get; set; }
        public string Content { get; set; }

        public virtual Assignment Assignment { get; set; }
        public virtual Student Student { get; set; }
    }
}
