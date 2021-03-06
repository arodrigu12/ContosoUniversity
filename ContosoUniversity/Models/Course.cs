﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Range(0, 5)]
        public int Credits { get; set; }

        public string CourseName
        {
            get { return Title + " " + CourseID; }
        }

        public int DepartmentID { get; set; }
        public Department Department { get; set; }
    
        public IList<Enrollment> Enrollments { get; set; }
        public IList<CourseAssignment> CourseAssignments { get; set; }
    }
}