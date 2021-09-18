using GradeBook.Common;
using GradeBook.Data.GradeBook;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GradeBookService.Controllers
{
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public UsersController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("check")]
        public IActionResult CheckAsync()
        {
            var result = JwtHelper.GetUserInfo(Request);
            if (result == null)
                return BadRequest(new { Error = $"Incorrect Authorization header." });

            return base.Ok(result);
        }
    }
}
