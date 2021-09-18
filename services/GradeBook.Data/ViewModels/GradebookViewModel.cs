using GradeBook.Data.GradeBook;
using System.Collections.Generic;

namespace GradeBook.Data.ViewModels
{
    public class GradebookViewModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int SemesterId { get; set; }
        public int FacultyId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }

        public Gradebook Gradebook { get; set; }
        public Group Group { get; set; }
        public Semester Semester { get; set; }
        public Faculty Faculty { get; set; }
        public ICollection<Lecturer> LecturerCollection { get; set; }
    }
}
