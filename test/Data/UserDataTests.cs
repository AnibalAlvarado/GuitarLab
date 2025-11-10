using Data.Implementations;
using Entity.Contexts;
using Entity.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace test.Data
{
    public class UserDataTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UserData _repository;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public UserDataTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _configurationMock = new Mock<IConfiguration>();
            _auditServiceMock = new Mock<IAuditService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _repository = new UserData(_context, _configurationMock.Object, _auditServiceMock.Object, _currentUserServiceMock.Object);

            // ================
            // Datos base
            // ================
            var guitarist1 = new Guitarist { Id = 1, Name = "John Doe", IsDeleted = false };
            var guitarist2 = new Guitarist { Id = 2, Name = "Jane Smith", IsDeleted = false };

            var user1 = new User
            {
                Id = 1,
                Username = "johndoe",
                Email = "john@example.com",
                Password = "hashedpassword1",
                GuitaristId = 1,
                Guitarist = guitarist1,
                IsDeleted = false
            };

            var user2 = new User
            {
                Id = 2,
                Username = "janesmith",
                Email = "jane@example.com",
                Password = "hashedpassword2",
                GuitaristId = 2,
                Guitarist = guitarist2,
                IsDeleted = true
            };

            _context.Guitarists.AddRange(guitarist1, guitarist2);
            _context.Users.AddRange(user1, user2);
            _context.SaveChanges();
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnOnlyActiveUsers()
        {
            var result = await _repository.GetAll();

            result.Should().HaveCount(1);
            result.First().Username.Should().Be("johndoe");
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnUser_WhenExistsAndActive()
        {
            var result = await _repository.GetById(1);

            result.Should().NotBeNull();
            result!.Username.Should().Be("johndoe");
            result.Email.Should().Be("john@example.com");
            result.Guitarist.Should().NotBeNull();
            result.Guitarist.Name.Should().Be("John Doe");
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenDeleted()
        {
            var result = await _repository.GetById(2);
            result.Should().BeNull();
        }

        // ========================================================
        // TEST 3: Save
        // ========================================================
        [Fact]
        public async Task Save_ShouldAddNewUser()
        {
            var newUser = new User
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "newpassword",
                GuitaristId = 1
            };

            var result = await _repository.Save(newUser);

            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Username.Should().Be("newuser");

            var saved = await _context.Users.FindAsync(result.Id);
            saved.Should().NotBeNull();
        }

        // ========================================================
        // TEST 4: Update
        // ========================================================
        [Fact]
        public async Task Update_ShouldModifyExistingUser()
        {
            var user = await _context.Users.FindAsync(1);
            user!.Email = "updated@example.com";
            await _repository.Update(user);


            var updated = await _context.Users.FindAsync(1);
            updated!.Email.Should().Be("updated@example.com");
        }

        // ========================================================
        // TEST 5: Delete
        // ========================================================
        [Fact]
        public async Task Delete_ShouldMarkUserAsDeleted()
        {
            var result = await _repository.Delete(1);

            result.Should().Be(1); // Rows affected

            var deleted = await _context.Users.FindAsync(1);
            deleted!.IsDeleted.Should().BeTrue();
        }

        // ========================================================
        // TEST 6: PermanentDelete
        // ========================================================
        [Fact]
        public async Task PermanentDelete_ShouldRemoveUserFromDatabase()
        {
            var result = await _repository.PermanentDelete(2);

            result.Should().BeTrue();

            var deleted = await _context.Users.FindAsync(2);
            deleted.Should().BeNull();
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenUserExists()
        {
            var result = await _repository.ExistsAsync(u => u.Username == "johndoe");
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            var result = await _repository.ExistsAsync(u => u.Username == "nonexistent");
            result.Should().BeFalse();
        }

        // ========================================================
        // TEST 8: GetByEmailOrUsernameAsync
        // ========================================================
        [Fact]
        public async Task GetByEmailOrUsernameAsync_ShouldReturnUser_WhenEmailMatches()
        {
            var result = await _repository.GetByEmailOrUsernameAsync("john@example.com");

            result.Should().NotBeNull();
            result!.Username.Should().Be("johndoe");
            result.Email.Should().Be("john@example.com");
        }

        [Fact]
        public async Task GetByEmailOrUsernameAsync_ShouldReturnUser_WhenUsernameMatches()
        {
            var result = await _repository.GetByEmailOrUsernameAsync("johndoe");

            result.Should().NotBeNull();
            result!.Username.Should().Be("johndoe");
            result.Email.Should().Be("john@example.com");
        }

        [Fact]
        public async Task GetByEmailOrUsernameAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetByEmailOrUsernameAsync("nonexistent@example.com");
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmailOrUsernameAsync_ShouldReturnNull_WhenDeleted()
        {
            var result = await _repository.GetByEmailOrUsernameAsync("jane@example.com");
            result.Should().BeNull();
        }
    }
}