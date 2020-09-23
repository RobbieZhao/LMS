﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCategory
    {
        public AssignmentCategory()
        {
            Assignment = new HashSet<Assignment>();
        }

        public int CategoryId { get; set; }
        public int ClassId { get; set; }
        public int? Weight { get; set; }
        public string Name { get; set; }

        public virtual Class Class { get; set; }
        public virtual ICollection<Assignment> Assignment { get; set; }
    }
}
