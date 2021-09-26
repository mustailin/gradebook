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
    public class AdministratorsController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public AdministratorsController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _dbContext.Admins.ToListAsync());
        }

        [HttpGet("{id:int}", Name = nameof(GetAdminAsync))]
        public async Task<IActionResult> GetAdminAsync(int id)
        {
            return Ok(await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] AdminViewModel adminToCreate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var admin = new Admin
            {
                FirstName = adminToCreate.FirstName,
                LastName = adminToCreate.LastName,
                Email = adminToCreate.Email,
                Role = "admin"
            };

            _dbContext.Admins.Add(admin);

            var user = new User
            {
                Email = admin.Email,
                IsAccountNonExpired = true,
                IsAccountNonLocked = true,
                IsCredentialsNonExpired = true,
                IsEnabled = true,
                Password = Hasher.PBKDF2Hash(adminToCreate.Password)
            };

            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetAdminAsync), routeValues: new { id = admin.Id }, value: admin);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UserViewModel userToUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var admin = await _dbContext.Admins.FirstOrDefaultAsync(s => s.Id == id);
            if (admin == null)
                return BadRequest();

            if (!string.IsNullOrEmpty(userToUpdate.Password))
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == admin.Email);
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

            admin.Email = userToUpdate.Email;
            admin.FirstName = userToUpdate.FirstName;
            admin.LastName = userToUpdate.LastName;

            await _dbContext.SaveChangesAsync();

            return Ok(admin);
        }

        [HttpDelete("{id:int}")]
        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            _dbContext.Admins.Remove(await _dbContext.Admins.FirstOrDefaultAsync(s => s.Id == id));
            await _dbContext.SaveChangesAsync();
        }
    }
}
