using Business.Interfaces;
using Entity.Dtos;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Web.Controllers.Interfaces;

namespace Web.Controllers.Implementations
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class RepositoryController<T, D> : ARepositoryController<T, D>, IRepositoryController<T, D> where T : BaseModel where D : BaseDto
    {
        private readonly IRepositoryBusiness<T, D> _business;

        public RepositoryController(IRepositoryBusiness<T, D> business)
        {
            _business = business;
        }

        /// <summary>
        /// GetAllSelect
        /// </summary>
        /// <returns></returns>
        [HttpGet("select")]
        public override  async Task<ActionResult<IEnumerable<D>>> GetAll()
        {
            try
            {
                var data = await _business.GetAll();
                if (data == null)
                {
                    var responseNull = new ApiResponse<IEnumerable<D>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<D>>((IEnumerable<D>)data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<D>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public override async Task<ActionResult<D>> GetById(int id)
        {
            try
            {
                var data = await _business.GetById(id);

                if (data == null)
                {
                    var responseNull = new ApiResponse<D>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }

                var response = new ApiResponse<D>(data, true, "Ok", null);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<D>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        /// <summary>
        /// Save
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public override async Task<ActionResult<D>> Save(D dto)
        {
            try
            {
                D dtoSaved = await _business.Save(dto);
                var response = new ApiResponse<D>(dtoSaved, true, "Registro almacenado exitosamente", null!);

                return new CreatedAtRouteResult(new { id = dtoSaved.Id }, response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<D>>(null!, false, ex.Message.ToString(), null!);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        public override  async Task<ActionResult<D>> Update(D dto)
        {
            try
            {
                await _business.Update(dto);

                var response = new ApiResponse<D>(dto, true, "Registro actualizado exitosamente", null!);

                return new CreatedAtRouteResult(new { id = dto.Id }, response);

            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<D>>(null!, false, ex.Message.ToString(), null!);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Permanent Delete 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("permanent/{id}")]
        public override async Task<ActionResult> PermanentDelete(int id)
        {
            try
            {
                bool registroAfectados = await _business.PermanentDelete(id);
                if (!registroAfectados)
                {
                    var errorResponse = new ApiResponse<IEnumerable<D>>(null, false, "Registro no eliminado!", null);
                    return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);

                }
                var successResponse = new ApiResponse<D>(null, true, "Registro eliminado permanentemente", null);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<IEnumerable<D>>(null, false, "¡El registro se encuentra asociado, no se puede eliminar!", null);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public override async Task<ActionResult> Delete(int id)
        {
            try
            {
                int registroAfectados = await _business.Delete(id);
                if (registroAfectados == 0)
                {
                    var errorResponse = new ApiResponse<IEnumerable<D>>(null, false, "Registro no eliminado!", null);
                    return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);

                }
                var successResponse = new ApiResponse<D>(null, true, "Registro eliminado exitosamente", null);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<IEnumerable<D>>(null, false, "¡El registro se encuentra asociado, no se puede eliminar!", null);
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpGet("dynamic")]
        public override async Task<IActionResult> GetDynamicAsync()
        {
            var result = await _business.GetAllDynamicAsync();
            return Ok(result);
        }

    }
}
