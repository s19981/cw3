using cw3.DTOs.Requests;
using cw3.Models;
using cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace cw3.Controllers
{

    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private IStudentDbService _service;
        public IConfiguration Configuration { get; set; }
        public StudentsController(IConfiguration configuration, IStudentDbService service)
        {
            Configuration = configuration;
            _service = service;
        }
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s19981;Integrated Security=True";

        [HttpGet]
        [Authorize(Roles = "employee")]
        public IActionResult GetStudents()
        {
            var list = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT IndexNumber, FirstName, LastName, BirthDate, Enrollment.Semester, Studies.Name as Studies FROM Student, Enrollment, Studies WHERE Student.IdEnrollment = Enrollment.IdEnrollment AND Enrollment.IdStudy = Studies.IdStudy";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.Semester = (int)dr["Semester"];
                    st.Studies = dr["Studies"].ToString();
                    list.Add(st);
                }
            };

            return Ok(list);
        }

        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            var result = _service.CheckCredentials(request);

            if (!result.Authenticated)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, result.IndexNumber),
                    new Claim(ClaimTypes.Name, result.FirstName),
                    new Claim(ClaimTypes.Role, "employee")
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = result.RefreshToken
            });
        }

        [HttpPost("refresh-token/{refToken}")]
        public IActionResult RefreshToken(string refToken)
        {
            var result = _service.CheckRefreshToken(refToken);

            if (!result.Authenticated)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, result.IndexNumber),
                    new Claim(ClaimTypes.Name, result.FirstName),
                    new Claim(ClaimTypes.Role, "employee")
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = result.RefreshToken
            });
        }

        /*
        private static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes); 
                return Convert.ToBase64String(randomBytes);
            }
        }
        */
    }
}
