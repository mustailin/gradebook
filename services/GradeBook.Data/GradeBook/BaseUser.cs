namespace GradeBook.Data.GradeBook
{
    public abstract class BaseUser
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Role { get; set; }
    }
}
