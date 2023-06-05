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
    [Route("api/Classes")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class ClassesController : ControllerBase
    {
        private readonly ClassManagementService _service = null!;

        public ClassesController()
        {
            _service = new ClassManagementService();
        }

        /// <summary>
        /// Returns a list of classes from a page. The number of classes per page and the number
        /// of the desired page must be specified. Optionally, classes can be filtered by their name.
        /// </summary>
        /// <param name="pageNumber">The number of the requested page</param>
        /// <param name="itemsPerPage">The number of items for every page</param>
        /// <param name="className">Searched class name</param>
        /// <returns></returns>
        [HttpGet("{pageNumber}/{itemsPerPage}")]
        [HttpGet("{pageNumber}/{itemsPerPage}/{className}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<ClassDTO>> GetClasses(int pageNumber = 1, int itemsPerPage = 10, string? className = null)
        {
            return _service.Get(pageNumber, itemsPerPage, className);
        }

        /// <summary>
        /// Returns the count of pages if all pages have the same number of classes as specified.
        /// Optionally, classes can be filtered by their name before counting the pages.
        /// </summary>
        /// <param name="itemsPerPage">The number of items for every page</param>
        /// <param name="className">Searched class name</param>
        /// <returns></returns>
        [HttpGet("PageCount/{itemsPerPage}")]
        [HttpGet("PageCount/{itemsPerPage}/{className}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<int> GetClassPageCount(int itemsPerPage, string? className = null)
        {
            return _service.GetPageCount(itemsPerPage, className);
        }

        /// <summary>
        /// Returns the class with specified ID.
        /// </summary>
        /// <param name="id">ID of the class</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ClassDTO> GetClassById(int id)
        {
            ClassDTO? foundClass = _service.GetById(id);

            if (foundClass == null)
            {
                return NotFound($"Class with id [{id}] was not found!");
            }
            else
            {
                return foundClass;
            }
        }

        /// <summary>
        /// Saves a class to the database. Saving can be either updating an already existing
        /// entity or creating a completely new one. When ID of 0 is provided for the entity, it will
        /// be created, otherwise an update attempt will be made.
        /// Note: The class specialization can be null or one of the following 
        /// ["Foreign languages", "Natural sciences", "Social sciences"].
        /// </summary>
        /// <param name="saveClassDTO">DTO with necessary class data</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ClassDTO> SaveClass(SaveClassDTO saveClassDTO)
        {
            ClassDTO classDTO = new ClassDTO(saveClassDTO);

            //Error code 400 is returned when validation with Validator fails
            if (!classDTO.Validate())
            {
                return BadRequest("Invalid data! Ensure that the specialization is null or" +
                    " in this list [\"Foreign languages\", \"Natural sciences\", \"Social sciences\"]");
            }

            if (_service.Save(classDTO))
            {
                return StatusCode(200, "Class is saved successfully!");
            }
            else
            {
                return StatusCode(500, "Failure to save class!");
            }
        }

        /// <summary>
        /// Deletes a class from the database with specified ID.
        /// </summary>
        /// <param name="id">The ID of the class</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteClass(int id)
        {
            if (_service.GetById(id) == null)
            {
                return NotFound($"Class with id [{id}] was not found!");
            }

            if (_service.Delete(id))
            {
                return StatusCode(200, "Class is deleted successfully!");
            }
            else
            {
                return StatusCode(500, "Failure to delete class!");
            }
        }
    }
}
