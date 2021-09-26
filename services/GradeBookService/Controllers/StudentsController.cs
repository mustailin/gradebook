using GradeBook.Common;
using GradeBook.Data.GradeBook;
using GradeBook.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GradeBookService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public StudentsController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _dbContext.Students
                .Include(s => s.Group)
                .ToListAsync());
        }

        [HttpGet("{id:int}", Name = nameof(GetStudentAsync))]
        public async Task<IActionResult> GetStudentAsync(int id)
        {
            return Ok(await _dbContext.Students
                .FirstOrDefaultAsync(s => s.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] StudentViewModel studentToCreate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = new Student
            {
                FirstName = studentToCreate.FirstName,
                LastName = studentToCreate.LastName,
                Email = studentToCreate.Email,
                GroupId = studentToCreate.Group.Id,
                Role = "student"
            };

            _dbContext.Students.Add(student);

            var user = new User
            {
                Email = student.Email,
                IsAccountNonExpired = true,
                IsAccountNonLocked = true,
                IsCredentialsNonExpired = true,
                IsEnabled = true,
                Password = Hasher.PBKDF2Hash(studentToCreate.Password)
            };

            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetStudentAsync), routeValues: new { id = student.Id }, value: student);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UserViewModel userToUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
                return BadRequest();

            if (!string.IsNullOrEmpty(userToUpdate.Password))
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == student.Email);
                if (user == null)
                {
                    user = new User
                    {
                        Email = userToUpdate.Email,
                        IsAccountNonExpired = true,
                        IsAccountNonLocked = true,
                        IsCredentialsNonExpired = true,
                        IsEnabled = true,
                        Password = Hasher.PBKDF2Hash(userToUpdate.Password)
                    };

                    _dbContext.Users.Add(user);
                }
                else
                {
                    user.Email = userToUpdate.Email;
                    user.Password = Hasher.PBKDF2Hash(userToUpdate.Password);
                }
            }

            student.Email = userToUpdate.Email;
            student.FirstName = userToUpdate.FirstName;
            student.LastName = userToUpdate.LastName;

            await _dbContext.SaveChangesAsync();

            return Ok(student);
        }

        [HttpPut("{id}/tasks/{taskId}/grade")]
        public async Task<IActionResult> UpdateAsync(int id, int taskId, [FromBody] GradeViewModel gradeViewModel)
        {
            var studentGrade = await _dbContext.StudentGrades.FirstOrDefaultAsync(sg => sg.StudentId == id && sg.TaskId == taskId);

            if (studentGrade == null)
                return BadRequest(new { Error = $"StudentGrade with StudentId {id} and TaskId {taskId} does not exist" });

            studentGrade.Grade = gradeViewModel.Grade;
            _dbContext.Update(studentGrade);
            await _dbContext.SaveChangesAsync();
            return Ok(studentGrade);
        }

        [HttpDelete("{id:int}")]
        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            _dbContext.Students.Remove(await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id));
            await _dbContext.SaveChangesAsync();
        }
    }
}
