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
    public class DepartmentsController : DepartmentEditController
    {
        public DepartmentsController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Departments.Include(d => d.Administrator);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "LastName");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,Name,Budget,StartDate,InstructorID,RowVersion")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "LastName", department.InstructorID);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DepartmentEditViewModel departmentEditVM = new DepartmentEditViewModel();

            departmentEditVM.Department = await _context.Departments
                //.Include(d => d.Administrator)  // eager loading
                .AsNoTracking()                 // tracking not required
                .FirstOrDefaultAsync(m => m.DepartmentID == id);

            if (departmentEditVM.Department == null)
            {
                return NotFound();
            }

            // Use strongly typed data rather than ViewData.
            departmentEditVM.InstructorNameSL = new SelectList(_context.Instructors, "ID", "LastName", 
                departmentEditVM.Department.InstructorID);

            //ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "LastName", department.InstructorID);

            return View(departmentEditVM);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentID,Name,Budget,StartDate,InstructorID,RowVersion")] Department department)
        {
            if (id != department.DepartmentID)
            {
                return NotFound();
            }

            DepartmentEditViewModel departmentEditVM = new DepartmentEditViewModel();

            if (!ModelState.IsValid)
            {
                departmentEditVM.Department = department;
                departmentEditVM.InstructorNameSL = new SelectList(_context.Instructors, "ID", "LastName",
                    departmentEditVM.Department.InstructorID);

                return View(departmentEditVM);
            }

            var departmentToUpdate = await _context.Departments
                .Include(i => i.Administrator)
                .FirstOrDefaultAsync(m => m.DepartmentID == id);

            if (departmentToUpdate == null)
            {
                departmentEditVM.Department = department;
                departmentEditVM.InstructorNameSL = new SelectList(_context.Instructors, "ID", "LastName",
                    departmentEditVM.Department.InstructorID);
                ModelState.AddModelError(string.Empty,
                    "Unable to save. The department was deleted by another user.");
                return View(departmentEditVM);
            }

            // department.RowVersion is what was in the entity when it was originally fetched in the Get request,
            // which may be different from the value that was in the database when FirstOrDefaultAsync was called
            // in this method, if someone else updated the record before the call to FirstOrDefaultAsync
            _context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = department.RowVersion;

            if (await TryUpdateModelAsync<Department>(
                departmentToUpdate,
                "Department",
                s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Department)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        departmentEditVM.Department = department;
                        departmentEditVM.InstructorNameSL = new SelectList(_context.Instructors, "ID", "LastName",
                            departmentEditVM.Department.InstructorID);
                        ModelState.AddModelError(string.Empty, "Unable to save. " +
                            "The department was deleted by another user.");
                        return View(departmentEditVM);
                    }

                    var dbValues = (Department)databaseEntry.ToObject();
                    await SetDbErrorMessage(dbValues, clientValues);

                    // Save the current RowVersion so next postback
                    // matches unless a new concurrency issue happens.
                    department.RowVersion = (byte[])dbValues.RowVersion;

                    // Clear the model error for the next postback.
                    ModelState.Remove("Department.RowVersion");
                }
            }

            departmentEditVM.Department = department;
            departmentEditVM.InstructorNameSL = new SelectList(_context.Instructors, "ID", "LastName",
                departmentEditVM.Department.InstructorID);

            return View(departmentEditVM);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            DepartmentDeleteViewModel departmentDeleteVM = new DepartmentDeleteViewModel();

            departmentDeleteVM.Department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DepartmentID == id);

            if (departmentDeleteVM.Department == null)
            {
                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                departmentDeleteVM.ConcurrencyErrorMessage = "The record you attempted to delete "
                  + "was modified by another user after you selected delete. "
                  + "The delete operation was canceled and the current values in the "
                  + "database have been displayed. If you still want to delete this "
                  + "record, click the Delete button again.";
            }

            return View(departmentDeleteVM);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, 
            [Bind("DepartmentID,Name,Budget,StartDate,InstructorID,RowVersion")] Department department)
        {
            try
            {
                if (await _context.Departments.AnyAsync(m => m.DepartmentID == id))
                {
                    // Department.rowVersion value is from when the entity
                    // was fetched. If it doesn't match the DB, a
                    // DbUpdateConcurrencyException exception is thrown.
                    _context.Departments.Remove(department);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = id });
            }
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentID == id);
        }
    }
}
