using Business.Interfaces;
using Entity.Dtos;
using Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class LessonController : RepositoryController<Lesson, LessonDto>
    {
        private readonly ILessonBusiness _business;
        public LessonController(ILessonBusiness business)
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<LessonDto>>> GetAllJoin()
        {
            try
            {
                IEnumerable<LessonDto> data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<LessonDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<LessonDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<LessonDto>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
