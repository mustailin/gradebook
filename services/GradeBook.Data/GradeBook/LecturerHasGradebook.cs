#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class LecturerHasGradebook
    {
        public int GradebookId { get; set; }
        public int LecturerId { get; set; }

        public virtual Gradebook Gradebook { get; set; }
        public virtual Lecturer Lecturer { get; set; }
    }
}
