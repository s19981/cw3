using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Models;
using cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;

        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            try
            {
                var enrollmentResult = _service.EnrollStudent(request);
                return Created("api/enrollments", enrollmentResult);
            } catch (EnrollmentException exc)
            {
                return BadRequest(exc.Message);
            }
        }

        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudents(PromoteStudentsRequest request)
        {
            try
            {
                var promotionResult = _service.PromoteStudents(request);
                return Created("api/enrollments/promotions", promotionResult);
            }
            catch (EnrollmentException exc)
            {
                return BadRequest(exc.Message);
            }
        }

    }
}
