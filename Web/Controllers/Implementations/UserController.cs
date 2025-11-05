using Business.Implementations;
using Business.Interfaces;
using Entity.Dtos;
using Entity.DTOs;
using Entity.Model;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Utilities.BackgroundTasks;

namespace Web.Controllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : RepositoryController<User, UserDto>
    {
        private readonly IUserBusiness _business;
        private readonly ILogger<UserController> _logger;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        public UserController(IUserBusiness business, ILogger<UserController> logger, IBackgroundTaskQueue backgroundTaskQueue)
            : base(business)
        {
            _logger = logger;
            _business = business;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<UserResponseDto>(null,false,"Datos de inicio de sesión inválidos",ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                // Validar credenciales
                var user = await _business.ValidateUserAsync(loginRequest.Username, loginRequest.Password);

                // Si no se encuentra el usuario o las credenciales son incorrectas
                if (user == null)
                {
                    return Unauthorized(new ApiResponse<UserResponseDto>(null,false,"Nombre de usuario o contraseña incorrectos",null));
                }

                // Devolver la información del usuario autenticado
                return Ok(new ApiResponse<UserResponseDto>(user,true,"Inicio de sesión exitoso",null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la autenticación del usuario");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<UserResponseDto>(null,false,"Error interno del servidor durante la autenticación",null));
            }
        }

        [HttpPost]
        public override async Task<ActionResult<UserDto>> Save(UserDto dto)
        {
            try
            {
                // Guardar usuario y asignar rol por defecto dentro del método Save del Business
                UserDto dtoSaved = await _business.Save(dto);

                _backgroundTaskQueue.Enqueue(async token =>
                {
                    try
                    {
                        _logger.LogInformation(" Iniciando tarea en segundo plano para enviar correo...");

                        // Simular demora
                        await Task.Delay(3000, token);
                        token.ThrowIfCancellationRequested();

                        if (string.IsNullOrWhiteSpace(dtoSaved.Email))
                        {
                            _logger.LogWarning("No se envió el correo porque el email está vacío para el usuario ID {UserId}", dtoSaved.Id);
                            return;
                        }

                        await _business.SendWelcomeEmailAsync(dtoSaved.Email);
                        _logger.LogInformation("Tarea completada: Correo enviado.");
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogWarning("Tarea cancelada antes de enviar el correo a {Email}", dtoSaved.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en la tarea en segundo plano al enviar correo a {Email}", dtoSaved.Email);
                    }
                });
                _logger.LogInformation(" Tarea encolada. El controlador sigue respondiendo.");

                var response = new ApiResponse<UserDto>(dtoSaved, true, "Registro almacenado exitosamente", null!);

                return new CreatedAtRouteResult(new { id = dtoSaved.Id }, response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<UserDto>>(null!, false, ex.Message.ToString(), null!);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
