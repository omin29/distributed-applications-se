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
    [Route("api/TeachersClasses")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class TeachersClassesController : ControllerBase
    {
        private readonly TeacherClassManagementService _service;

        public TeachersClassesController()
        {
            _service = new TeacherClassManagementService();
        }

        /// <summary>
        /// Returns all teacher and class combinations. They represent information in which classes teachers teach.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<TeacherClassDTO>> GetTeachersAndClasses()
        {
            return _service.Get();
        }

        /// <summary>
        /// Returns the teacher and class combination with specified ID.
        /// </summary>
        /// <param name="id">ID of the teacher and class combination</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TeacherClassDTO> GetTeacherAndClassById(int id)
        {
            TeacherClassDTO? foundTeacherClass = _service.GetById(id);

            if (foundTeacherClass == null)
            {
                return NotFound($"Teacher and class combination with id [{id}] was not found!");
            }
            else
            {
                return foundTeacherClass;
            }
        }

        /// <summary>
        /// Returns the teacher and class combinations with specified teacher ID - getting the classes in which
        /// a specific teacher teaches. Optionally, the logic can be reversed to return the classes in which
        /// the specified teacher does NOT teach.
        /// </summary>
        /// <param name="teacherId">The teacher ID of the teacher and class combination</param>
        /// <param name="reverse">Specifies whether the returned teacher and class combinations should not be
        /// with the provided teacher ID</param>
        /// <returns></returns>
        [HttpGet("ForTeacher/{teacherId}")]
        [HttpGet("ForTeacher/{teacherId}/{reverse}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<TeacherClassDTO>> GetTeacherAndClassByTeacherId(int teacherId, bool reverse = false)
        {
            List<TeacherClassDTO> foundTeacherClass = _service.GetByTeacherId(teacherId, reverse);
            return foundTeacherClass;
        }

        /// <summary>
        /// Saves a teacher and class combination to the database. Saving can be either updating an already existing
        /// entity or creating a completely new one. When ID of 0 is provided for the entity, it will
        /// be created, otherwise an update attempt will be made.
        /// </summary>
        /// <param name="saveTeacherClassDTO">DTO with necessary teacher and class combination data</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TeacherClassDTO> SaveTeacherAndClass(SaveTeacherClassDTO saveTeacherClassDTO)
        {
            TeacherClassDTO teacherClassDTO = new TeacherClassDTO(saveTeacherClassDTO);

            //Error code 400 is returned when validation with Validator fails
            if (!teacherClassDTO.Validate())
            {
                return BadRequest("Invalid data!");
            }

            if (_service.Save(teacherClassDTO))
            {
                return StatusCode(200, "Teacher and class combination is saved successfully!");
            }
            else
            {
                return StatusCode(500, "Failure to save teacher and class combination!");
            }
        }

        /// <summary>
        /// Deletes a teacher and class combination from the database with specified ID.
        /// </summary>
        /// <param name="id">ID of the teacher and class combination</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTeacherAndClass(int id)
        {
            if (_service.GetById(id) == null)
            {
                return NotFound($"Teacher and class combination with id [{id}] was not found!");
            }

            if (_service.Delete(id))
            {
                return StatusCode(200, "Teacher and class combination is deleted successfully!");
            }
            else
            {
                return StatusCode(500, "Failure to delete teacher and class combination!");
            }
        }
    }
}
