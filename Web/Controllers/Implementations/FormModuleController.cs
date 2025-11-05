using Business.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormModuleController : RepositoryController<FormModule, FormModuleDto>
    {
        private readonly IFormModuleBusiness _business;
        public FormModuleController(IFormModuleBusiness business)
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<FormModuleDto>>> GetAllJoin()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<FormModuleDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<FormModuleDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<FormModuleDto>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}
