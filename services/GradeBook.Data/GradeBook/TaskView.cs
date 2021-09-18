using System;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class TaskView
    {
        public long Id { get; set; }
        public int GroupId { get; set; }
        public int SemesterId { get; set; }
        public int GradebookId { get; set; }
        public int TaskId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public byte TaskLength { get; set; }
        public byte MaxGrade { get; set; }
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte Grade { get; set; }

        public virtual Group Group { get; set; }
        public virtual Semester Semester { get; set; }
        public virtual Gradebook Gradebook { get; set; }
        public virtual Task Task { get; set; }
        public virtual Student Student { get; set; }
    }
}
