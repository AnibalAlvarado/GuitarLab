using Business.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : RepositoryController<Form, FormDto>
    {

        public FormController(IFormBusiness business)
            : base(business)
        {
        }
    }
}
