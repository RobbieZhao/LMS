using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategory = new HashSet<AssignmentCategory>();
            Enroll = new HashSet<Enroll>();
        }

        public int ClassId { get; set; }
        public uint Year { get; set; }
        public string Season { get; set; }
        public string Location { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string ProfessorId { get; set; }
        public int CourseId { get; set; }

        public virtual Course Course { get; set; }
        public virtual Professor Professor { get; set; }
        public virtual ICollection<AssignmentCategory> AssignmentCategory { get; set; }
        public virtual ICollection<Enroll> Enroll { get; set; }
    }
}
