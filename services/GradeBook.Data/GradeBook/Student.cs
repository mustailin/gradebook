using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class Student : BaseUser
    {
        public Student()
        {
            StudentAttendances = new HashSet<StudentAttendance>();
            StudentGrades = new HashSet<StudentGrade>();
        }

        public int GroupId { get; set; }

        public virtual Group Group { get; set; }
        public virtual ICollection<StudentAttendance> StudentAttendances { get; set; }
        public virtual ICollection<StudentGrade> StudentGrades { get; set; }
    }
}
