using AutoMapper;
using Business.Interfaces;
using Data.Implementations;
using Data.Interfaces;
using Entity.Dtos;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Interfaces;

namespace Business.Implementations
{
    public class UserBusiness : RepositoryBusiness<User, UserDto>, IUserBusiness
    {
        private readonly IUserData _data;
        private readonly IMapper _mapper;
        private readonly ILogger<UserBusiness> _logger;
        private readonly IEmailService _emailService;
        private readonly IRolBusiness _rolBusiness;
        private readonly IRolUserBusiness _rolUserBusiness;
        public UserBusiness(
        IUserData data, IMapper mapper, ILogger<UserBusiness> logger,IEmailService emailService,IRolBusiness rolBusiness,IRolUserBusiness rolUserBusiness) : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
            _rolBusiness = rolBusiness;
            _rolUserBusiness = rolUserBusiness;
        }

        // Obtener todos los usuarios con información de persona

        // Obtener usuario por ID con información de persona
        public override async Task<UserDto> GetById(int id)
        {
            try
            {
                var user = await _data.GetById(id);
                if (user == null)
                    return null;

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por ID con información de persona");
                throw;
            }
        }

        public override async Task<UserDto> Save(UserDto dto)
        {
            try
            {
                // Aplicar valores predeterminados
                dto.Asset = true;

                // Hashear la contraseña antes de guardar
                if (!string.IsNullOrEmpty(dto.Password))
                {
                    dto.Password = HashPassword(dto.Password);
                }

                // Mapear DTO a entidad
                User entity = _mapper.Map<User>(dto);

                // Guardar entidad en la base de datos
                entity = await _data.Save(entity);

                // Mapear la entidad guardada de vuelta a DTO
                var savedDto = _mapper.Map<UserDto>(entity);

                // Mapear la entidad guardada de vuelta a DTO y devolverla
                return savedDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario.");
                throw new BusinessException("Error al crear el usuario.", ex);
            }
        }

        public async Task<UserResponseDto?> ValidateUserAsync(string username, string password)
        {
            try
            {
                // Validaciones iniciales
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    _logger.LogWarning("Intento de validación con username o password vacíos");
                    return null;
                }

                // Obtener usuario por nombre de usuario
                var user = await _data.GetUserByUsernameAsync(username);

                // Si el usuario no existe o la contraseña no coincide, retorna null
                if (user == null)
                {
                    _logger.LogWarning("Usuario no encontrado durante validación: {Username}", username);
                    return null;
                }

                if (!VerifyPassword(password, user.Password))
                {
                    _logger.LogWarning("Contraseña incorrecta para el usuario: {Username}", username);
                    return null;
                }

                // Obtener el rol del usuario a través de la tabla pivote
                var roleName = await _data.GetUserRoleAsync(user.Id);

                // Usar AutoMapper para crear el DTO de respuesta
                var userResponseDto = _mapper.Map<UserResponseDto>(user);
                userResponseDto.Role = roleName;
                userResponseDto.UserId = user.Id;

                return userResponseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la validación del usuario: {Username}", username);
                throw;
            }
        }


        public string HashPassword(string password)
        {
            try
            {
                // BCrypt automáticamente genera y maneja la sal
                return BCrypt.Net.BCrypt.HashPassword(password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar hash de contraseña");
                throw;
            }
        }

        // Para verificar una contraseña contra un hash almacenado
        public bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            try
            {
                // BCrypt verifica la contraseña contra el hash (que incluye la sal)
                return BCrypt.Net.BCrypt.Verify(inputPassword, storedPasswordHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar contraseña");
                throw;
            }
        }

        public async Task AssignDefaultRoleAsync(int userId)
        {
            // 1. Verificar que el usuario exista
            var user = await _data.GetById(userId);
            if (user == null)
                throw new Exception("El usuario no existe.");

            // 2. Obtener el rol por defecto ya creado
            var defaultRole = await _rolBusiness.GetByNameAsync("Usuario"); // Cambia "Usuario" por el nombre exacto si es diferente
            if (defaultRole == null)
                throw new Exception("El rol por defecto no existe.");

            // 3. Verificar si ya tiene el rol asignado
            var alreadyAssigned = await _rolUserBusiness.ExistsAsync(userId, defaultRole.Id);
            if (alreadyAssigned)
                return;

            // 4. Asignar el rol al usuario
            var userRoleDto = new RolUserDto
            {
                UserId = user.Id,
                RolId = defaultRole.Id
            };

            await _rolUserBusiness.Save(userRoleDto);
        }

        public async Task SendWelcomeEmailAsync(string to)
        {
            try
            {
                // Armas el mensaje de bienvenida (puedes personalizarlo)
                string message = "¡Bienvenido! Tu usuario ha sido creado exitosamente.";

                // Llamas al servicio de email para enviar
                await _emailService.SendEmailAsync(to, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo de bienvenida al usuario: {Email}", to);
                // Decide si quieres relanzar la excepción o solo loguear
                // throw;
            }
        }

    }
}
