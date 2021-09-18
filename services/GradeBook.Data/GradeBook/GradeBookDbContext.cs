using Microsoft.EntityFrameworkCore;

#nullable disable

namespace GradeBook.Data.GradeBook
{
    public partial class GradeBookDbContext : DbContext
    {
        public GradeBookDbContext()
        {
        }

        public GradeBookDbContext(DbContextOptions<GradeBookDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<AttendanceView> AttendanceViews { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<Gradebook> Gradebooks { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Lecturer> Lecturers { get; set; }
        public virtual DbSet<LecturerGradebook> LecturerGradebooks { get; set; }
        public virtual DbSet<LecturerHasGradebook> LecturerHasGradebooks { get; set; }
        public virtual DbSet<Semester> Semesters { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentAttendance> StudentAttendances { get; set; }
        public virtual DbSet<StudentGrade> StudentGrades { get; set; }
        public virtual DbSet<StudentGradebook> StudentGradebooks { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TaskView> TaskViews { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("Name=DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admin");

                entity.HasIndex(e => e.Email, "email_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(60)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("last_name");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("role");
            });

            modelBuilder.Entity<AttendanceView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("attendance_view");

                entity.Property(e => e.Absent).HasColumnName("absent");

                entity.Property(e => e.AbsentWithReason).HasColumnName("absent_with_reason");

                entity.Property(e => e.AttendanceId)
                    .HasColumnType("bigint unsigned")
                    .HasColumnName("attendance_id");

                entity.Property(e => e.ClassDate)
                    .HasColumnType("date")
                    .HasColumnName("class_date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("first_name");

                entity.Property(e => e.GradebookId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("gradebook_id");

                entity.Property(e => e.GroupId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("group_id");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("last_name");

                entity.Property(e => e.Present).HasColumnName("present");

                entity.Property(e => e.SemesterId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("semester_id");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.StudentId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("student_id");

                entity.Property(e => e.TaskId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("task_id");

                entity.Property(e => e.TaskLength).HasColumnName("task_length");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department");

                entity.HasIndex(e => e.FacultyId, "fk_department_faculty1_idx");

                entity.HasIndex(e => e.Name, "name_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.FacultyId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("faculty_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.FacultyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_department_faculty1");
            });

            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.ToTable("faculty");

                entity.Property(e => e.Id)
                    .HasColumnType("int unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Gradebook>(entity =>
            {
                entity.ToTable("gradebook");

                entity.HasIndex(e => e.GroupId, "fk_gradebook_group1_idx");

                entity.HasIndex(e => e.SemesterId, "fk_gradebook_semester1_idx");

                entity.Property(e => e.Id)
                    .HasColumnType("int unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.GroupId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("group_id");

                entity.Property(e => e.SemesterId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("semester_id");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("subject");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Gradebooks)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_gradebook_group1");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Gradebooks)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_gradebook_semester1");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("group");

                entity.HasIndex(e => e.FacultyId, "fk_academic_group_faculty1_idx");

                entity.Property(e => e.Id)
                    .HasColumnType("int unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.FacultyId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("faculty_id");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("number");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.FacultyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_academic_group_faculty1");
            });

            modelBuilder.Entity<Lecturer>(entity =>
            {
                entity.ToTable("lecturer");

                entity.HasIndex(e => e.Email, "email_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.DepartmentId, "fk_lecturer_department1_idx");

                entity.Property(e => e.Id)
                    .HasColumnType("int unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.DepartmentId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("department_id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(60)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("last_name");

                entity.Property(e => e.Role)
                    .HasMaxLength(45)
                    .HasColumnName("role");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Lecturers)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_lecturer_department1");
            });

            modelBuilder.Entity<LecturerGradebook>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("lecturer_gradebooks");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("smallint unsigned")
                    .HasColumnName("academic_year");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("first_name");

                entity.Property(e => e.GradebookId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("gradebook_id");

                entity.Property(e => e.GroupId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("group_id");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("last_name");

                entity.Property(e => e.LecturerId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("lecturer_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("name");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("number");

                entity.Property(e => e.SemesterId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("semester_id");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("subject");
            });

            modelBuilder.Entity<LecturerHasGradebook>(entity =>
            {
                entity.HasKey(e => new { e.GradebookId, e.LecturerId })
                    .HasName("PRIMARY");

                entity.ToTable("lecturer_has_gradebook");

                entity.HasIndex(e => e.GradebookId, "fk_lecturer_has_gradebook_gradebook1_idx");

                entity.HasIndex(e => e.LecturerId, "fk_lecturer_has_gradebook_lecturer1_idx");

                entity.Property(e => e.GradebookId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("gradebook_id");

                entity.Property(e => e.LecturerId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("lecturer_id");

                entity.HasOne(d => d.Gradebook)
                    .WithMany(p => p.LecturerHasGradebooks)
                    .HasForeignKey(d => d.GradebookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_course_lecturer_gradebook1");

                entity.HasOne(d => d.Lecturer)
                    .WithMany(p => p.LecturerHasGradebooks)
                    .HasForeignKey(d => d.LecturerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_course_lecturer_lecturer1");
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.ToTable("semester");

                entity.Property(e => e.Id)
                    .HasColumnType("int unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("smallint unsigned")
                    .HasColumnName("academic_year");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("student");

                entity.HasIndex(e => e.Email, "email_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.GroupId, "fk_student_group_idx");

                entity.Property(e => e.Id)
                    .HasColumnType("int unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(60)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("first_name");

                entity.Property(e => e.GroupId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("group_id");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("last_name");

                entity.Property(e => e.Role)
                    .HasMaxLength(45)
                    .HasColumnName("role");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_student_group");
            });

            modelBuilder.Entity<StudentAttendance>(entity =>
            {
                entity.ToTable("student_attendance");

                entity.HasIndex(e => e.StudentId, "fk_student_attendance_student1_idx");

                entity.HasIndex(e => e.TaskId, "fk_student_attendance_task1_idx");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Absent).HasColumnName("absent");

                entity.Property(e => e.AbsentWithReason).HasColumnName("absent_with_reason");

                entity.Property(e => e.ClassDate)
                    .HasColumnType("date")
                    .HasColumnName("class_date");

                entity.Property(e => e.Present).HasColumnName("present");

                entity.Property(e => e.StudentId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("student_id");

                entity.Property(e => e.TaskId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("task_id");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.StudentAttendances)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_student_attendance_student1");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.StudentAttendances)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_student_attendance_task1");
            });

            modelBuilder.Entity<StudentGrade>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.TaskId })
                    .HasName("PRIMARY");

                entity.ToTable("student_grade");

                entity.HasIndex(e => e.StudentId, "fk_student_grade_student1_idx");

                entity.HasIndex(e => e.TaskId, "fk_student_grade_task1_idx");

                entity.Property(e => e.StudentId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("student_id");

                entity.Property(e => e.TaskId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("task_id");

                entity.Property(e => e.Grade).HasColumnName("grade");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.StudentGrades)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_student_grade_student1");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.StudentGrades)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_student_grade_task1");
            });

            modelBuilder.Entity<StudentGradebook>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("student_gradebooks");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("smallint unsigned")
                    .HasColumnName("academic_year");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("first_name");

                entity.Property(e => e.GradebookId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("gradebook_id");

                entity.Property(e => e.GroupId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("group_id");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("last_name");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("name");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("number");

                entity.Property(e => e.SemesterId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("semester_id");

                entity.Property(e => e.StudentId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("student_id");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("subject");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.ToTable("task");

                entity.HasIndex(e => e.GradebookId, "fk_task_gradebook1_idx");

                entity.Property(e => e.Id)
                    .HasColumnType("int unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.GradebookId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("gradebook_id");

                entity.Property(e => e.MaxGrade).HasColumnName("max_grade");

                entity.Property(e => e.OnCourseFri).HasColumnName("on_course_fri");

                entity.Property(e => e.OnCourseMon).HasColumnName("on_course_mon");

                entity.Property(e => e.OnCourseThu).HasColumnName("on_course_thu");

                entity.Property(e => e.OnCourseTue).HasColumnName("on_course_tue");

                entity.Property(e => e.OnCourseWed).HasColumnName("on_course_wed");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.TaskLength).HasColumnName("task_length");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("title");

                entity.HasOne(d => d.Gradebook)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.GradebookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_task_gradebook1");
            });

            modelBuilder.Entity<TaskView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("task_view");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("first_name");

                entity.Property(e => e.Grade).HasColumnName("grade");

                entity.Property(e => e.GradebookId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("gradebook_id");

                entity.Property(e => e.GroupId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("group_id");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("last_name");

                entity.Property(e => e.MaxGrade).HasColumnName("max_grade");

                entity.Property(e => e.SemesterId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("semester_id");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.StudentId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("student_id");

                entity.Property(e => e.TaskId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("task_id");

                entity.Property(e => e.TaskLength).HasColumnName("task_length");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Email)
                    .HasName("PRIMARY");

                entity.ToTable("user");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.IsAccountNonExpired)
                    .HasColumnType("bit(1)")
                    .HasColumnName("is_account_non_expired");

                entity.Property(e => e.IsAccountNonLocked)
                    .HasColumnType("bit(1)")
                    .HasColumnName("is_account_non_locked");

                entity.Property(e => e.IsCredentialsNonExpired)
                    .HasColumnType("bit(1)")
                    .HasColumnName("is_credentials_non_expired");

                entity.Property(e => e.IsEnabled)
                    .HasColumnType("bit(1)")
                    .HasColumnName("is_enabled");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("password");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("user_roles");

                entity.HasIndex(e => e.UserEmail, "FKfinmcawb90mtj05cpco76b963");

                entity.Property(e => e.Roles)
                    .HasMaxLength(255)
                    .HasColumnName("roles");

                entity.Property(e => e.UserEmail)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("user_email");

                entity.HasOne(d => d.UserEmailNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.UserEmail)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKfinmcawb90mtj05cpco76b963");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
