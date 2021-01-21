using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s19981;Integrated Security=True";

        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT FirstName, LastName, BirthDate, Enrollment.Semester, Studies.Name as Studies FROM Student, Enrollment, Studies WHERE Student.IdEnrollment = Enrollment.IdEnrollment AND Enrollment.IdStudy = Studies.IdStudy";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while(dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.Semester = dr["Semester"].ToString();
                    st.Studies = dr["Studies"].ToString();
                    list.Add(st);
                }
            };

            return Ok(list);
        }
        
        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            var list = new List<Enrollment>();
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT Enrollment.IdEnrollment, Enrollment.Semester, Enrollment.IdStudy, Enrollment.StartDate, Studies.Name as Studies FROM Enrollment, Student, Studies WHERE Enrollment.IdEnrollment = Student.IdEnrollment AND Studies.IdStudy = Enrollment.IdStudy AND Student.IndexNumber=@id";
                com.Parameters.AddWithValue("id", indexNumber);

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var e = new Enrollment();
                    e.IdEnrollment = dr["IdEnrollment"].ToString();
                    e.Semester = dr["Semester"].ToString();
                    e.IdStudy = dr["IdStudy"].ToString();
                    e.StartDate = dr["StartDate"].ToString();
                    e.Studies = dr["Studies"].ToString();
                    list.Add(e);
                }
            }
            return Ok(list);
        }
    }
}
