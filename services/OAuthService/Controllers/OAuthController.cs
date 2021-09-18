﻿using GradeBook.Common;
using GradeBook.Data.GradeBook;
using GradeBook.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthService.Controllers
{
    [Route("oauth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly GradeBookDbContext _dbContext;

        public OAuthController(GradeBookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("authorize")]
        public async Task<IActionResult> Authorize()
        {
            if (!Request.Headers.TryGetValue("Authorization", out StringValues value))
                return BadRequest(new { Error = $"Incorrect Authorization header: {value}" });

            var decodedValues = Encoding.UTF8.GetString(Convert.FromBase64String(value.ToString().Split(' ').Last())).Split(":");
            if (decodedValues.Length != 2)
                return BadRequest(new { Error = $"Incorrect Authorication header value. There is no Username and/or Password" });

            var username = decodedValues.First();
            var password = Hasher.PBKDF2Hash(decodedValues.Last());

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == username
                                                                       && u.Password == password
                                                                       && u.IsEnabled
                                                                       && u.IsAccountNonExpired
                                                                       && u.IsAccountNonLocked
                                                                       && u.IsCredentialsNonExpired);
            if (user == null)
                return Unauthorized();

            var userInfo = new UserViewModel { Email = user.Email };
            var admin = await _dbContext.Admins.FirstOrDefaultAsync(u => u.Email == username);
            if (admin == null)
            {
                var lecturer = await _dbContext.Lecturers.FirstOrDefaultAsync(u => u.Email == username);
                if (lecturer == null)
                {
                    var student = await _dbContext.Students.FirstOrDefaultAsync(u => u.Email == username);
                    if (student == null)
                    {
                        return Unauthorized();
                    }
                    else
                    {
                        userInfo.FirstName = student.FirstName;
                        userInfo.LastName = student.LastName;
                        userInfo.Role = student.Role;
                        userInfo.Id = student.Id;
                    }
                }
                else
                {
                    userInfo.FirstName = lecturer.FirstName;
                    userInfo.LastName = lecturer.LastName;
                    userInfo.Role = lecturer.Role;
                    userInfo.Id = lecturer.Id;
                }
            }
            else
            {
                userInfo.FirstName = admin.FirstName;
                userInfo.LastName = admin.LastName;
                userInfo.Role = admin.Role;
                userInfo.Id = admin.Id;
            }


            string token = JwtHelper.GenerateJSONWebToken(userInfo);

            Response.Headers.Add("Token", token);
            return Ok(user);
        }
    }
}
