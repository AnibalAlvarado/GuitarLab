using AutoMapper;
using Business.Implementations;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Enums;
using Entity.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Utilities.Exceptions;
using Utilities.Interfaces;

namespace test.Business
{
    public class UserBusinessTests
    {
        private readonly Mock<IUserData> _userDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UserBusiness>> _loggerMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IGuitaristBusiness> _guitaristBusinessMock;
        private readonly UserBusiness _business;

        public UserBusinessTests()
        {
            _userDataMock = new Mock<IUserData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UserBusiness>>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _guitaristBusinessMock = new Mock<IGuitaristBusiness>();
            _business = new UserBusiness(_userDataMock.Object, _mapperMock.Object, _loggerMock.Object, _passwordHasherMock.Object, _guitaristBusinessMock.Object);
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnMappedUserDtos()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "user1@example.com", IsDeleted = false },
                new User { Id = 2, Username = "user2", Email = "user2@example.com", IsDeleted = false }
            };

            var userDtos = new List<UserDto>
            {
                new UserDto { Id = 1, Username = "user1", Email = "user1@example.com" },
                new UserDto { Id = 2, Username = "user2", Email = "user2@example.com" }
            };

            _userDataMock.Setup(d => d.GetAll()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<List<UserDto>>(users)).Returns(userDtos);

