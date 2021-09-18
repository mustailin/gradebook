using GradeBook.Data.GradeBook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GradeBookService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public AttendancesController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendancesAsync()
        {
            return Ok(await _dbContext.StudentAttendances.ToListAsync());
        }

        [HttpGet("{id:int}", Name = nameof(GetAttendancesAsync))]
        public async Task<IActionResult> GetAttendancesAsync(int id)
        {
            return Ok(await _dbContext.StudentAttendances.FirstOrDefaultAsync(sa => sa.Id == id));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] StudentAttendance updateStudentAttendance)
        {
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var studentAttendance = await _dbContext.StudentAttendances.FirstOrDefaultAsync(sa => sa.Id == id);
            if (studentAttendance == null)
                return BadRequest(new { Error = $"StudentAttendance with id {id} does not exist" });

            updateStudentAttendance.Id = studentAttendance.Id;
            _dbContext.Entry(studentAttendance).CurrentValues.SetValues(updateStudentAttendance);
            await _dbContext.SaveChangesAsync();

            return Ok(studentAttendance);
        }
    }
}
