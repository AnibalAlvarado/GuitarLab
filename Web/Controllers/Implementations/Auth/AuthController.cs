using Business.Implementations;
using Business.Interfaces;
using Business.Interfaces.Auth;
using Entity.Dtos;
using Entity.Dtos.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Web.Infrastructure.Cookies.Interfaces;

namespace ModelSecurity.Controllers.Implements.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<AuthController> _logger;
        private readonly ITokenBusiness _token;
        private readonly IUserBusiness _userBusiness;
        private readonly JwtSettings _jwt;
        private readonly CookieSettings _cookieSettings;
        private readonly IAuthCookieFactory _cookieFactory;


        public AuthController(ILogger<AuthController> logger,
            ITokenBusiness token,
            IUserBusiness userBusiness,
            IOptions<JwtSettings> jwtOptions,
            IOptions<CookieSettings> cookieOptions,
            IAuthCookieFactory cookieFactory)
        {

            _logger = logger;
            _token = token;
            _jwt = jwtOptions.Value;
            _cookieSettings = cookieOptions.Value;
            _cookieFactory = cookieFactory;
            _userBusiness = userBusiness;

        }


        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Registrarse(RegisterRequestDto objeto)
        {
            try
            {
                UserDto userCreated = await _userBusiness.RegisterAsync(objeto);

                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
        {
            try
            {
                var (access, refresh, csrf) = await _token.GenerateTokensAsync(dto);

                var now = DateTime.UtcNow;

                Response.Cookies.Append(
                    _cookieSettings.AccessTokenName,
                    access,
                    _cookieFactory.AccessCookieOptions(now.AddMinutes(_jwt.AccessTokenExpirationMinutes)));

                Response.Cookies.Append(
                    _cookieSettings.RefreshTokenName,
                    refresh,
                    _cookieFactory.RefreshCookieOptions(now.AddDays(_jwt.RefreshTokenExpirationDays)));

                Response.Cookies.Append(
                    _cookieSettings.CsrfCookieName,
                    csrf,
                    _cookieFactory.CsrfCookieOptions(now.AddDays(_jwt.RefreshTokenExpirationDays)));

                return Ok(new { isSuccess = true, message = "Login exitoso" });
            }
            catch (UnauthorizedAccessException)
            {
                // Mensaje controlado y status 401
                return Unauthorized(new { isSuccess = false, message = "Credenciales inválidas" });
            }
        }



        /// <summary>Renueva tokens (usa refresh cookie + comprobación CSRF double-submit).</summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            var refreshCookie = Request.Cookies[_cookieSettings.RefreshTokenName];
            if (string.IsNullOrWhiteSpace(refreshCookie))
                return Unauthorized();

            // Validación CSRF (double-submit: header debe igualar cookie)
            if (!Request.Headers.TryGetValue("X-XSRF-TOKEN", out var headerValue))
                return Forbid();

            var csrfCookie = Request.Cookies[_cookieSettings.CsrfCookieName];
            if (string.IsNullOrWhiteSpace(csrfCookie) || csrfCookie != headerValue)
                return Forbid();

            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var (newAccess, newRefresh) = await _token.RefreshAsync(refreshCookie, remoteIp);

            var now = DateTime.UtcNow;

            // Eliminar cookies anteriores con las MISMAS opciones (path/domain/samesite) para asegurar borrado
            Response.Cookies.Delete(_cookieSettings.AccessTokenName, _cookieFactory.AccessCookieOptions(now));
            Response.Cookies.Delete(_cookieSettings.RefreshTokenName, _cookieFactory.RefreshCookieOptions(now));

            // Escribir nuevas
            Response.Cookies.Append(
                _cookieSettings.AccessTokenName,
                newAccess,
                _cookieFactory.AccessCookieOptions(now.AddMinutes(_jwt.AccessTokenExpirationMinutes)));

            Response.Cookies.Append(
                _cookieSettings.RefreshTokenName,
                newRefresh,
                _cookieFactory.RefreshCookieOptions(now.AddDays(_jwt.RefreshTokenExpirationDays)));

            return Ok(new { isSuccess = true });
        }


        /// <summary>Logout: revoca refresh token y borra cookies.</summary>
        [HttpPost("logout")]
        [AllowAnonymous] // puede hacerse sin estar autenticado si solo borra cookies
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var refreshCookie = Request.Cookies[_cookieSettings.RefreshTokenName];
            if (!string.IsNullOrWhiteSpace(refreshCookie))
            {
                await _token.RevokeRefreshTokenAsync(refreshCookie);
            }

            var now = DateTime.UtcNow;
            Response.Cookies.Delete(_cookieSettings.AccessTokenName, _cookieFactory.AccessCookieOptions(now));
            Response.Cookies.Delete(_cookieSettings.RefreshTokenName, _cookieFactory.RefreshCookieOptions(now));
            Response.Cookies.Delete(_cookieSettings.CsrfCookieName, _cookieFactory.CsrfCookieOptions(now));

            return Ok(new { message = "Sesión cerrada" });
        }

        /// <summary>Revoca el refresh token actual (si existe) y elimina la cookie.</summary>
        [HttpPost("revoke-token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RevokeToken()
        {
            var refreshToken = Request.Cookies[_cookieSettings.RefreshTokenName];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest(new { message = "No hay refresh token" });

            await _token.RevokeRefreshTokenAsync(refreshToken);

            var now = DateTime.UtcNow;
            Response.Cookies.Delete(_cookieSettings.RefreshTokenName, _cookieFactory.RefreshCookieOptions(now));

            return Ok(new { message = "Refresh token revocado" });
        }

        /// <summary>
        /// Retorna la información básica del usuario autenticado.
        /// Usa el access_token (desde la cookie) validado por JWT middleware.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            // 1️⃣ Extraer el userId desde los claims del JWT
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Usuario no autenticado" });

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Id de usuario inválido" });

            // 2️⃣ Obtener datos del usuario desde el negocio
            var user = await _userBusiness.GetById(userId);
            if (user == null)
                return Unauthorized(new { message = "Usuario no encontrado" });

            // 3️⃣ Retornar datos limpios (sin contraseñas ni datos sensibles)
            return Ok(new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                message = "Usuario autenticado"
            });
        }


    }
}