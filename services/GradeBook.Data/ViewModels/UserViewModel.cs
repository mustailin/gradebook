using System;
using System.ComponentModel.DataAnnotations;

namespace GradeBook.Data.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public RoleEnum RoleEnum
        {
            get
            {
                return Role switch
                {
                    "admin" => RoleEnum.Admin,
                    "lecturer" => RoleEnum.Lecturer,
                    "student" => RoleEnum.Student,
                    _ => throw new ArgumentOutOfRangeException(),
                };
            }
        }
    }

    public enum RoleEnum
    {
        Admin,
        Lecturer,
        Student
    }
}
