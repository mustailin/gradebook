using GradeBook.Data.GradeBook;
using System.ComponentModel.DataAnnotations;

namespace GradeBook.Data.ViewModels
{
    public class CreateGroupViewModel
    {
        [Required]
        public string Number { get; set; }
        public Faculty Faculty { get; set; }
    }
}
