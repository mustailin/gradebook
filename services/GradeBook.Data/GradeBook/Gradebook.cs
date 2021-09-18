using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class Gradebook
    {
        public Gradebook()
        {
            LecturerHasGradebooks = new HashSet<LecturerHasGradebook>();
            Tasks = new HashSet<Task>();
        }

        public int Id { get; set; }
        public int SemesterId { get; set; }
        public int GroupId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }

        public virtual Group Group { get; set; }
        public virtual Semester Semester { get; set; }
        public virtual ICollection<LecturerHasGradebook> LecturerHasGradebooks { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
