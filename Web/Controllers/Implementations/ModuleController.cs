using Business.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleController : RepositoryController<Module, ModuleDto>
    {
        public ModuleController(IModuleBusiness business)
            : base(business)
        {
        }
    }
}
