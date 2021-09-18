using System;
using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class Task
    {
        public Task()
        {
            StudentAttendances = new HashSet<StudentAttendance>();
            StudentGrades = new HashSet<StudentGrade>();
        }

        public int Id { get; set; }
        public int GradebookId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public byte TaskLength { get; set; }
        public bool OnCourseMon { get; set; }
        public bool OnCourseTue { get; set; }
        public bool OnCourseWed { get; set; }
        public bool OnCourseThu { get; set; }
        public bool OnCourseFri { get; set; }
        public byte MaxGrade { get; set; }
        public string Description { get; set; }

        public virtual Gradebook Gradebook { get; set; }
        public virtual ICollection<StudentAttendance> StudentAttendances { get; set; }
        public virtual ICollection<StudentGrade> StudentGrades { get; set; }
    }
}
