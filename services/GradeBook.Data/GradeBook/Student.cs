using System;
using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class Student
    {
        public Student()
        {
            StudentAttendances = new HashSet<StudentAttendance>();
            StudentGrades = new HashSet<StudentGrade>();
        }

        public int Id { get; set; }
        public int GroupId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public virtual Group Group { get; set; }
        public virtual ICollection<StudentAttendance> StudentAttendances { get; set; }
        public virtual ICollection<StudentGrade> StudentGrades { get; set; }
    }
}
