using GradeBook.Data.GradeBook;
using System.Collections.Generic;

namespace GradeBook.Data.ViewModels
{
    public class StudentGradebookViewModel
    {
        public int Id { get; set; }
        public int GradebookId { get; set; }
        public string Subject { get; set; }
        public int GroupId { get; set; }
        public int SemesterId { get; set; }
        public int StudentId { get; set; }
        public int AcademicYear { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }

        public Gradebook Gradebook { get; set; }
        public Group Group { get; set; }
        public Semester Semester { get; set; }
        public Student Student { get; set; }
        public ICollection<Lecturer> LecturerCollection { get; set; }
    }
}
