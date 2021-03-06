﻿using System;
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
    public class InstructorsController : InstructorCoursesController
    {
        public InstructorsController(ApplicationDbContext context) : base(context)
        {
        }

        public InstructorIndexData InstructorData { get; set; }

        // GET: Instructors
        public async Task<IActionResult> Index(int? id, int? courseID)
        {
            //return View(await _context.Instructors.ToListAsync());

            InstructorData = new InstructorIndexData();
            InstructorData.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Department)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Enrollments)
                            .ThenInclude(i => i.Student)
                .AsNoTracking()
                .OrderBy(i => i.LastName)
                .ToListAsync();

            if (id != null)
            {
                InstructorData.InstructorID = id.Value;
                Instructor instructor = InstructorData.Instructors
                    .Where(i => i.ID == id.Value).Single();
                InstructorData.Courses = instructor.CourseAssignments.Select(s => s.Course);
                //Remember instructor name
                InstructorData.InstructorName = instructor.FullName;
            }

            if (courseID != null)
            {
                InstructorData.CourseID = courseID.Value;
                var selectedCourse = InstructorData.Courses
                    .Where(x => x.CourseID == courseID).Single();
                InstructorData.Enrollments = selectedCourse.Enrollments;
                //Remember course
                InstructorData.CourseName = selectedCourse.CourseName;
            }

            return View(InstructorData);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        public IActionResult Create()
        {
            InstructorCoursesViewModel instructorCoursesVM = new InstructorCoursesViewModel();

            instructorCoursesVM.Instructor = new Instructor();
            instructorCoursesVM.Instructor.CourseAssignments = new List<CourseAssignment>();

            // Provides an empty collection for the foreach loop
            // foreach (var course in Model.AssignedCourseDataList)
            // in the Create Razor page.
            PopulateAssignedCourseData(instructorCoursesVM.Instructor);
            instructorCoursesVM.AssignedCourseDataList = this.AssignedCourseDataList;

            return View(instructorCoursesVM);
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[] selectedCourses, 
            [Bind("ID,LastName,FirstMidName,HireDate")] Instructor instructor)
        {
            InstructorCoursesViewModel instructorCoursesVM = new InstructorCoursesViewModel();

            instructorCoursesVM.Instructor = new Instructor();

            if (selectedCourses != null)
            {
                instructorCoursesVM.Instructor.CourseAssignments = new List<CourseAssignment>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = new CourseAssignment
                    {
                        // Note that only CourseID is populated because InstructorID does not exist
                        // until new instructor is created by SaveChangesAsync() below
                        CourseID = int.Parse(course)
                    };
                    instructorCoursesVM.Instructor.CourseAssignments.Add(courseToAdd);
                }
            }

            if (await TryUpdateModelAsync<Instructor>(
                instructorCoursesVM.Instructor,
                "Instructor",
                i => i.FirstMidName, i => i.LastName,
                i => i.HireDate, i => i.OfficeAssignment))
            {
                _context.Instructors.Add(instructorCoursesVM.Instructor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Something went wrong. Redirect to create instructor view
            PopulateAssignedCourseData(instructorCoursesVM.Instructor);
            instructorCoursesVM.AssignedCourseDataList = this.AssignedCourseDataList;

            return View(instructorCoursesVM);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            InstructorCoursesViewModel instructorCoursesVM = new InstructorCoursesViewModel();

            //var instructor = await _context.Instructors.FindAsync(id);

            instructorCoursesVM.Instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (instructorCoursesVM.Instructor == null)
            {
                return NotFound();
            }

            PopulateAssignedCourseData(instructorCoursesVM.Instructor);

            instructorCoursesVM.AssignedCourseDataList = this.AssignedCourseDataList;

            return View(instructorCoursesVM);
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedCourses,
            [Bind("ID,LastName,FirstMidName,HireDate")] Instructor instructor)
        {
            if (id != instructor.ID)
            {
                return NotFound();
            }

            InstructorCoursesViewModel instructorCoursesVM = new InstructorCoursesViewModel();

            instructorCoursesVM.Instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (instructorCoursesVM.Instructor == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Instructor>(
                instructorCoursesVM.Instructor,
                "Instructor",
                i => i.FirstMidName, i => i.LastName,
                i => i.HireDate, i => i.OfficeAssignment))
            {
                if (String.IsNullOrWhiteSpace(
                    instructorCoursesVM.Instructor.OfficeAssignment?.Location))
                {
                    instructorCoursesVM.Instructor.OfficeAssignment = null;
                }
                UpdateInstructorCourses(selectedCourses, instructorCoursesVM.Instructor);

                try
                {
                    //_context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.ID))
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

            //These method calls restore the assigned course data entered on the page, when it is redisplayed with an error message
            UpdateInstructorCourses(selectedCourses, instructorCoursesVM.Instructor);
            PopulateAssignedCourseData(instructorCoursesVM.Instructor);
            instructorCoursesVM.AssignedCourseDataList = this.AssignedCourseDataList;

            return View(instructorCoursesVM);
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.FindAsync(id);

            if (instructor == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var departments = await _context.Departments
                .Where(d => d.InstructorID == id)
                .ToListAsync();

            // No cascade delete for nullable FKs, by default. Would not want to delete department anyway,
            // if instructor in charge of department is deleted from db, so set InstructorID to null
            departments.ForEach(d => d.InstructorID = null);

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }
    }
}
