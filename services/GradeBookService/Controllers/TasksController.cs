using GradeBook.Data.GradeBook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GradeBookService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public TasksController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _dbContext.Tasks
                .Include(t => t.Gradebook.Semester)
                .Include(t => t.Gradebook.Group)
                .ToListAsync());
        }

        [HttpGet("{id:int}", Name = nameof(GetTaskAsync))]
        public async Task<IActionResult> GetTaskAsync(int id)
        {
            return Ok(await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(GradeBook.Data.GradeBook.Task task)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetTaskAsync), routeValues: new { id = task.Id }, value: task);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] GradeBook.Data.GradeBook.Task taskToUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _dbContext.Tasks.FirstOrDefaultAsync(f => f.Id == id);
            if (task == null)
                return BadRequest();

            _dbContext.Entry(task).CurrentValues.SetValues(taskToUpdate);
            await _dbContext.SaveChangesAsync();

            return Ok(task);
        }

        [HttpDelete("{id:int}")]
        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            _dbContext.Tasks.Remove(await _dbContext.Tasks.FirstOrDefaultAsync(s => s.Id == id));
            await _dbContext.SaveChangesAsync();
        }
    }
}
