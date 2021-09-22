using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class Lecturer : BaseUser
    {
        public Lecturer()
        {
            LecturerHasGradebooks = new HashSet<LecturerHasGradebook>();
        }

        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<LecturerHasGradebook> LecturerHasGradebooks { get; set; }
    }
}
