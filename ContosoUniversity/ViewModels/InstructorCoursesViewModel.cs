using ContosoUniversity.Helpers;
using ContosoUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.ViewModels
{
    public class InstructorCoursesViewModel
    {
        public Instructor Instructor { get; set; }

        public List<AssignedCourseData> AssignedCourseDataList;
    }
}
