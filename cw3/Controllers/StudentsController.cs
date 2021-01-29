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
        public IConfiguration Configuration { get; set; }
        public StudentsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var db = new s19981Context();
            var list = db.Students.ToList();
            return Ok(list);
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            var db = new s19981Context();
            var student = db.Students.Where(student => student.IndexNumber == indexNumber).First();
            return Ok(student);
        }

        [HttpPatch("{indexNumber}")]
        public IActionResult UpdateStudent(string indexNumber, string firstName, string lastName)
        {
            var db = new s19981Context();
            var student = new Student
            {
                IndexNumber = indexNumber,
                FirstName = firstName,
                LastName = lastName
            };
            db.Attach(student);
            db.Entry(student).Property("FirstName").IsModified = true;
            db.Entry(student).Property("LastName").IsModified = true;
            db.SaveChanges();

            return Ok(student);
        }

        [HttpPost]
        public IActionResult CreateStudent(string indexNumber, string firstName, string lastName, DateTime birthDate)
        {
            var db = new s19981Context();
            var student = new Student
            {
                IndexNumber = indexNumber,
                FirstName = firstName,
                LastName = lastName,
                BirthDate = DateTime.Parse(birthDate.ToString()),
                IdEnrollment = 1
            };

            db.Students.Add(student);
            db.SaveChanges();

            return Ok(student);
        }

        [HttpDelete("{indexNumber}")]
        public IActionResult DeleteStudent(string indexNumber)
        {
            var db = new s19981Context();
            var student = new Student
            {
                IndexNumber = indexNumber
            };

            db.Attach(student);
            db.Remove(student);
            
            db.SaveChanges();

            return Ok();
        }
    }
}
