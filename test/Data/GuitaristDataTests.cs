using Data.Implementations;
using Entity.Contexts;
using Entity.Enums;
using Entity.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace test.Data
{
    public class GuitaristDataTests
    {
        private readonly ApplicationDbContext _context;
        private readonly GuitaristData _repository;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<GuitaristData>> _loggerMock;
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public GuitaristDataTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<GuitaristData>>();
            _auditServiceMock = new Mock<IAuditService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _repository = new GuitaristData(_context, _configurationMock.Object, _loggerMock.Object, _auditServiceMock.Object, _currentUserServiceMock.Object);

            // ================
            // Datos base
            // ================
            var guitarist1 = new Guitarist
            {
                Id = 1,
                Name = "John Doe",
                SkillLevel = SkillLevel.Intermediate,
                ExperienceYears = 5,
                IsDeleted = false
            };

            var guitarist2 = new Guitarist
            {
                Id = 2,
                Name = "Jane Smith",
                SkillLevel = SkillLevel.Beginner,
                ExperienceYears = 1,
                IsDeleted = true
            };

            var guitarist3 = new Guitarist
            {
                Id = 3,
                Name = "Bob Johnson",
                SkillLevel = SkillLevel.Advanced,
                ExperienceYears = 10,
                IsDeleted = false
            };

            _context.Guitarists.AddRange(guitarist1, guitarist2, guitarist3);
            _context.SaveChanges();
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnOnlyActiveGuitarists()
        {
            var result = await _repository.GetAll();

            result.Should().HaveCount(2);
            result.All(g => !g.IsDeleted).Should().BeTrue();
            result.Select(g => g.Name).Should().Contain(new[] { "John Doe", "Bob Johnson" });
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnGuitarist_WhenExistsAndActive()
        {
            var result = await _repository.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("John Doe");
            result.SkillLevel.Should().Be(SkillLevel.Intermediate);
            result.ExperienceYears.Should().Be(5);
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
        public async Task Save_ShouldAddNewGuitarist()
        {
            var newGuitarist = new Guitarist
            {
                Name = "New Guitarist",
                SkillLevel = SkillLevel.Beginner,
                ExperienceYears = 0
            };

            var result = await _repository.Save(newGuitarist);

            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("New Guitarist");

            var saved = await _context.Guitarists.FindAsync(result.Id);
            saved.Should().NotBeNull();
        }

        // ========================================================
        // TEST 4: Update
        // ========================================================
        [Fact]
        public async Task Update_ShouldModifyExistingGuitarist()
        {
            var guitarist = await _context.Guitarists.FindAsync(1);
            guitarist!.ExperienceYears = 6;

            await _repository.Update(guitarist);

            var updated = await _context.Guitarists.FindAsync(1);
            updated!.ExperienceYears.Should().Be(6);
        }

        // ========================================================
        // TEST 5: Delete
        // ========================================================
        [Fact]
        public async Task Delete_ShouldMarkGuitaristAsDeleted()
        {
            var result = await _repository.Delete(1);

            result.Should().Be(1); // Rows affected

            var deleted = await _context.Guitarists.FindAsync(1);
            deleted!.IsDeleted.Should().BeTrue();
        }

        // ========================================================
        // TEST 6: PermanentDelete
        // ========================================================
        [Fact]
        public async Task PermanentDelete_ShouldRemoveGuitaristFromDatabase()
        {
            var result = await _repository.PermanentDelete(2);

            result.Should().BeTrue();

            var deleted = await _context.Guitarists.FindAsync(2);
            deleted.Should().BeNull();
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenGuitaristExists()
        {
            var result = await _repository.ExistsAsync(g => g.Name == "John Doe");
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenGuitaristDoesNotExist()
        {
            var result = await _repository.ExistsAsync(g => g.Name == "Nonexistent");
            result.Should().BeFalse();
        }

        // ========================================================
        // TEST 8: GetGuitaristByGuitaristnameAsync
        // ========================================================
        [Fact]
        public async Task GetGuitaristByGuitaristnameAsync_ShouldReturnGuitarist_WhenExistsAndActive()
        {
            var result = await _repository.GetGuitaristByGuitaristnameAsync("John Doe");

            result.Should().NotBeNull();
            result!.Name.Should().Be("John Doe");
            result.SkillLevel.Should().Be(SkillLevel.Intermediate);
        }

        [Fact]
        public async Task GetGuitaristByGuitaristnameAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetGuitaristByGuitaristnameAsync("Nonexistent");
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetGuitaristByGuitaristnameAsync_ShouldReturnNull_WhenDeleted()
        {
            var result = await _repository.GetGuitaristByGuitaristnameAsync("Jane Smith");
            result.Should().BeNull();
        }
    }
}