using GradeBook.Data.GradeBook;
using System.ComponentModel.DataAnnotations;

namespace GradeBook.Data.ViewModels
{
    public class CreateDepartmentViewModel
    {
        [Required]
        public string Name { get; set; }
        public Faculty Faculty { get; set; }
    }
}
