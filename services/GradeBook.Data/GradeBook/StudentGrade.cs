using System;
using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class StudentGrade
    {
        public int StudentId { get; set; }
        public int TaskId { get; set; }
        public byte Grade { get; set; }

        public virtual Student Student { get; set; }
        public virtual Task Task { get; set; }
    }
}
