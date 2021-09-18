using GradeBook.Data.GradeBook;
using GradeBook.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GradeBookService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public DepartmentsController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _dbContext.Departments.
                Include(d => d.Faculty)
                .ToListAsync());
        }

        [HttpGet("{id:int}", Name = nameof(GetDepartmentAsync))]
        public async Task<IActionResult> GetDepartmentAsync(int id)
        {
            return Ok(await _dbContext.Departments.FirstOrDefaultAsync(f => f.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateDepartmentViewModel departmentToCreate)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            Department department = new()
            {
                FacultyId = departmentToCreate.Faculty.Id,
                Name = departmentToCreate.Name
            };

            _dbContext.Departments.Add(department);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetDepartmentAsync), routeValues: new { id = department.Id }, value: department);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Department departmentToUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var department = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department == null)
                return BadRequest();

            _dbContext.Entry(department).CurrentValues.SetValues(departmentToUpdate);
            await _dbContext.SaveChangesAsync();

            return Ok(department);
        }

        [HttpDelete("{id:int}")]
        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            _dbContext.Departments.Remove(await _dbContext.Departments.FirstOrDefaultAsync(f => f.Id == id));
            await _dbContext.SaveChangesAsync();
        }
    }
}
