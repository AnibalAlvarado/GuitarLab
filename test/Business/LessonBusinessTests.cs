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
    public class LessonBusinessTests
    {
        private readonly Mock<ILessonData> _lessonDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly LessonBusiness _business;

        public LessonBusinessTests()
        {
            _lessonDataMock = new Mock<ILessonData>();
            _mapperMock = new Mock<IMapper>();
            _business = new LessonBusiness(_lessonDataMock.Object, _mapperMock.Object);
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnMappedLessonDtos()
        {
            // Arrange
            var lessons = new List<Lesson>
            {
                new Lesson { Id = 1, Name = "Basic Chords", Description = "Learn basic guitar chords", IsDeleted = false },
                new Lesson { Id = 2, Name = "Advanced Scales", Description = "Master advanced scales", IsDeleted = false }
            };

            var lessonDtos = new List<LessonDto>
            {
                new LessonDto { Id = 1, Name = "Basic Chords", Description = "Learn basic guitar chords" },
                new LessonDto { Id = 2, Name = "Advanced Scales", Description = "Master advanced scales",  }
            };

            _lessonDataMock.Setup(d => d.GetAll()).ReturnsAsync(lessons);
            _mapperMock.Setup(m => m.Map<List<LessonDto>>(lessons)).Returns(lessonDtos);

            // Act
            var result = await _business.GetAll();

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
            result.First().Name.Should().Be("Basic Chords");
            result.Last().Name.Should().Be("Advanced Scales");
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnMappedLessonDto_WhenExists()
        {
            // Arrange
            var lesson = new Lesson { Id = 1, Name = "Basic Chords", Description = "Learn basic guitar chords", IsDeleted = false };
            var lessonDto = new LessonDto { Id = 1, Name = "Basic Chords", Description = "Learn basic guitar chords" };

            _lessonDataMock.Setup(d => d.GetById(1)).ReturnsAsync(lesson);
            _mapperMock.Setup(m => m.Map<LessonDto>(lesson)).Returns(lessonDto);

            // Act
            var result = await _business.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Basic Chords");
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
        public async Task GetById_ShouldThrowEntityNotFoundException_WhenLessonDoesNotExist()
        {
            // Arrange
            _lessonDataMock.Setup(d => d.GetById(999)).ReturnsAsync((Lesson)null!);

            // Act
            Func<Task> act = async () => await _business.GetById(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*Lesson*999*");
        }

        // ========================================================
        // TEST 3: Save
        // ========================================================
        [Fact]
        public async Task Save_ShouldMapAndSaveLessonDto()
        {
            // Arrange
            var lessonDto = new LessonDto { Name = "New Lesson", Description = "A new lesson"};
            var lesson = new Lesson { Id = 0, Name = "New Lesson", Description = "A new lesson", IsDeleted = false };
            var savedLesson = new Lesson { Id = 1, Name = "New Lesson", Description = "A new lesson", IsDeleted = false };
            var savedLessonDto = new LessonDto { Id = 1, Name = "New Lesson", Description = "A new lesson" };

            _mapperMock.Setup(m => m.Map<Lesson>(lessonDto)).Returns(lesson);
            _lessonDataMock.Setup(d => d.Save(lesson)).ReturnsAsync(savedLesson);
            _mapperMock.Setup(m => m.Map<LessonDto>(savedLesson)).Returns(savedLessonDto);

            // Act
            var result = await _business.Save(lessonDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("New Lesson");
        }

        [Fact]
        public async Task Save_ShouldThrowBusinessException_WhenSaveFails()
        {
            // Arrange
            var lessonDto = new LessonDto { Name = "New Lesson" };
            var lesson = new Lesson { Name = "New Lesson" };

            _mapperMock.Setup(m => m.Map<Lesson>(lessonDto)).Returns(lesson);
            _lessonDataMock.Setup(d => d.Save(lesson)).ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _business.Save(lessonDto);

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
            var lessonDto = new LessonDto { Id = 1, Name = "Updated Lesson" };
            var lesson = new Lesson { Id = 1, Name = "Updated Lesson" };

            _mapperMock.Setup(m => m.Map<Lesson>(lessonDto)).Returns(lesson);
            _lessonDataMock.Setup(d => d.Update(lesson)).ThrowsAsync(new Exception("Update error"));

            // Act
            Func<Task> act = async () => await _business.Update(lessonDto);

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
            _lessonDataMock.Setup(d => d.Delete(1)).ReturnsAsync(1);

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
            _lessonDataMock.Setup(d => d.PermanentDelete(1)).ReturnsAsync(true);

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
        public async Task PermanentDelete_ShouldThrowEntityNotFoundException_WhenLessonDoesNotExist()
        {
            // Arrange
            _lessonDataMock.Setup(d => d.PermanentDelete(999)).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _business.PermanentDelete(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*Lesson*999*");
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenLessonExists()
        {
            // Arrange
            _lessonDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Lesson, bool>>>())).ReturnsAsync(true);

            // Act
            var result = await _business.ExistsAsync(e => e.Name == "Test");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldThrowBusinessException_WhenExistsFails()
        {
            // Arrange
            _lessonDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Lesson, bool>>>())).ThrowsAsync(new Exception("Exists error"));

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
            _lessonDataMock.Setup(d => d.GetAllDynamicAsync()).ReturnsAsync(dynamicObjects);

            // Act
            var result = await _business.GetAllDynamicAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dynamicObjects);
        }
    }
}