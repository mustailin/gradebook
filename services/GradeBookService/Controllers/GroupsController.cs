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
    public class GroupsController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public GroupsController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _dbContext.Groups
                .Include(g => g.Faculty)
                .ToListAsync());
        }

        [HttpGet("{id:int}", Name = nameof(GetGroupAsync))]
        public async Task<IActionResult> GetGroupAsync(int id)
        {
            return Ok(await _dbContext.Groups
                .Include(g => g.Gradebooks)
                .Include(g => g.Faculty)
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.Id == id));
        }

        [HttpGet("{id:int}/semesters/{semesterId:int}/gradebooks/{gradebookId:int}/tasks")]
        public async Task<IActionResult> GetTasksAsync(int id, int semesterId, int gradebookId)
        {
            return Ok(await _dbContext.TaskViews
                .Include(t => t.Group)
                .Include(t => t.Semester)
                .Include(t => t.Gradebook)
                .Include(t => t.Task)
                .Include(t => t.Student)
                .Where(t => t.GroupId == id
                    && t.SemesterId == semesterId
                    && t.GradebookId == gradebookId)
                .ToListAsync());
        }

        [HttpGet("{id:int}/semesters/{semesterId:int}/gradebooks/{gradebookId:int}/attendance")]
        public async Task<IActionResult> GetAttendanceAsync(int id, int semesterId, int gradebookId)
        {
            return Ok (await _dbContext.AttendanceViews
                .Include(aw => aw.Student)
                .Include(aw => aw.Gradebook)
                .Include(aw => aw.Semester)
                .Where(aw => aw.GroupId == id
                    && aw.SemesterId == semesterId
                    && aw.GradebookId == gradebookId)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateGroupViewModel groupToCreate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Group group = new()
            {
                Number = groupToCreate.Number,
                FacultyId = groupToCreate.Faculty.Id
            };
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetGroupAsync), routeValues: new { id = group.Id }, value: group);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Group groupToUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id);
            if (group == null)
                return BadRequest();

            _dbContext.Entry(group).CurrentValues.SetValues(groupToUpdate);
            await _dbContext.SaveChangesAsync();

            return Ok(group);
        }

        [HttpDelete("{id:int}")]
        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            _dbContext.Groups.Remove(await _dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id));
            await _dbContext.SaveChangesAsync();
        }
    }
}
