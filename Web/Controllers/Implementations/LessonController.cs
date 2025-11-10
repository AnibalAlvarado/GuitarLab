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
    public class LessonController : RepositoryController<Lesson, LessonDto>
    {
        public LessonController(ILessonBusiness business)
            : base(business)
        {
        }
    }
}
