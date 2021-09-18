using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class Semester
    {
        public Semester()
        {
            Gradebooks = new HashSet<Gradebook>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public short AcademicYear { get; set; }

        public virtual ICollection<Gradebook> Gradebooks { get; set; }
    }
}
