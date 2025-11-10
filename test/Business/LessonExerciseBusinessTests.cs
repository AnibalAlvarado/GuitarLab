using AutoMapper;
using Business.Implementations;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Models;
using FluentAssertions;
using Moq;
using Utilities.Exceptions;

namespace test.Business
{
    public class LessonExerciseBusinessTests
    {
        private readonly Mock<ILessonExerciseData> _lessonExerciseDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly LessonExerciseBusiness _business;

        public LessonExerciseBusinessTests()
        {
            _lessonExerciseDataMock = new Mock<ILessonExerciseData>();
            _mapperMock = new Mock<IMapper>();
            _business = new LessonExerciseBusiness(_lessonExerciseDataMock.Object, _mapperMock.Object);
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnMappedLessonExerciseDtos()
        {
            // Arrange
            var lessonExercises = new List<LessonExercise>
            {
                new LessonExercise { Id = 1, LessonId = 1, ExerciseId = 1,  IsDeleted = false },
                new LessonExercise { Id = 2, LessonId = 1, ExerciseId = 2,  IsDeleted = false }
            };

            var lessonExerciseDtos = new List<LessonExerciseDto>
            {
                new LessonExerciseDto { Id = 1, LessonId = 1, ExerciseId = 1,},
                new LessonExerciseDto { Id = 2, LessonId = 1, ExerciseId = 2, }
            };

            _lessonExerciseDataMock.Setup(d => d.GetAll()).ReturnsAsync(lessonExercises);
            _mapperMock.Setup(m => m.Map<List<LessonExerciseDto>>(lessonExercises)).Returns(lessonExerciseDtos);

            // Act
            var result = await _business.GetAll();

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnMappedLessonExerciseDto_WhenExists()
        {
            // Arrange
            var lessonExercise = new LessonExercise { Id = 1, LessonId = 1, ExerciseId = 1,  IsDeleted = false };
            var lessonExerciseDto = new LessonExerciseDto { Id = 1, LessonId = 1, ExerciseId = 1 };

            _lessonExerciseDataMock.Setup(d => d.GetById(1)).ReturnsAsync(lessonExercise);
            _mapperMock.Setup(m => m.Map<LessonExerciseDto>(lessonExercise)).Returns(lessonExerciseDto);

            // Act
            var result = await _business.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result.LessonId.Should().Be(1);
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
        public async Task GetById_ShouldThrowEntityNotFoundException_WhenLessonExerciseDoesNotExist()
        {
            // Arrange
            _lessonExerciseDataMock.Setup(d => d.GetById(999)).ReturnsAsync((LessonExercise)null!);

            // Act
            Func<Task> act = async () => await _business.GetById(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*LessonExercise*999*");
        }

        // ========================================================
        // TEST 3: Save
        // ========================================================
        [Fact]
        public async Task Save_ShouldMapAndSaveLessonExerciseDto()
        {
            // Arrange
            var lessonExerciseDto = new LessonExerciseDto { LessonId = 1, ExerciseId = 1 };
            var lessonExercise = new LessonExercise { Id = 0, LessonId = 1, ExerciseId = 1,  IsDeleted = false };
            var savedLessonExercise = new LessonExercise { Id = 1, LessonId = 1, ExerciseId = 1,  IsDeleted = false };
            var savedLessonExerciseDto = new LessonExerciseDto { Id = 1, LessonId = 1, ExerciseId = 1};

            _mapperMock.Setup(m => m.Map<LessonExercise>(lessonExerciseDto)).Returns(lessonExercise);
            _lessonExerciseDataMock.Setup(d => d.Save(lessonExercise)).ReturnsAsync(savedLessonExercise);
            _mapperMock.Setup(m => m.Map<LessonExerciseDto>(savedLessonExercise)).Returns(savedLessonExerciseDto);

            // Act
            var result = await _business.Save(lessonExerciseDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            
        }

        [Fact]
        public async Task Save_ShouldThrowBusinessException_WhenSaveFails()
        {
            // Arrange
            var lessonExerciseDto = new LessonExerciseDto { LessonId = 1, ExerciseId = 1 };
            var lessonExercise = new LessonExercise { LessonId = 1, ExerciseId = 1 };

            _mapperMock.Setup(m => m.Map<LessonExercise>(lessonExerciseDto)).Returns(lessonExercise);
            _lessonExerciseDataMock.Setup(d => d.Save(lessonExercise)).ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _business.Save(lessonExerciseDto);

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
            var lessonExerciseDto = new LessonExerciseDto { Id = 1, };
            var lessonExercise = new LessonExercise { Id = 1, };

            _mapperMock.Setup(m => m.Map<LessonExercise>(lessonExerciseDto)).Returns(lessonExercise);
            _lessonExerciseDataMock.Setup(d => d.Update(lessonExercise)).ThrowsAsync(new Exception("Update error"));

            // Act
            Func<Task> act = async () => await _business.Update(lessonExerciseDto);

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
            _lessonExerciseDataMock.Setup(d => d.Delete(1)).ReturnsAsync(1);

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
            _lessonExerciseDataMock.Setup(d => d.PermanentDelete(1)).ReturnsAsync(true);

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
        public async Task PermanentDelete_ShouldThrowEntityNotFoundException_WhenLessonExerciseDoesNotExist()
        {
            // Arrange
            _lessonExerciseDataMock.Setup(d => d.PermanentDelete(999)).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _business.PermanentDelete(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*LessonExercise*999*");
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenLessonExerciseExists()
        {
            // Arrange
            _lessonExerciseDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<LessonExercise, bool>>>())).ReturnsAsync(true);

            // Act
            var result = await _business.ExistsAsync(e => e.Id == 1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldThrowBusinessException_WhenExistsFails()
        {
            // Arrange
            _lessonExerciseDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<LessonExercise, bool>>>())).ThrowsAsync(new Exception("Exists error"));

            // Act
            Func<Task> act = async () => await _business.ExistsAsync(e => e.Id == 1);

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
            _lessonExerciseDataMock.Setup(d => d.GetAllDynamicAsync()).ReturnsAsync(dynamicObjects);

            // Act
            var result = await _business.GetAllDynamicAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dynamicObjects);
        }

        // ========================================================
        // TEST 9: GetAllJoinAsync
        // ========================================================
        [Fact]
        public async Task GetAllJoinAsync_ShouldReturnMappedLessonExerciseDtos()
        {
            // Arrange
            var lessonExercises = new List<LessonExercise>
            {
                new LessonExercise { Id = 1, LessonId = 1, ExerciseId = 1,  IsDeleted = false },
                new LessonExercise { Id = 2, LessonId = 1, ExerciseId = 2,  IsDeleted = false }
            };

            var lessonExerciseDtos = new List<LessonExerciseDto>
            {
                new LessonExerciseDto { Id = 1, LessonId = 1, ExerciseId = 1 },
                new LessonExerciseDto { Id = 2, LessonId = 1, ExerciseId = 2 }
            };

            _lessonExerciseDataMock.Setup(d => d.GetAllJoinAsync()).ReturnsAsync(lessonExercises);
            _mapperMock.Setup(m => m.Map<IEnumerable<LessonExerciseDto>>(lessonExercises)).Returns(lessonExerciseDtos);

            // Act
            var result = await _business.GetAllJoinAsync();

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
        }
    }
}