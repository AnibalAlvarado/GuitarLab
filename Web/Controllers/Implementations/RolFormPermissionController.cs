using Business.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolFormPermissionController : RepositoryController<RolFormPermission, RolFormPermissionDto>
    {
        private readonly IRolFormPermissionBusiness _business;
        public RolFormPermissionController(IRolFormPermissionBusiness business)
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<RolFormPermissionDto>>> GetAllJoin()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<RolFormPermissionDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<RolFormPermissionDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<RolFormPermissionDto>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
