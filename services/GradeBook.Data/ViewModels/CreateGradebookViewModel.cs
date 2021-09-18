using System.Collections.Generic;

namespace GradeBook.Data.ViewModels
{
    public class CreateGradebookViewModel
    {
        public int GroupId { get; set; }
        public string Subject { get; set; }
        public ICollection<int> LecturerCollection { get; set; }
        public int SemesterId { get; set; }
        public string Description { get; set; }
    }
}
