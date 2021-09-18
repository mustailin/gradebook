#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class User
    {
        public string Email { get; set; }
        public bool IsAccountNonExpired { get; set; }
        public bool IsAccountNonLocked { get; set; }
        public bool IsCredentialsNonExpired { get; set; }
        public bool IsEnabled { get; set; }
        public string Password { get; set; }
    }
}
