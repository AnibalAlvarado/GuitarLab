using Entity.Dtos;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Interfaces
{
    public interface IRepositoryController<T,D> where T : BaseModel where D : BaseDto
    {
        Task<ActionResult<IEnumerable<D>>> GetAll();

        Task<ActionResult<D>> GetById(int id);

        Task<ActionResult<D>> Save(D dto);

        Task<ActionResult<D>> Update(D dto);

        Task<ActionResult> PermanentDelete(int id);

        Task<ActionResult> Delete(int id);
        Task<IActionResult> GetDynamicAsync();
    }
}
