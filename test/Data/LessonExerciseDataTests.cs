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
    public class LessonExerciseDataTests
    {
        private readonly ApplicationDbContext _context;
        private readonly LessonExerciseData _repository;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IAuditService> _auditServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public LessonExerciseDataTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _configurationMock = new Mock<IConfiguration>();
            _auditServiceMock = new Mock<IAuditService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _repository = new LessonExerciseData(_context, _configurationMock.Object, _auditServiceMock.Object, _currentUserServiceMock.Object);

            // ================
            // Datos base
            // ================
            var technique = new Technique { Id = 1, Name = "Fingerpicking", IsDeleted = false };
            var tuning = new Tuning { Id = 1, Name = "Standard", Notes = "EADGBE", IsDeleted = false };

            var lesson = new Lesson { Id = 1, Name = "Basic Lesson", TechniqueId = 1, Technique = technique, IsDeleted = false };
            var exercise = new Exercise
            {
                Id = 1,
                Name = "Basic Chord",
                Difficulty = Difficulty.Easy,
                BPM = 120,
                TabNotation = "e|-0-",
                TuningId = 1,
                Tuning = tuning,
                IsDeleted = false
            };

            var lessonExercise1 = new LessonExercise
            {
                Id = 1,
                LessonId = 1,
                ExerciseId = 1,
                Lesson = lesson,
                Exercise = exercise,
                IsDeleted = false
            };

            var lessonExercise2 = new LessonExercise
            {
                Id = 2,
                LessonId = 1,
                ExerciseId = 1,
                IsDeleted = true
            };

            _context.Techniques.Add(technique);
            _context.Tunings.Add(tuning);
            _context.Lessons.Add(lesson);
            _context.Exercises.Add(exercise);
            _context.LessonExercises.AddRange(lessonExercise1, lessonExercise2);
            _context.SaveChanges();
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnOnlyActiveLessonExercises()
        {
            var result = await _repository.GetAll();

            result.Should().HaveCount(1);
            result.First().LessonId.Should().Be(1);
            result.First().ExerciseId.Should().Be(1);
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnLessonExercise_WhenExistsAndActive()
        {
            var result = await _repository.GetById(1);

            result.Should().NotBeNull();
            result!.LessonId.Should().Be(1);
            result.ExerciseId.Should().Be(1);
            result.Lesson.Should().NotBeNull();
            result.Exercise.Should().NotBeNull();
            result.Lesson.Name.Should().Be("Basic Lesson");
            result.Exercise.Name.Should().Be("Basic Chord");
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
        public async Task Save_ShouldAddNewLessonExercise()
        {
            var newLessonExercise = new LessonExercise
            {
                LessonId = 1,
                ExerciseId = 1
            };

            var result = await _repository.Save(newLessonExercise);

            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.LessonId.Should().Be(1);
            result.ExerciseId.Should().Be(1);

            var saved = await _context.LessonExercises.FindAsync(result.Id);
            saved.Should().NotBeNull();
        }


        // ========================================================
        // TEST 5: Delete
        // ========================================================
        [Fact]
        public async Task Delete_ShouldMarkLessonExerciseAsDeleted()
        {
            var result = await _repository.Delete(1);

            result.Should().Be(1); // Rows affected

            var deleted = await _context.LessonExercises.FindAsync(1);
            deleted!.IsDeleted.Should().BeTrue();
        }

        // ========================================================
        // TEST 6: PermanentDelete
        // ========================================================
        [Fact]
        public async Task PermanentDelete_ShouldRemoveLessonExerciseFromDatabase()
        {
            var result = await _repository.PermanentDelete(2);

            result.Should().BeTrue();

            var deleted = await _context.LessonExercises.FindAsync(2);
            deleted.Should().BeNull();
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenLessonExerciseExists()
        {
            var result = await _repository.ExistsAsync(le => le.LessonId == 1 && le.ExerciseId == 1);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenLessonExerciseDoesNotExist()
        {
            var result = await _repository.ExistsAsync(le => le.LessonId == 999 && le.ExerciseId == 999);
            result.Should().BeFalse();
        }

        // ========================================================
        // TEST 8: GetAllJoinAsync
        // ========================================================
        [Fact]
        public async Task GetAllJoinAsync_ShouldReturnAllLessonExercisesWithIncludes()
        {
            var result = await _repository.GetAllJoinAsync();

            result.Should().HaveCount(2); // Includes deleted ones since it's a custom method
            var activeLessonExercise = result.First(le => !le.IsDeleted);
            activeLessonExercise.Lesson.Should().NotBeNull();
            activeLessonExercise.Exercise.Should().NotBeNull();
            activeLessonExercise.Lesson.Name.Should().Be("Basic Lesson");
            activeLessonExercise.Exercise.Name.Should().Be("Basic Chord");
        }
    }
}