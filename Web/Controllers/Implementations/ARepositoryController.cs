using Entity.Dtos;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Interfaces;

namespace Web.Controllers.Implementations
{
    public abstract class ARepositoryController<T,D> : ControllerBase, IRepositoryController<T,D> where T : BaseModel where D : BaseDto
    {

        public abstract Task<ActionResult<IEnumerable<D>>> GetAll();
        public abstract Task<ActionResult<D>> GetById(int id);
        public abstract Task<ActionResult<D>> Save(D dto);
        public abstract Task<ActionResult<D>> Update(D dto);
        public abstract Task<ActionResult> PermanentDelete(int id);
        public abstract Task<ActionResult> Delete(int id);
        public abstract Task<IActionResult> GetDynamicAsync();
    }
}
