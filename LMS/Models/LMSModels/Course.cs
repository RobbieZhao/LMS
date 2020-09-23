using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Class = new HashSet<Class>();
        }

        public int CourseId { get; set; }
        public string DepartmentAbbr { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }

        public virtual Department DepartmentAbbrNavigation { get; set; }
        public virtual ICollection<Class> Class { get; set; }
    }
}
