using ContosoUniversity.Models;
using System;

namespace ContosoUniversity.ViewModels
{
    public class DepartmentDeleteViewModel
    {
        public Department Department { get; set; }
        public string ConcurrencyErrorMessage { get; set; }
    }
}
