using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.ViewModels;

namespace ContosoUniversity.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Courses.Include(c => c.Department);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .AsNoTracking() // improves performance when tracking isn't required.
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            CreateEditCourseViewModel createEditCourseViewModel =
                new CreateEditCourseViewModel(_context.Departments.OrderBy(n => n.Name).ToList());

            return View(createEditCourseViewModel);
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Title,Credits,DepartmentID")] CreateEditCourseViewModel createEditCourseVM)
        {
            if (ModelState.IsValid)
            {
                Course newCourse = new Course
                {
                    CourseID = createEditCourseVM.CourseID,
                    Title = createEditCourseVM.Title,
                    Credits = createEditCourseVM.Credits,
                    DepartmentID = createEditCourseVM.DepartmentID
                };
                _context.Add(newCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(createEditCourseVM);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            List<Department> departmentList = await _context.Departments.OrderBy(n => n.Name).ToListAsync();

            CreateEditCourseViewModel createEditCourseVM =
                new CreateEditCourseViewModel(departmentList)
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Credits = course.Credits,
                    DepartmentID = course.DepartmentID
                };

            return View(createEditCourseVM);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseID,Title,Credits,DepartmentID")] CreateEditCourseViewModel createEditCourseVM)
        {
            if (id != createEditCourseVM.CourseID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Course course = new Course
                {
                    CourseID = createEditCourseVM.CourseID,
                    Title = createEditCourseVM.Title,
                    Credits = createEditCourseVM.Credits,
                    DepartmentID = createEditCourseVM.DepartmentID
                };

                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(createEditCourseVM);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .AsNoTracking() // improves performance when tracking isn't required.
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseID == id);
        }
    }
}
