using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enroll
    {
        public string StudentId { get; set; }
        public int ClassId { get; set; }
        public string Grade { get; set; }

        public virtual Class Class { get; set; }
        public virtual Student Student { get; set; }
    }
}
