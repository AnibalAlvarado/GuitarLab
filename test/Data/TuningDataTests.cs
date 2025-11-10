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
    public class TuningDataTests
    {
        private readonly ApplicationDbContext _context;
        private readonly TuningData _repository;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public TuningDataTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _configurationMock = new Mock<IConfiguration>();
            _auditServiceMock = new Mock<IAuditService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _repository = new TuningData(_context, _configurationMock.Object, _auditServiceMock.Object, _currentUserServiceMock.Object);

            // ================
            // Datos base
            // ================
            var tuning1 = new Tuning
            {
                Id = 1,
                Name = "Standard",
                Notes = "EADGBE",
                IsDeleted = false
            };

            var tuning2 = new Tuning
            {
                Id = 2,
                Name = "Drop D",
                Notes = "DADGBE",
                IsDeleted = true
            };

            _context.Tunings.AddRange(tuning1, tuning2);
            _context.SaveChanges();
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnOnlyActiveTunings()
        {
            var result = await _repository.GetAll();

            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Standard");
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnTuning_WhenExistsAndActive()
        {
            var result = await _repository.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Standard");
            result.Notes.Should().Be("EADGBE");
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
        public async Task Save_ShouldAddNewTuning()
        {
            var newTuning = new Tuning
            {
                Name = "Open G",
                Notes = "DGDGBD"
            };

            var result = await _repository.Save(newTuning);

            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Open G");

            var saved = await _context.Tunings.FindAsync(result.Id);
            saved.Should().NotBeNull();
        }

        // ========================================================
        // TEST 5: Delete
        // ========================================================
        [Fact]
        public async Task Delete_ShouldMarkTuningAsDeleted()
        {
            var result = await _repository.Delete(1);

            result.Should().Be(1); // Rows affected

            var deleted = await _context.Tunings.FindAsync(1);
            deleted!.IsDeleted.Should().BeTrue();
        }

        // ========================================================
        // TEST 6: PermanentDelete
        // ========================================================
        [Fact]
        public async Task PermanentDelete_ShouldRemoveTuningFromDatabase()
        {
            var result = await _repository.PermanentDelete(2);

            result.Should().BeTrue();

            var deleted = await _context.Tunings.FindAsync(2);
            deleted.Should().BeNull();
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenTuningExists()
        {
            var result = await _repository.ExistsAsync(t => t.Name == "Standard");
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenTuningDoesNotExist()
        {
            var result = await _repository.ExistsAsync(t => t.Name == "Nonexistent");
            result.Should().BeFalse();
        }
    }
}