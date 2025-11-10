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
    public class LessonDataTests
    {
        private readonly ApplicationDbContext _context;
        private readonly LessonData _repository;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public LessonDataTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _configurationMock = new Mock<IConfiguration>();
            _auditServiceMock = new Mock<IAuditService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _repository = new LessonData(_context, _configurationMock.Object, _auditServiceMock.Object, _currentUserServiceMock.Object);

            // ================
            // Datos base
            // ================
            var technique1 = new Technique { Id = 1, Name = "Fingerpicking", Description = "Technique for fingerpicking", IsDeleted = false };
            var technique2 = new Technique { Id = 2, Name = "Strumming", Description = "Technique for strumming", IsDeleted = false };

            var lesson1 = new Lesson
            {
                Id = 1,
                Name = "Basic Fingerpicking",
                Description = "Learn basic fingerpicking patterns",
                TechniqueId = 1,
                Technique = technique1,
                IsDeleted = false
            };

            var lesson2 = new Lesson
            {
                Id = 2,
                Name = "Advanced Strumming",
                Description = "Advanced strumming techniques",
                TechniqueId = 2,
                Technique = technique2,
                IsDeleted = true
            };

            _context.Techniques.AddRange(technique1, technique2);
            _context.Lessons.AddRange(lesson1, lesson2);
            _context.SaveChanges();
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnOnlyActiveLessons()
        {
            var result = await _repository.GetAll();

            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Basic Fingerpicking");
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnLesson_WhenExistsAndActive()
        {
            var result = await _repository.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Basic Fingerpicking");
            result.Description.Should().Be("Learn basic fingerpicking patterns");
            result.Technique.Should().NotBeNull();
            result.Technique.Name.Should().Be("Fingerpicking");
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
        public async Task Save_ShouldAddNewLesson()
        {
            var newLesson = new Lesson
            {
                Name = "New Lesson",
                Description = "A new lesson description",
                TechniqueId = 1
            };

            var result = await _repository.Save(newLesson);

            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("New Lesson");

            var saved = await _context.Lessons.FindAsync(result.Id);
            saved.Should().NotBeNull();
        }

        // ========================================================
        // TEST 4: Update
        // ========================================================
        [Fact]
        public async Task Update_ShouldModifyExistingLesson()
        {
            var lesson = await _context.Lessons.FindAsync(1);
            lesson!.Description = "Updated description";

            await _repository.Update(lesson);

            var updated = await _context.Lessons.FindAsync(1);
            updated!.Description.Should().Be("Updated description");
        }

        // ========================================================
        // TEST 5: Delete
        // ========================================================
        [Fact]
        public async Task Delete_ShouldMarkLessonAsDeleted()
        {
            var result = await _repository.Delete(1);

            result.Should().Be(1); // Rows affected

            var deleted = await _context.Lessons.FindAsync(1);
            deleted!.IsDeleted.Should().BeTrue();
        }

        // ========================================================
        // TEST 6: PermanentDelete
        // ========================================================
        [Fact]
        public async Task PermanentDelete_ShouldRemoveLessonFromDatabase()
        {
            var result = await _repository.PermanentDelete(2);

            result.Should().BeTrue();

            var deleted = await _context.Lessons.FindAsync(2);
            deleted.Should().BeNull();
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenLessonExists()
        {
            var result = await _repository.ExistsAsync(l => l.Name == "Basic Fingerpicking");
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenLessonDoesNotExist()
        {
            var result = await _repository.ExistsAsync(l => l.Name == "Nonexistent");
            result.Should().BeFalse();
        }
    }
}