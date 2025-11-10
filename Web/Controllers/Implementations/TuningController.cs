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

    public class TuningController : RepositoryController<Tuning, TuningDto>
    {
      
        public TuningController(ITuningBusiness business )
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
