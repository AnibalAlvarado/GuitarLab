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
    public class GuitaristLessonDataTests
    {
        private readonly ApplicationDbContext _context;
        private readonly GuitaristLessonData _repository;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<GuitaristLessonData>> _loggerMock;
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public GuitaristLessonDataTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<GuitaristLessonData>>();
            _auditServiceMock = new Mock<IAuditService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _repository = new GuitaristLessonData(_context, _configurationMock.Object, _loggerMock.Object, _auditServiceMock.Object, _currentUserServiceMock.Object);

            // ================
            // Datos base
            // ================
            var technique1 = new Technique { Id = 1, Name = "Fingerpicking", IsDeleted = false };
            var technique2 = new Technique { Id = 2, Name = "Strumming", IsDeleted = false };

            var lesson1 = new Lesson { Id = 1, Name = "Basic Fingerpicking", TechniqueId = 1, Technique = technique1, IsDeleted = false };
            var lesson2 = new Lesson { Id = 2, Name = "Advanced Strumming", TechniqueId = 2, Technique = technique2, IsDeleted = false };

            var guitarist1 = new Guitarist { Id = 1, Name = "John Doe", IsDeleted = false };
            var guitarist2 = new Guitarist { Id = 2, Name = "Jane Smith", IsDeleted = false };

            var guitaristLesson1 = new GuitaristLesson
            {
                Id = 1,
                GuitaristId = 1,
                LessonId = 1,
                Status = LessonStatus.InProgress,
                ProgressPercent = 50.0,
                Guitarist = guitarist1,
                Lesson = lesson1,
                IsDeleted = false
            };

            var guitaristLesson2 = new GuitaristLesson
            {
                Id = 2,
                GuitaristId = 2,
                LessonId = 2,
                Status = LessonStatus.Completed,
                ProgressPercent = 100.0,
                Guitarist = guitarist2,
                Lesson = lesson2,
                IsDeleted = true
            };

            _context.Techniques.AddRange(technique1, technique2);
            _context.Lessons.AddRange(lesson1, lesson2);
            _context.Guitarists.AddRange(guitarist1, guitarist2);
            _context.GuitaristLessons.AddRange(guitaristLesson1, guitaristLesson2);
            _context.SaveChanges();
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnOnlyActiveGuitaristLessons()
        {
            var result = await _repository.GetAll();

            result.Should().HaveCount(1);
            result.First().Status.Should().Be(LessonStatus.InProgress);
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnGuitaristLessonWithIncludes_WhenExistsAndActive()
        {
            var result = await _repository.GetById(1);

            result.Should().NotBeNull();
            result!.Guitarist.Should().NotBeNull();
            result.Lesson.Should().NotBeNull();
            result.Guitarist.Name.Should().Be("John Doe");
            result.Lesson.Name.Should().Be("Basic Fingerpicking");
            result.Status.Should().Be(LessonStatus.InProgress);
            result.ProgressPercent.Should().Be(50.0);
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
        public async Task Save_ShouldAddNewGuitaristLesson()
        {
            var newGuitaristLesson = new GuitaristLesson
            {
                GuitaristId = 1,
                LessonId = 2,
                Status = LessonStatus.NotStarted,
                ProgressPercent = 0.0
            };

            var result = await _repository.Save(newGuitaristLesson);

            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Status.Should().Be(LessonStatus.NotStarted);

            var saved = await _context.GuitaristLessons.FindAsync(result.Id);
            saved.Should().NotBeNull();
        }


        // ========================================================
        // TEST 5: Delete
        // ========================================================
        [Fact]
        public async Task Delete_ShouldMarkGuitaristLessonAsDeleted()
        {
            var result = await _repository.Delete(1);

            result.Should().Be(1); // Rows affected

            var deleted = await _context.GuitaristLessons.FindAsync(1);
            deleted!.IsDeleted.Should().BeTrue();
        }

        // ========================================================
        // TEST 6: PermanentDelete
        // ========================================================
        [Fact]
        public async Task PermanentDelete_ShouldRemoveGuitaristLessonFromDatabase()
        {
            var result = await _repository.PermanentDelete(2);

            result.Should().BeTrue();

            var deleted = await _context.GuitaristLessons.FindAsync(2);
            deleted.Should().BeNull();
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenGuitaristLessonExists()
        {
            var result = await _repository.ExistsAsync(gl => gl.GuitaristId == 1 && gl.LessonId == 1);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenGuitaristLessonDoesNotExist()
        {
            var result = await _repository.ExistsAsync(gl => gl.GuitaristId == 999 && gl.LessonId == 999);
            result.Should().BeFalse();
        }

        // ========================================================
        // TEST 8: GetAllJoinAsync
        // ========================================================
        [Fact]
        public async Task GetAllJoinAsync_ShouldReturnAllGuitaristLessonsWithIncludes()
        {
            var result = await _repository.GetAllJoinAsync();

            result.Should().HaveCount(2); // Includes deleted ones since it's a custom method
            var activeLesson = result.First(gl => !gl.IsDeleted);
            activeLesson.Guitarist.Should().NotBeNull();
            activeLesson.Lesson.Should().NotBeNull();
            activeLesson.Guitarist.Name.Should().Be("John Doe");
            activeLesson.Lesson.Name.Should().Be("Basic Fingerpicking");
        }
    }
}