using ApplicationService.DTOs;
using ApplicationService.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace SchoolInfoAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/Teachers")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class TeachersController : ControllerBase
    {
        private readonly TeacherManagementService _service = null!;

        public TeachersController()
        {
            _service = new TeacherManagementService();
        }

        /// <summary>
        /// Returns a list of teachers from a page. The number of teachers per page and the number
        /// of the desired page must be specified. Optionally, the teachers can be filtered by first name and last name.
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
        public ActionResult<IEnumerable<TeacherDTO>> GetTeachers(int pageNumber = 1, int itemsPerPage = 10,
            string? firstName = null, string? lastName = null)
        {
            return _service.Get(pageNumber, itemsPerPage, firstName, lastName);
        }

        /// <summary>
        /// Returns all teachers without their own class. Those teachers are not
        /// considered form teachers.
        /// </summary>
        /// <returns></returns>
        [HttpGet("WithoutClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<TeacherDTO>> GetTeachersWithoutTheirOwnClass()
        {
            return _service.Get(null, null, null, null, true);
        }

        /// <summary>
        /// Returns the count of pages if all pages have the same number of teachers as specified.
        /// Optionally, teachers can be filtered by first name and last name before counting the pages.
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
        public ActionResult<int> GetTeacherPageCount(int itemsPerPage, string? firstName = null, string? lastName = null)
        {
            return _service.GetPageCount(itemsPerPage, firstName, lastName);
        }

        /// <summary>
        /// Returns the teacher with specified ID.
        /// </summary>
        /// <param name="id">ID of the teacher</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TeacherDTO> GetTeacherById(int id)
        {
            TeacherDTO? foundTeacher = _service.GetById(id);

            if(foundTeacher == null)
            {
                return NotFound($"Teacher with id [{id}] was not found!");
            }
            else
            {
                return foundTeacher;
            }
        }

        /// <summary>
        /// Saves a teacher to the database. Saving can be either updating an already existing
        /// entity or creating a completely new one. When ID of 0 is provided for the entity, it will
        /// be created, otherwise an update attempt will be made.
        /// </summary>
        /// <param name="teacherDTO">DTO with necessary teacher data</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TeacherDTO> SaveTeacher(TeacherDTO teacherDTO)
        {
            //Error code 400 is returned when validation with Validator fails
            if (!teacherDTO.Validate())
            {
                return BadRequest("Invalid data!");
            }

            if(_service.Save(teacherDTO))
            {
                return StatusCode(200, "Teacher is saved successfully!");
            }
            else
            {
                return StatusCode(500, "Failure to save teacher!");
            }
        }

        /// <summary>
        /// Deletes a teacher from the database with specified ID.
        /// </summary>
        /// <param name="id">The ID of the teacher</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTeacher(int id)
        {
            if (_service.GetById(id) == null)
            {
                return NotFound($"Teacher with id [{id}] was not found!");
            }

            if (_service.Delete(id))
            {
                return StatusCode(200, "Teacher is deleted successfully!");
            }
            else
            {
                return StatusCode(500, "Failure to delete teacher!");
            }
        }
    }
}
