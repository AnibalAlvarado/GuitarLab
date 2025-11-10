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
    public class TechniqueDataTests
    {
        private readonly ApplicationDbContext _context;
        private readonly TechniqueData _repository;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public TechniqueDataTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _configurationMock = new Mock<IConfiguration>();
            _auditServiceMock = new Mock<IAuditService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _repository = new TechniqueData(_context, _configurationMock.Object, _auditServiceMock.Object, _currentUserServiceMock.Object);

            // ================
            // Datos base
            // ================
            var technique1 = new Technique
            {
                Id = 1,
                Name = "Fingerpicking",
                Description = "Technique for playing with fingers",
                IsDeleted = false
            };

            var technique2 = new Technique
            {
                Id = 2,
                Name = "Strumming",
                Description = "Technique for strumming chords",
                IsDeleted = true
            };

            _context.Techniques.AddRange(technique1, technique2);
            _context.SaveChanges();
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnOnlyActiveTechniques()
        {
            var result = await _repository.GetAll();

            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Fingerpicking");
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnTechnique_WhenExistsAndActive()
        {
            var result = await _repository.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Fingerpicking");
            result.Description.Should().Be("Technique for playing with fingers");
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
        public async Task Save_ShouldAddNewTechnique()
        {
            var newTechnique = new Technique
            {
                Name = "New Technique",
                Description = "A new guitar technique"
            };

            var result = await _repository.Save(newTechnique);

            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("New Technique");

            var saved = await _context.Techniques.FindAsync(result.Id);
            saved.Should().NotBeNull();
        }


        // ========================================================
        // TEST 5: Delete
        // ========================================================
        [Fact]
        public async Task Delete_ShouldMarkTechniqueAsDeleted()
        {
            var result = await _repository.Delete(1);

            result.Should().Be(1); // Rows affected

            var deleted = await _context.Techniques.FindAsync(1);
            deleted!.IsDeleted.Should().BeTrue();
        }

        // ========================================================
        // TEST 6: PermanentDelete
        // ========================================================
        [Fact]
        public async Task PermanentDelete_ShouldRemoveTechniqueFromDatabase()
        {
            var result = await _repository.PermanentDelete(2);

            result.Should().BeTrue();

            var deleted = await _context.Techniques.FindAsync(2);
            deleted.Should().BeNull();
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenTechniqueExists()
        {
            var result = await _repository.ExistsAsync(t => t.Name == "Fingerpicking");
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenTechniqueDoesNotExist()
        {
            var result = await _repository.ExistsAsync(t => t.Name == "Nonexistent");
            result.Should().BeFalse();
        }
    }
}