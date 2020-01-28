using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Data;
using ContosoUniversity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContosoUniversity.Controllers
{
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<EnrollmentDateGroup> Students { get; set; }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            IQueryable<EnrollmentDateGroup> data = _context.Students.
                GroupBy(s => s.EnrollmentDate).
                Select(dateGroup => new EnrollmentDateGroup()
                {
                    EnrollmentDate = dateGroup.Key,
                    StudentCount = dateGroup.Count()
                });

                //from student in _context.Students
                //group student by student.EnrollmentDate into dateGroup
                //select new EnrollmentDateGroup()
                //{
                //    EnrollmentDate = dateGroup.Key,
                //    StudentCount = dateGroup.Count()
                //};

            Students = await data.AsNoTracking().ToListAsync();
            return View(Students);
        }
    }
}
