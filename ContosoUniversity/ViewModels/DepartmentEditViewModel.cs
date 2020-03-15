using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.ViewModels
{
    public class DepartmentEditViewModel
    {
        public Department Department { get; set; }

        // Replaces ViewData["InstructorID"] 
        public SelectList InstructorNameSL { get; set; }
    }
}
