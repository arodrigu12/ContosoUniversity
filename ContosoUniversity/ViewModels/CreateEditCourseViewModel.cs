using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ContosoUniversity.ViewModels
{
    public class CreateEditCourseViewModel
    {
        [Display(Name = "Number")]
        [Required]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        [Range(0, 5)]
        [Required]
        public int Credits { get; set; }

        [Display(Name = "Department")]
        [Required]
        public int DepartmentID { get; set; }
        public SelectList DepartmentNameSL { get; set; }

        public CreateEditCourseViewModel() { }
        public CreateEditCourseViewModel(IEnumerable<Department> departments )
        {
            DepartmentNameSL = new SelectList(departments,"DepartmentID", "Name");
        }
    }
}
