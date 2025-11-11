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

    public class UserController : RepositoryController<User, UserDto>
    {
        private readonly IUserBusiness _business;
        public UserController(IUserBusiness business )
            : base(business)
        {
            _business = business;
        }

        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllJoin()
        {
            try
            {
                IEnumerable<UserDto> data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<UserDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<UserDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<UserDto>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }

    //public class RolController : RepositoryController<Rol, RolDto>
    //{
    //    public RolController(IRepositoryBusiness<Rol, RolDto> business) : base(business)
    //    {
    //    }
    //}
}
