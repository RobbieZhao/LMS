using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professor
    {
        public Professor()
        {
            Class = new HashSet<Class>();
        }

        public string UId { get; set; }
        public string DepartmentAbbr { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DoB { get; set; }

        public virtual Department DepartmentAbbrNavigation { get; set; }
        public virtual ICollection<Class> Class { get; set; }
    }
}
