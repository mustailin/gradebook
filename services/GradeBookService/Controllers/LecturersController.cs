using GradeBook.Common;
using GradeBook.Data.GradeBook;
using GradeBook.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GradeBookService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LecturersController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public LecturersController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _dbContext.Lecturers
                .Include(l => l.Department)
                .Include(l => l.Department.Faculty)
                .Include(l => l.LecturerHasGradebooks)
                .ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            return Ok(await _dbContext.Lecturers
                .Include(l => l.Department)
                .Include(l => l.Department.Faculty)
                .Include(l => l.LecturerHasGradebooks)
                .FirstOrDefaultAsync(l => l.Id == id));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UserViewModel userToUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lecturer = await _dbContext.Lecturers.FirstOrDefaultAsync(s => s.Id == id);
            if (lecturer == null)
                return BadRequest();

            if (!string.IsNullOrEmpty(userToUpdate.Password))
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == lecturer.Email);
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

            lecturer.Email = userToUpdate.Email;
            lecturer.FirstName = userToUpdate.FirstName;
            lecturer.LastName = userToUpdate.LastName;

            await _dbContext.SaveChangesAsync();

            return Ok(lecturer);
        }

        [HttpDelete("{id:int}")]
        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            _dbContext.Lecturers.Remove(await _dbContext.Lecturers.FirstOrDefaultAsync(s => s.Id == id));
            await _dbContext.SaveChangesAsync();
        }
    }
}
