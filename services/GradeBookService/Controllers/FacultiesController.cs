using GradeBook.Data.GradeBook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GradeBookService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FacultiesController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public FacultiesController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _dbContext.Faculties.ToListAsync());
        }

        [HttpGet("{id:int}", Name = nameof(GetFacultyAsync))]
        public async Task<IActionResult> GetFacultyAsync(int id)
        {
            return Ok(await _dbContext.Faculties.FirstOrDefaultAsync(f => f.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Faculty faculty)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _dbContext.Faculties.Add(faculty);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetFacultyAsync), routeValues: new { id = faculty.Id }, value: faculty);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Faculty facultyToUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var faculty = await _dbContext.Faculties.FirstOrDefaultAsync(f => f.Id == id);
            if (faculty == null)
                return BadRequest();

            _dbContext.Entry(faculty).CurrentValues.SetValues(facultyToUpdate);
            await _dbContext.SaveChangesAsync();

            return Ok(faculty);
        }

        [HttpDelete("{id:int}")]
        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            _dbContext.Faculties.Remove(await _dbContext.Faculties.FirstOrDefaultAsync(f => f.Id == id));
            await _dbContext.SaveChangesAsync();
        }
    }
}
