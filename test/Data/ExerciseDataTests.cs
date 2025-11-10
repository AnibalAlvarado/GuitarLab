using Data.Implementations;
using Entity.Contexts;
using Entity.Enums;
using Entity.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace test.Data
{
    public class ExerciseDataTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ExerciseData _repository;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public ExerciseDataTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _configurationMock = new Mock<IConfiguration>();
            _auditServiceMock = new Mock<IAuditService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _repository = new ExerciseData(_context, _configurationMock.Object, _auditServiceMock.Object, _currentUserServiceMock.Object);

            // ================
            // Datos base
            // ================
            var tuning1 = new Tuning { Id = 1, Name = "Standard", Notes = "EADGBE", IsDeleted = false };
            var tuning2 = new Tuning { Id = 2, Name = "Drop D", Notes = "DADGBE", IsDeleted = false };

            var exercise1 = new Exercise
            {
                Id = 1,
                Name = "Basic Chord",
                Difficulty = Difficulty.Easy,
                BPM = 120,
                TabNotation = "e|-0-",
                TuningId = 1,
                Tuning = tuning1,
                IsDeleted = false
            };

            var exercise2 = new Exercise
            {
                Id = 2,
                Name = "Advanced Scale",
                Difficulty = Difficulty.Hard,
                BPM = 180,
                TabNotation = "e|-----",
                TuningId = 2,
                Tuning = tuning2,
                IsDeleted = true
            };

            _context.Tunings.AddRange(tuning1, tuning2);
            _context.Exercises.AddRange(exercise1, exercise2);
            _context.SaveChanges();
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnOnlyActiveExercises()
        {
            var result = await _repository.GetAll();

            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Basic Chord");
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnExercise_WhenExistsAndActive()
        {
            var result = await _repository.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Basic Chord");
            result.Tuning.Should().NotBeNull();
            result.Tuning.Name.Should().Be("Standard");
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
        public async Task Save_ShouldAddNewExercise()
        {
            var newExercise = new Exercise
            {
                Name = "New Exercise",
                Difficulty = Difficulty.Medium,
                BPM = 140,
                TabNotation = "e|-1-",
                TuningId = 1
            };

            var result = await _repository.Save(newExercise);

            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("New Exercise");

            var saved = await _context.Exercises.FindAsync(result.Id);
            saved.Should().NotBeNull();
        }


        // ========================================================
        // TEST 5: Delete
        // ========================================================
        [Fact]
        public async Task Delete_ShouldMarkExerciseAsDeleted()
        {
            var result = await _repository.Delete(1);

            result.Should().Be(1); // Rows affected

            var deleted = await _context.Exercises.FindAsync(1);
            deleted!.IsDeleted.Should().BeTrue();
        }

        // ========================================================
        // TEST 6: PermanentDelete
        // ========================================================
        [Fact]
        public async Task PermanentDelete_ShouldRemoveExerciseFromDatabase()
        {
            var result = await _repository.PermanentDelete(2);

            result.Should().BeTrue();

            var deleted = await _context.Exercises.FindAsync(2);
            deleted.Should().BeNull();
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenExerciseExists()
        {
            var result = await _repository.ExistsAsync(e => e.Name == "Basic Chord");
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenExerciseDoesNotExist()
        {
            var result = await _repository.ExistsAsync(e => e.Name == "Nonexistent");
            result.Should().BeFalse();
        }
    }
}