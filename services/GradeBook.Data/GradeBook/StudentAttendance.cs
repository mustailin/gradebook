using System;
using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class StudentAttendance
    {
        public long Id { get; set; }
        public int StudentId { get; set; }
        public int TaskId { get; set; }
        public DateTime ClassDate { get; set; }
        public bool Present { get; set; }
        public bool Absent { get; set; }
        public bool AbsentWithReason { get; set; }

        public virtual Student Student { get; set; }
        public virtual Task Task { get; set; }
    }
}
