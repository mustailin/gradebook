#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class LecturerGradebook
    {
        public long Id { get; set; }
        public int LecturerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GroupId { get; set; }
        public string Number { get; set; }
        public int SemesterId { get; set; }
        public short AcademicYear { get; set; }
        public string Name { get; set; }
        public int GradebookId { get; set; }
        public string Subject { get; set; }

        public virtual Gradebook Gradebook { get; set; }
        public virtual Lecturer Lecturer { get; set; }
        public virtual Group Group { get; set; }
        public virtual Semester Semester { get; set; }
    }
}
