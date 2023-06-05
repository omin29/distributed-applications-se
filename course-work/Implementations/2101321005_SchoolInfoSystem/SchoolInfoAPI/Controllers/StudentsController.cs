using ApplicationService.DTOs;
using ApplicationService.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace SchoolInfoAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/Students")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class StudentsController : ControllerBase
    {
        private readonly StudentManagementService _service;

        public StudentsController()
        {
            _service = new StudentManagementService();
        }

        /// <summary>
        /// Returns a list of students from a page. The number of students per page and the number
        /// of the desired page must be specified. Optionally, the students can be filtered by first name and last name.
        /// </summary>
        /// <param name="pageNumber">The number of the requested page</param>
        /// <param name="itemsPerPage">The number of items for every page</param>
        /// <param name="firstName">Searched first name</param>
        /// <param name="lastName">Searched last name</param>
        /// <returns></returns>
        [HttpGet("{pageNumber}/{itemsPerPage}/")]
        [HttpGet("{pageNumber}/{itemsPerPage}/{firstName}")]
        [HttpGet("{pageNumber}/{itemsPerPage}/_{lastName}")]
        [HttpGet("{pageNumber}/{itemsPerPage}/{firstName}_{lastName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<StudentDTO>> GetStudents(int pageNumber = 1, int itemsPerPage = 10,
            string? firstName = null, string? lastName = null)
        {
            return _service.Get(pageNumber, itemsPerPage, firstName, lastName);
        }

        /// <summary>
        /// Returns the count of pages if all pages have the same number of students as specified.
        /// Optionally, students can be filtered by first name and last name before counting the pages.
        /// </summary>
        /// <param name="itemsPerPage">The number of items for every page</param>
        /// <param name="firstName">Searched first name</param>
        /// <param name="lastName">Searched last name</param>
        /// <returns></returns>
        [HttpGet("PageCount/{itemsPerPage}/")]
        [HttpGet("PageCount/{itemsPerPage}/{firstName}")]
        [HttpGet("PageCount/{itemsPerPage}/_{lastName}")]
        [HttpGet("PageCount/{itemsPerPage}/{firstName}_{lastName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<int> GetStudentPageCount(int itemsPerPage, string? firstName = null, string? lastName = null)
        {
            return _service.GetPageCount(itemsPerPage, firstName, lastName);
        }

        /// <summary>
        /// Returns the student with specified ID.
        /// </summary>
        /// <param name="id">ID of the student</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> GetStudentById(int id)
        {
            StudentDTO? foundStudent = _service.GetById(id);

            if (foundStudent == null)
            {
                return NotFound($"Student with id [{id}] was not found!");
            }
            else
            {
                return foundStudent;
            }
        }

        /// <summary>
        /// Saves a student to the database. Saving can be either updating an already existing
        /// entity or creating a completely new one. When ID of 0 is provided for the entity, it will
        /// be created, otherwise an update attempt will be made.
        /// </summary>
        /// <param name="saveStudentDTO">DTO with necessary student data</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> SaveStudent(SaveStudentDTO saveStudentDTO)
        {
            StudentDTO studentDTO = new StudentDTO(saveStudentDTO);

            //Error code 400 is returned when validation with Validator fails
            if (!studentDTO.Validate())
            {
                return BadRequest("Invalid data!");
            }

            if (_service.Save(studentDTO))
            {
                return StatusCode(200, "Student is saved successfully!");
            }
            else
            {
                return StatusCode(500, "Failure to save student!");
            }
        }

        /// <summary>
        /// Deletes a student from the database with specified ID.
        /// </summary>
        /// <param name="id">The ID of the student</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteStudent(int id)
        {
            if (_service.GetById(id) == null)
            {
                return NotFound($"Student with id [{id}] was not found!");
            }

            if (_service.Delete(id))
            {
                return StatusCode(200, "Student is deleted successfully!");
            }
            else
            {
                return StatusCode(500, "Failure to delete student!");
            }
        }
    }
}
