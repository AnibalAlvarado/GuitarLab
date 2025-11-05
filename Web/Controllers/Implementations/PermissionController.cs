using Business.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : RepositoryController<Permission, PermissionDto>
    {
        public PermissionController(IPermissionBusiness business)
            : base(business)
        {
        }
    }
}
