using GradeBook.Data.GradeBook;
using System.ComponentModel.DataAnnotations;

namespace GradeBook.Data.ViewModels
{
    public class LecturerViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public Department Department { get; set; }
    }
}
