using Business.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : RepositoryController<Person, PersonDto>
    {
        public PersonController(IPersonBusiness business)
            : base(business)
        {
        }
    }
}
