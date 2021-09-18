using System;
using System.Collections.Generic;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class UserRole
    {
        public string UserEmail { get; set; }
        public string Roles { get; set; }

        public virtual User UserEmailNavigation { get; set; }
    }
}
