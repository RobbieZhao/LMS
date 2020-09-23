using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Student
    {
        public Student()
        {
            Enroll = new HashSet<Enroll>();
            Submission = new HashSet<Submission>();
        }

        public string UId { get; set; }
        public string DepartmentAbbr { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DoB { get; set; }

        public virtual Department DepartmentAbbrNavigation { get; set; }
        public virtual ICollection<Enroll> Enroll { get; set; }
        public virtual ICollection<Submission> Submission { get; set; }
    }
}
