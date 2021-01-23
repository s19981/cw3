using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Services
{
    public interface IStudentDbService
    {
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
        PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request);
        Boolean CheckIndexNumber(string index);
        AuthResponse CheckCredentials(LoginRequest request);
        AuthResponse CheckRefreshToken(string refToken);

    }
}
