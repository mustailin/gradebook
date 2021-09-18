using System;
using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class AttendanceView
    {
        public long Id { get; set; }
        public int GroupId { get; set; }
        public int SemesterId { get; set; }
        public int GradebookId { get; set; }
        public int TaskId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public byte TaskLength { get; set; }
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long AttendanceId { get; set; }
        public bool Present { get; set; }
        public bool AbsentWithReason { get; set; }
        public bool Absent { get; set; }
        public DateTime ClassDate { get; set; }

        public virtual Student Student { get; set; }
        public virtual Gradebook Gradebook { get; set; }
        public virtual Task Task { get; set; }
        public virtual Semester Semester { get; set; }
        public virtual Group Group { get; set; }
        public virtual StudentAttendance Attendance { get; set; }
    }
}
