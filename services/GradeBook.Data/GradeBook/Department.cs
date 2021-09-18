using System;
using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class Department
    {
        public Department()
        {
            Lecturers = new HashSet<Lecturer>();
        }

        public int Id { get; set; }
        public int FacultyId { get; set; }
        public string Name { get; set; }

        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Lecturer> Lecturers { get; set; }
    }
}