            // Act
            var result = await _business.GetAll();

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
            result.First().Username.Should().Be("user1");
            result.Last().Username.Should().Be("user2");
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnMappedUserDto_WhenExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "user1", Email = "user1@example.com", IsDeleted = false };
            var userDto = new UserDto { Id = 1, Username = "user1", Email = "user1@example.com" };

            _userDataMock.Setup(d => d.GetById(1)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _business.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("user1");
            result.Email.Should().Be("user1@example.com");
        }

        [Fact]
        public async Task GetById_ShouldThrowValidationException_WhenIdIsInvalid()
        {
            // Act
            Func<Task> act = async () => await _business.GetById(0);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*El ID debe ser mayor que cero.*");
        }

        [Fact]
        public async Task GetById_ShouldThrowEntityNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            _userDataMock.Setup(d => d.GetById(999)).ReturnsAsync((User)null!);

            // Act
            Func<Task> act = async () => await _business.GetById(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*User*999*");
        }

        // ========================================================
        // TEST 3: Save
        // ========================================================
        [Fact]
        public async Task Save_ShouldMapAndSaveUserDto()
        {
            // Arrange
            var userDto = new UserDto { Username = "newuser", Email = "newuser@example.com" };
            var user = new User { Id = 0, Username = "newuser", Email = "newuser@example.com", IsDeleted = false };
            var savedUser = new User { Id = 1, Username = "newuser", Email = "newuser@example.com", IsDeleted = false };
            var savedUserDto = new UserDto { Id = 1, Username = "newuser", Email = "newuser@example.com" };

            _mapperMock.Setup(m => m.Map<User>(userDto)).Returns(user);
            _userDataMock.Setup(d => d.Save(user)).ReturnsAsync(savedUser);
            _mapperMock.Setup(m => m.Map<UserDto>(savedUser)).Returns(savedUserDto);

            // Act
            var result = await _business.Save(userDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Username.Should().Be("newuser");
            result.Email.Should().Be("newuser@example.com");
        }

        [Fact]
        public async Task Save_ShouldThrowBusinessException_WhenSaveFails()
        {
            // Arrange
            var userDto = new UserDto { Username = "newuser" };
            var user = new User { Username = "newuser" };

            _mapperMock.Setup(m => m.Map<User>(userDto)).Returns(user);
            _userDataMock.Setup(d => d.Save(user)).ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _business.Save(userDto);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*Error al crear la entidad.*");
        }

        // ========================================================
        // TEST 4: Update
        // ========================================================
        [Fact]
        public async Task Update_ShouldThrowBusinessException_WhenUpdateFails()
        {
            // Arrange
            var userDto = new UserDto { Id = 1, Username = "updateduser" };
            var user = new User { Id = 1, Username = "updateduser" };

            _mapperMock.Setup(m => m.Map<User>(userDto)).Returns(user);
            _userDataMock.Setup(d => d.Update(user)).ThrowsAsync(new Exception("Update error"));

            // Act
            Func<Task> act = async () => await _business.Update(userDto);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*Error al actualizar la entidad.*");
        }

        // ========================================================
        // TEST 5: Delete
        // ========================================================
        [Fact]
        public async Task Delete_ShouldReturnRowsAffected_WhenSuccessful()
        {
            // Arrange
            _userDataMock.Setup(d => d.Delete(1)).ReturnsAsync(1);

            // Act
            var result = await _business.Delete(1);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task Delete_ShouldThrowValidationException_WhenIdIsInvalid()
        {
            // Act
            Func<Task> act = async () => await _business.Delete(0);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*El ID debe ser mayor que cero.*");
        }

        // ========================================================
        // TEST 6: PermanentDelete
        // ========================================================
        [Fact]
        public async Task PermanentDelete_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            _userDataMock.Setup(d => d.PermanentDelete(1)).ReturnsAsync(true);

            // Act
            var result = await _business.PermanentDelete(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task PermanentDelete_ShouldThrowValidationException_WhenIdIsInvalid()
        {
            // Act
            Func<Task> act = async () => await _business.PermanentDelete(0);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*El ID debe ser mayor que cero.*");
        }

        [Fact]
        public async Task PermanentDelete_ShouldThrowEntityNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            _userDataMock.Setup(d => d.PermanentDelete(999)).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _business.PermanentDelete(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*User*999*");
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            _userDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync(true);

            // Act
            var result = await _business.ExistsAsync(e => e.Username == "Test");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldThrowBusinessException_WhenExistsFails()
        {
            // Arrange
            _userDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ThrowsAsync(new Exception("Exists error"));

            // Act
            Func<Task> act = async () => await _business.ExistsAsync(e => e.Username == "Test");

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*Error al validar existencia de la entidad.*");
        }

        // ========================================================
        // TEST 8: GetAllDynamicAsync
        // ========================================================
        [Fact]
        public async Task GetAllDynamicAsync_ShouldReturnDynamicObjects()
        {
            // Arrange
            var dynamicObjects = new List<System.Dynamic.ExpandoObject>();
            _userDataMock.Setup(d => d.GetAllDynamicAsync()).ReturnsAsync(dynamicObjects);

            // Act
            var result = await _business.GetAllDynamicAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dynamicObjects);
        }

        // ========================================================
        // TEST 9: LoginUser
        // ========================================================
        [Fact]
        public async Task LoginUser_ShouldReturnUserDto_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginRequestDto { Email = "user@example.com", Password = "password123" };
            var user = new User { Id = 1, Username = "user", Email = "user@example.com", Password = "hashedpassword" };
            var userDto = new UserDto { Id = 1, Username = "user", Email = "user@example.com" };

            _userDataMock.Setup(d => d.GetByEmailOrUsernameAsync("user@example.com")).ReturnsAsync(user);
            _passwordHasherMock.Setup(p => p.VerifyHashedPassword("hashedpassword", "password123")).Returns(true);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _business.LoginUser(loginDto);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("user");
            result.Email.Should().Be("user@example.com");
        }

        [Fact]
        public async Task LoginUser_ShouldThrowUnauthorizedAccessException_WhenUserNotFound()
        {
            // Arrange
            var loginDto = new LoginRequestDto { Email = "nonexistent@example.com", Password = "password123" };

            _userDataMock.Setup(d => d.GetByEmailOrUsernameAsync("nonexistent@example.com")).ReturnsAsync((User)null!);

            // Act
            Func<Task> act = async () => await _business.LoginUser(loginDto);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Credenciales inválidas");
        }

        [Fact]
        public async Task LoginUser_ShouldThrowUnauthorizedAccessException_WhenPasswordIsInvalid()
        {
            // Arrange
            var loginDto = new LoginRequestDto { Email = "user@example.com", Password = "wrongpassword" };
            var user = new User { Id = 1, Username = "user", Email = "user@example.com", Password = "hashedpassword" };

            _userDataMock.Setup(d => d.GetByEmailOrUsernameAsync("user@example.com")).ReturnsAsync(user);
            _passwordHasherMock.Setup(p => p.VerifyHashedPassword("hashedpassword", "wrongpassword")).Returns(false);

            // Act
            Func<Task> act = async () => await _business.LoginUser(loginDto);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Credenciales inválidas");
        }

        // ========================================================
        // TEST 10: RegisterAsync
        // ========================================================
        [Fact]
        public async Task RegisterAsync_ShouldReturnUserDto_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerDto = new RegisterRequestDto
            {
                Name = "John Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "password123",
                SkillLevel = SkillLevel.Beginner,
                ExperienceYears = 2
            };

            var guitaristDto = new GuitaristDto { Id = 1, Name = "John Doe", SkillLevel = SkillLevel.Beginner, ExperienceYears = 2 };
            var user = new User { Id = 1, Username = "johndoe", Email = "john@example.com", Password = "hashedpassword", GuitaristId = 1 };
            var userDto = new UserDto { Id = 1, Username = "johndoe", Email = "john@example.com" };

            _userDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync(false);
            _guitaristBusinessMock.Setup(g => g.Save(It.IsAny<GuitaristDto>())).ReturnsAsync(guitaristDto);
            _passwordHasherMock.Setup(p => p.HashPassword("password123")).Returns("hashedpassword");
            _userDataMock.Setup(d => d.Save(It.IsAny<User>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _business.RegisterAsync(registerDto);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("johndoe");
            result.Email.Should().Be("john@example.com");
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowValidationException_WhenPasswordIsNullOrEmpty()
        {
            // Arrange
            var registerDto = new RegisterRequestDto { Email = "john@example.com" };

            // Act
            Func<Task> act = async () => await _business.RegisterAsync(registerDto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("El correo y la contraseña son obligatorios.");
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowBusinessException_WhenEmailAlreadyExists()
        {
            // Arrange
            var registerDto = new RegisterRequestDto { Email = "existing@example.com", Password = "password123" };

            _userDataMock.Setup(d => d.ExistsAsync(u => u.Email == "existing@example.com")).ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _business.RegisterAsync(registerDto);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Ya existe un usuario registrado con este correo.");
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowBusinessException_WhenUsernameAlreadyExists()
        {
            // Arrange
            var registerDto = new RegisterRequestDto { Username = "existinguser", Email = "new@example.com", Password = "password123" };

            _userDataMock.Setup(d => d.ExistsAsync(u => u.Email == "new@example.com")).ReturnsAsync(false);
            _userDataMock.Setup(d => d.ExistsAsync(u => u.Username == "existinguser")).ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _business.RegisterAsync(registerDto);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("El nombre de usuario ya está en uso.");
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowBusinessException_WhenPasswordIsTooShort()
        {
            // Arrange
            var registerDto = new RegisterRequestDto { Email = "john@example.com", Password = "123" };

            // Act
            Func<Task> act = async () => await _business.RegisterAsync(registerDto);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("La contraseña debe tener al menos 6 caracteres.");
        }
    }
}