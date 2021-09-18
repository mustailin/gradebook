#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class StudentGradebook
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GroupId { get; set; }
        public string Number { get; set; }
        public int SemesterId { get; set; }
        public short AcademicYear { get; set; }
        public string Name { get; set; }
        public int GradebookId { get; set; }
        public string Subject { get; set; }

        public Student Student { get; set; }
        public Group Group { get; set; }
        public Semester Semester { get; set; }
        public Gradebook Gradebook { get; set; }
    }
}
