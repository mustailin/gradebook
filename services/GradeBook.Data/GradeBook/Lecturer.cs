using System;
using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class Lecturer
    {
        public Lecturer()
        {
            LecturerHasGradebooks = new HashSet<LecturerHasGradebook>();
        }

        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<LecturerHasGradebook> LecturerHasGradebooks { get; set; }
    }
}
