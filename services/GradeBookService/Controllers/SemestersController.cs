using GradeBook.Data.GradeBook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GradeBookService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SemestersController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public SemestersController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _dbContext.Semesters.ToListAsync());
        }

        [HttpGet("{id:int}", Name = nameof(GetSemesterAsync))]
        public async Task<IActionResult> GetSemesterAsync(int id)
        {
            return Ok(await _dbContext.Semesters
                .Include(s => s.Gradebooks)
                .FirstOrDefaultAsync(s => s.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Semester semester)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _dbContext.Semesters.Add(semester);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetSemesterAsync), routeValues: new { id = semester.Id }, value: semester);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Semester semesterToUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var semester = await _dbContext.Semesters.FirstOrDefaultAsync(s => s.Id == id);
            if (semester == null)
                return BadRequest();

            _dbContext.Entry(semester).CurrentValues.SetValues(semesterToUpdate);
            await _dbContext.SaveChangesAsync();

            return Ok(semester);
        }

        [HttpDelete("{id:int}")]
        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            _dbContext.Semesters.Remove(await _dbContext.Semesters.FirstOrDefaultAsync(s => s.Id == id));
            await _dbContext.SaveChangesAsync();
        }
    }
}
