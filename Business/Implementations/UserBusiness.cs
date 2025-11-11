using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Models;
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
        private readonly IPasswordHasher _passwordHasher;
        private readonly IGuitaristBusiness _guitaristBusiness;
        public UserBusiness(IUserData data, IMapper mapper, ILogger<UserBusiness> logger, IPasswordHasher passwordHasher, IGuitaristBusiness guitaristBusiness)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _guitaristBusiness = guitaristBusiness;
        }

        public async Task<UserDto> LoginUser(LoginRequestDto loginDto)
        {
            User? user = await _data.GetByEmailOrUsernameAsync(loginDto.Email);
            UserDto userDto = _mapper.Map<UserDto>(user);
            if (user == null)
            {
                _logger.LogWarning("Intento de login fallido para usuario: {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            bool valid = _passwordHasher.VerifyHashedPassword(user.Password, loginDto.Password);

            if (!valid)
            {
                _logger.LogWarning("Contraseña incorrecta para usuario: {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            _logger.LogInformation("Inicio de sesión exitoso para usuario: {Email}", user.Email);
            return userDto;
        }

        public async Task<UserDto> RegisterAsync(RegisterRequestDto dto)
        {
            try
            {
                // 🔍 Validaciones básicas
                if (string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Email))
                    throw new ValidationException("El correo y la contraseña son obligatorios.");

                if (await _data.ExistsAsync(u => u.Email == dto.Email))
                    throw new BusinessException("Ya existe un usuario registrado con este correo.");

                if (await _data.ExistsAsync(u => u.Username == dto.Username))
                    throw new BusinessException("El nombre de usuario ya está en uso.");

                // 🔐 Validar seguridad mínima de contraseña (opcional)
                if (dto.Password.Length < 6)
                    throw new BusinessException("La contraseña debe tener al menos 6 caracteres.");

                // 🎸 Crear entidad Guitarist
                GuitaristDto guitarist = new GuitaristDto
                {
                    Name = dto.Name,
                    SkillLevel = dto.SkillLevel,
                    ExperienceYears = dto.ExperienceYears,
                    Asset = true
                };

                // 🧠 Guardar guitarista en la base de datos
                GuitaristDto guitaristReturn = await _guitaristBusiness.Save(guitarist);

                // 👤 Crear entidad User y asociar Guitarist
                User user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Password = _passwordHasher.HashPassword(dto.Password), // Encriptado bcrypt
                    GuitaristId = guitaristReturn.Id
                };

                // 🧾 Guardar usuario en la base de datos
                User userReturn = await _data.Save(user);
                // 🧠 Retornar DTO mapeado
                return _mapper.Map<UserDto>(userReturn);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error en el registro del usuario: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllJoinAsync()
        {
            return await _data.GetAllJoinAsync();
        }

    }
}
