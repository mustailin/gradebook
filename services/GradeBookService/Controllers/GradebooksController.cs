using GradeBook.Common;
using GradeBook.Data.GradeBook;
using GradeBook.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradeBookService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GradebooksController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public GradebooksController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetGradeBooksAsync()
        {
            var result = JwtHelper.GetUserInfo(Request);
            if (result == null)
                return BadRequest(new { Error = $"Incorrect Authorization header." });

            switch (result.RoleEnum)
            {
                case RoleEnum.Admin:
                    return await GetGradebooksForAdminAsync();
                case RoleEnum.Lecturer:
                    return await GetGradebooksForLecturerAsync();
                case RoleEnum.Student:
                    return await GetGradebooksForStudentAsync();
                default:
                    return BadRequest();
            }
        }

        [HttpGet("{id:int}", Name = nameof(GetGradeBooksAsync))]
        public async Task<IActionResult> GetGradeBooksAsync(int id)
        {
            var result = JwtHelper.GetUserInfo(Request);
            if (result == null)
                return BadRequest(new { Error = $"Incorrect Authorization header." });

            switch (result.RoleEnum)
            {
                case RoleEnum.Admin:
                    return Ok(ConvertToGradebooksViewModel(id, await _dbContext.Gradebooks
                            .Include(g => g.Group)
                            .Include(g => g.Semester)
                            .Include(g => g.Tasks)
                            .Include(g => g.Group.Faculty)
                            .Include(g => g.LecturerHasGradebooks)
                                .ThenInclude(g => g.Lecturer)
                            .Where(g => g.Id == id)
                            .ToListAsync()));
                case RoleEnum.Lecturer:
                    var lecturerViewModel = ConvertToLecturerGradebookViewModel(id, await _dbContext.LecturerGradebooks
                                    .Include(g => g.Gradebook)
                                    .Include(g => g.Group)
                                    .Include(g => g.Semester)
                                    .Include(g => g.Lecturer)
                                    .Include(g => g.Gradebook.LecturerHasGradebooks)
                                        .ThenInclude(lhg => lhg.Lecturer)
                                    .Where(g => g.GradebookId == id)
                                    .ToListAsync());

                    //lecturerViewModel.LecturerCollection = await _dbContext.LecturerHasGradebooks
                    //    .Include(lhg => lhg.Lecturer)
                    //    .Where(lhg => lhg.GradebookId == id)
                    //    .Select(lhg => lhg.Lecturer)
                    //    .ToListAsync();

                    return Ok(lecturerViewModel);
                case RoleEnum.Student:
                    var studentViewModel = ConvertToStudentGradebooksViewModel(id, await _dbContext.StudentGradebooks
                                    .Include(g => g.Gradebook)
                                    .Include(g => g.Group)
                                    .Include(g => g.Semester)
                                    .Include(g => g.Student)
                                    .Include(g => g.Gradebook.LecturerHasGradebooks)
                                        .ThenInclude(lhg => lhg.Lecturer)
                                    .Where(g => g.GradebookId == id)
                                    .ToListAsync());

                    //studentViewModel.LecturerCollection = await _dbContext
                    //    .LecturerHasGradebooks.Include(lhg => lhg.Lecturer)
                    //    .Where(lhg => lhg.GradebookId == id)
                    //    .Select(lhg => lhg.Lecturer)
                    //    .ToListAsync();

                    return Ok(studentViewModel);
                default:
                    return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateGradebookViewModel gradeBookViewModel)
        {
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var gradebook = new Gradebook
            {
                GroupId = gradeBookViewModel.GroupId,
                SemesterId = gradeBookViewModel.SemesterId,
                Subject = gradeBookViewModel.Subject,
                Description = gradeBookViewModel.Description,
                LecturerHasGradebooks = gradeBookViewModel.LecturerCollection.Select(l => new LecturerHasGradebook { LecturerId = l }).ToList()
            };

            _dbContext.Gradebooks.Add(gradebook);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetGradeBooksAsync), routeValues: new { id = gradebook.Id }, value: gradebook);
        }

        [HttpDelete("{id:int}")]
        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            _dbContext.Gradebooks.Remove(await _dbContext.Gradebooks.FirstOrDefaultAsync(s => s.Id == id));
            await _dbContext.SaveChangesAsync();
        }

        private async Task<IActionResult> GetGradebooksForStudentAsync()
        {
            List<StudentGradebook> gradebooks = await _dbContext.StudentGradebooks
                            .Include(g => g.Group)
                            .Include(g => g.Semester)
                            .Include(g => g.Gradebook)
                            .Include(g => g.Student)
                            .ToListAsync();

            return Ok(gradebooks.GroupBy(v => v.GradebookId, ConvertToStudentGradebooksViewModel));
        }

        private async Task<IActionResult> GetGradebooksForLecturerAsync()
        {
            List<LecturerGradebook> lecturerGradebooks = await _dbContext.LecturerGradebooks
                            .Include(g => g.Group)
                            .Include(g => g.Semester)
                            .Include(g => g.Gradebook)
                            .Include(g => g.Lecturer)
                            .ToListAsync();

            return Ok(lecturerGradebooks.GroupBy(v => v.GradebookId, ConvertToLecturerGradebookViewModel));
        }

        private async Task<IActionResult> GetGradebooksForAdminAsync()
        {
            List<Gradebook> gradebooks = await _dbContext.Gradebooks
                                            .Include(lg => lg.Group)
                                            .Include(lg => lg.Semester)
                                            .Include(lg => lg.Group.Faculty)
                                            .Include(lg => lg.LecturerHasGradebooks)
                                                .ThenInclude(g => g.Lecturer)
                                            .ToListAsync();

            return Ok(gradebooks.GroupBy(v => v.Id, ConvertToGradebooksViewModel));
        }

        private static StudentGradebookViewModel ConvertToStudentGradebooksViewModel(int gradebookId, IEnumerable<StudentGradebook> studentGradebooks)
        {
            Gradebook gradebook = studentGradebooks.FirstOrDefault(g => g.GradebookId == gradebookId).Gradebook;
            Group group = studentGradebooks.FirstOrDefault(g => g.GradebookId == gradebookId)?.Group;
            Semester semester = studentGradebooks.FirstOrDefault(g => g.GradebookId == gradebookId)?.Semester;
            Student student = studentGradebooks.FirstOrDefault(g => g.GradebookId == gradebookId)?.Student;

            StudentGradebookViewModel studentGradebookViewModel = new()
            {
                GradebookId = gradebookId,
                Subject = gradebook.Subject,
                Gradebook = gradebook,
                GroupId = group.Id,
                Group = group,
                SemesterId = semester.Id,
                Semester = semester,
                StudentId = student.Id,
                Student = student,
                Name = semester.Name,
                Number = group.Number,
                FirstName = student.FirstName,
                LastName = student.LastName,
                AcademicYear = semester.AcademicYear,
                Description = gradebook.Description,
                LecturerCollection = gradebook.LecturerHasGradebooks.Select(lhg => lhg.Lecturer).ToList()
            };
            return studentGradebookViewModel;
        }

        private static LecturerGradebookViewModel ConvertToLecturerGradebookViewModel(int gradebookId, IEnumerable<LecturerGradebook> lecturerGradebooks)
        {
            Gradebook gradebook = lecturerGradebooks.FirstOrDefault(g => g.GradebookId == gradebookId).Gradebook;
            Group group = lecturerGradebooks.FirstOrDefault(g => g.GradebookId == gradebookId)?.Group;
            Semester semester = lecturerGradebooks.FirstOrDefault(g => g.GradebookId == gradebookId)?.Semester;
            Lecturer lecturer = lecturerGradebooks.FirstOrDefault(g => g.GradebookId == gradebookId)?.Lecturer;

            LecturerGradebookViewModel studentGradebookViewModel = new()
            {
                GradebookId = gradebookId,
                Subject = gradebook.Subject,
                Gradebook = gradebook,
                GroupId = group.Id,
                Group = group,
                SemesterId = semester.Id,
                Semester = semester,
                LecturerId = lecturer.Id,
                Lecturer = lecturer,
                Name = semester.Name,
                Number = group.Number,
                FirstName = lecturer.FirstName,
                LastName = lecturer.LastName,
                AcademicYear = semester.AcademicYear,
                Description = gradebook.Description,
                LecturerCollection = gradebook.LecturerHasGradebooks.Select(lhg => lhg.Lecturer).ToList()
            };
            return studentGradebookViewModel;
        }

        private static GradebookViewModel ConvertToGradebooksViewModel(int gradebookId, IEnumerable<Gradebook> gradebooks)
        {
            Gradebook gradebook = gradebooks.FirstOrDefault(g => g.Id == gradebookId);
            Group group = gradebooks.FirstOrDefault(g => g.Id == gradebookId)?.Group;
            Semester semester = gradebooks.FirstOrDefault(g => g.Id == gradebookId)?.Semester;
            Faculty faculty = gradebooks.FirstOrDefault(g => g.Id == gradebookId)?.Group?.Faculty;

            GradebookViewModel gradebookViewModel = new()
            {
                Id = gradebookId,
                Subject = gradebook.Subject,
                Description = gradebook.Description,
                Gradebook = gradebook,
                GroupId = group.Id,
                Group = group,
                SemesterId = semester.Id,
                Semester = semester,
                FacultyId = faculty.Id,
                Faculty = faculty,
                LecturerCollection = gradebook.LecturerHasGradebooks.Select(lhg => lhg.Lecturer).ToList()
            };
            return gradebookViewModel;
        }
    }
}
