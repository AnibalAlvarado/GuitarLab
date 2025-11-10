using AutoMapper;
using Business.Implementations;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Enums;
using Entity.Models;
using FluentAssertions;
using Moq;
using Utilities.Exceptions;

namespace test.Business
{
    public class ExerciseBusinessTests
    {
        private readonly Mock<IExerciseData> _exerciseDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ExerciseBusiness _business;

        public ExerciseBusinessTests()
        {
            _exerciseDataMock = new Mock<IExerciseData>();
            _mapperMock = new Mock<IMapper>();
            _business = new ExerciseBusiness(_exerciseDataMock.Object, _mapperMock.Object);
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnMappedExerciseDtos()
        {
            // Arrange
            var exercises = new List<Exercise>
            {
                new Exercise { Id = 1, Name = "Basic Chord", Difficulty = Difficulty.Easy, BPM = 120, IsDeleted = false },
                new Exercise { Id = 2, Name = "Advanced Scale", Difficulty = Difficulty.Hard, BPM = 180, IsDeleted = false }
            };

            var exerciseDtos = new List<ExerciseDto>
            {
                new ExerciseDto { Id = 1, Name = "Basic Chord", Difficulty = Difficulty.Easy, BPM = 120 },
                new ExerciseDto { Id = 2, Name = "Advanced Scale", Difficulty = Difficulty.Hard, BPM = 180 }
            };

            _exerciseDataMock.Setup(d => d.GetAll()).ReturnsAsync(exercises);
            _mapperMock.Setup(m => m.Map<List<ExerciseDto>>(exercises)).Returns(exerciseDtos);

            // Act
            var result = await _business.GetAll();

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
            result.First().Name.Should().Be("Basic Chord");
            result.Last().Name.Should().Be("Advanced Scale");
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnMappedExerciseDto_WhenExists()
        {
            // Arrange
            var exercise = new Exercise { Id = 1, Name = "Basic Chord", Difficulty = Difficulty.Easy, BPM = 120, IsDeleted = false };
            var exerciseDto = new ExerciseDto { Id = 1, Name = "Basic Chord", Difficulty = Difficulty.Easy, BPM = 120 };

            _exerciseDataMock.Setup(d => d.GetById(1)).ReturnsAsync(exercise);
            _mapperMock.Setup(m => m.Map<ExerciseDto>(exercise)).Returns(exerciseDto);

            // Act
            var result = await _business.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Basic Chord");
            result.BPM.Should().Be(120);
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
        public async Task GetById_ShouldThrowEntityNotFoundException_WhenExerciseDoesNotExist()
        {
            // Arrange
            _exerciseDataMock.Setup(d => d.GetById(999)).ReturnsAsync((Exercise)null!);

            // Act
            Func<Task> act = async () => await _business.GetById(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*Exercise*999*");
        }

        // ========================================================
        // TEST 3: Save
        // ========================================================
        [Fact]
        public async Task Save_ShouldMapAndSaveExerciseDto()
        {
            // Arrange
            var exerciseDto = new ExerciseDto { Name = "New Exercise", Difficulty = Difficulty.Medium, BPM = 140 };
            var exercise = new Exercise { Id = 0, Name = "New Exercise", Difficulty = Difficulty.Medium, BPM = 140, IsDeleted = false };
            var savedExercise = new Exercise { Id = 1, Name = "New Exercise", Difficulty = Difficulty.Medium, BPM = 140, IsDeleted = false };
            var savedExerciseDto = new ExerciseDto { Id = 1, Name = "New Exercise", Difficulty = Difficulty.Medium, BPM = 140 };

            _mapperMock.Setup(m => m.Map<Exercise>(exerciseDto)).Returns(exercise);
            _exerciseDataMock.Setup(d => d.Save(exercise)).ReturnsAsync(savedExercise);
            _mapperMock.Setup(m => m.Map<ExerciseDto>(savedExercise)).Returns(savedExerciseDto);

            // Act
            var result = await _business.Save(exerciseDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("New Exercise");
            result.BPM.Should().Be(140);
        }

        [Fact]
        public async Task Save_ShouldThrowBusinessException_WhenSaveFails()
        {
            // Arrange
            var exerciseDto = new ExerciseDto { Name = "New Exercise" };
            var exercise = new Exercise { Name = "New Exercise" };

            _mapperMock.Setup(m => m.Map<Exercise>(exerciseDto)).Returns(exercise);
            _exerciseDataMock.Setup(d => d.Save(exercise)).ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _business.Save(exerciseDto);

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
            var exerciseDto = new ExerciseDto { Id = 1, Name = "Updated Exercise" };
            var exercise = new Exercise { Id = 1, Name = "Updated Exercise" };

            _mapperMock.Setup(m => m.Map<Exercise>(exerciseDto)).Returns(exercise);
            _exerciseDataMock.Setup(d => d.Update(exercise)).ThrowsAsync(new Exception("Update error"));

            // Act
            Func<Task> act = async () => await _business.Update(exerciseDto);

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
            _exerciseDataMock.Setup(d => d.Delete(1)).ReturnsAsync(1);

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
            _exerciseDataMock.Setup(d => d.PermanentDelete(1)).ReturnsAsync(true);

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
        public async Task PermanentDelete_ShouldThrowEntityNotFoundException_WhenExerciseDoesNotExist()
        {
            // Arrange
            _exerciseDataMock.Setup(d => d.PermanentDelete(999)).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _business.PermanentDelete(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*Exercise*999*");
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenExerciseExists()
        {
            // Arrange
            _exerciseDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Exercise, bool>>>())).ReturnsAsync(true);

            // Act
            var result = await _business.ExistsAsync(e => e.Name == "Test");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldThrowBusinessException_WhenExistsFails()
        {
            // Arrange
            _exerciseDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Exercise, bool>>>())).ThrowsAsync(new Exception("Exists error"));

            // Act
            Func<Task> act = async () => await _business.ExistsAsync(e => e.Name == "Test");

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
            _exerciseDataMock.Setup(d => d.GetAllDynamicAsync()).ReturnsAsync(dynamicObjects);

            // Act
            var result = await _business.GetAllDynamicAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dynamicObjects);
        }
    }
}