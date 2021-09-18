using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class Group
    {
        public Group()
        {
            Gradebooks = new HashSet<Gradebook>();
            Students = new HashSet<Student>();
        }

        public int Id { get; set; }
        public string Number { get; set; }
        public int FacultyId { get; set; }

        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Gradebook> Gradebooks { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
