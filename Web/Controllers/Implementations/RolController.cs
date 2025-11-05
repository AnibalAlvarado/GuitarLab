using Business.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]

    public class RolController : RepositoryController<Rol, RolDto>
    {
      
        public RolController(IRolBusiness business )
            : base(business)
        {
        }
    }

    //public class RolController : RepositoryController<Rol, RolDto>
    //{
    //    public RolController(IRepositoryBusiness<Rol, RolDto> business) : base(business)
    //    {
    //    }
    //}
}
