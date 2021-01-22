using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace cw3.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s19981;Integrated Security=True";

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            EnrollStudentResponse response = null;
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            using (SqlCommand com2 = new SqlCommand())
            {
                com.Connection = con;
                com2.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                com.Transaction = tran;
                com2.Transaction = tran;

                try
                {
                    com.CommandText = "SELECT IdStudy FROM Studies WHERE Name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    var dr = com.ExecuteReader();

                    if (!dr.Read())
                    {
                        tran.Rollback();
                        throw new EnrollmentException("Studia nie instnieja");
                    }
                    int idstudies = (int)dr["IdStudy"];
                    dr.Close();

                    /*
                    com.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber=@indexnumber";
                    com.Parameters.AddWithValue("indexnumber", request.IndexNumber);

                    dr = com.ExecuteReader();

                    if (dr.HasRows)
                    {
                        //tran.Rollback();
                        throw new EnrollmentException("Numer indexu jest zajety");
                    }
                    dr.Close();
                    */


                    com.CommandText = "SELECT Enrollment.IdEnrollment FROM Enrollment, Student WHERE Semester=1 AND Student.IdEnrollment=Enrollment.IdEnrollment AND Student.IndexNumber=@indexnumber AND Enrollment.IdStudy=@idstudies ORDER BY StartDate DESC";
                    com.Parameters.AddWithValue("indexnumber", request.IndexNumber);
                    com.Parameters.AddWithValue("idstudies", idstudies);

                    dr = com.ExecuteReader();
                    if (dr.HasRows)
                    {
                        tran.Rollback();
                        throw new EnrollmentException("Student jest juz zapisany");
                    }
                    dr.Close();

                    com.CommandText = "SELECT MAX(IdEnrollment) AS MaxEnrollmentId FROM Enrollment";
                    dr = com.ExecuteReader();
                    int idenrollment = 1;

                    if (dr.Read())
                    {
                        idenrollment = (int)dr["MaxEnrollmentId"] + 1;
                    }
                    dr.Close();

                    DateTime startdate = DateTime.Now;
                    com.CommandText = "INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES (@IdEnrollment, @Semester, @IdStudy, @StartDate)";
                    com.Parameters.AddWithValue("IdEnrollment", idenrollment);
                    com.Parameters.AddWithValue("Semester", 1);
                    com.Parameters.AddWithValue("IdStudy", idstudies);
                    com.Parameters.AddWithValue("StartDate", startdate);
                    com.ExecuteNonQuery();

                    com2.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@IndexNumber, @FirstName, @LastName, @Birthdate, @IdEnrollment)";
                    com2.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                    com2.Parameters.AddWithValue("FirstName", request.FirstName);
                    com2.Parameters.AddWithValue("LastName", request.LastName);
                    com2.Parameters.AddWithValue("BirthDate", request.BirthDate);
                    com2.Parameters.AddWithValue("IdEnrollment", idenrollment);
                    com2.ExecuteNonQuery();

                    response = new EnrollStudentResponse
                    {
                        IdEnrollment = idenrollment,
                        Name = request.FirstName,
                        Semester = 1,
                        StartDate = startdate

                    };

                    tran.Commit();
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    throw new EnrollmentException(exc.Message);
                }
                return response;
                }
            }

        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request)
        {
            PromoteStudentsResponse response = null;
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = "PromoteStudents";
                com.Parameters.AddWithValue("@Studies", request.Studies);
                com.Parameters.AddWithValue("@Semester", request.Semester);
                com.Parameters.Add("@IdEnrollment", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;
                com.Parameters.Add("@IdStudies", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;
                com.Parameters.Add("@StartDate", System.Data.SqlDbType.Date).Direction = System.Data.ParameterDirection.Output;

                con.Open();
                com.ExecuteNonQuery();

                response = new PromoteStudentsResponse();
                response.IdEnrollment = (int)com.Parameters["@IdEnrollment"].Value;
                response.Semester = request.Semester + 1;
                response.IdStudy = (int)com.Parameters["@IdStudies"].Value;
                response.StartDate = (DateTime)com.Parameters["@StartDate"].Value;
            }

            return response;
        }
    }

    public class EnrollmentException : Exception
    {
        public EnrollmentException()
        {
        }

        public EnrollmentException(string message) : base(message)
        {
        }
    }
}
