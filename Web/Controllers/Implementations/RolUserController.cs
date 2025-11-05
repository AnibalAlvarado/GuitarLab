using Business.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolUserController : RepositoryController<RolUser, RolUserDto>
    {
        private readonly IRolUserBusiness _business;
        public RolUserController(IRolUserBusiness business)
            : base(business)
        {
            _business = business;
        }

        // Endpoint personalizado para GetAllJoinAsync
        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<RolUserDto>>> GetAllJoinAsync()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<RolUserDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<RolUserDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<RolUserDto>>(null, false, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
