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
    public class GuitaristLessonBusinessTests
    {
        private readonly Mock<IGuitaristLessonData> _guitaristLessonDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GuitaristLessonBusiness _business;

        public GuitaristLessonBusinessTests()
        {
            _guitaristLessonDataMock = new Mock<IGuitaristLessonData>();
            _mapperMock = new Mock<IMapper>();
            _business = new GuitaristLessonBusiness(_guitaristLessonDataMock.Object, _mapperMock.Object);
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnMappedGuitaristLessonDtos()
        {
            // Arrange
            var guitaristLessons = new List<GuitaristLesson>
            {
                new GuitaristLesson { Id = 1, GuitaristId = 1, LessonId = 1, Status = LessonStatus.InProgress, ProgressPercent = 50 },
                new GuitaristLesson { Id = 2, GuitaristId = 2, LessonId = 2, Status = LessonStatus.Completed, ProgressPercent = 100 }
            };

            var guitaristLessonDtos = new List<GuitaristLessonDto>
            {
                new GuitaristLessonDto { Id = 1, GuitaristId = 1, LessonId = 1, Status = LessonStatus.InProgress, ProgressPercent = 50 },
                new GuitaristLessonDto { Id = 2, GuitaristId = 2, LessonId = 2, Status = LessonStatus.Completed, ProgressPercent = 100 }
            };

            _guitaristLessonDataMock.Setup(d => d.GetAll()).ReturnsAsync(guitaristLessons);
            _mapperMock.Setup(m => m.Map<List<GuitaristLessonDto>>(guitaristLessons)).Returns(guitaristLessonDtos);

            // Act
            var result = await _business.GetAll();

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
            result.First().ProgressPercent.Should().Be(50);
            result.Last().ProgressPercent.Should().Be(100);
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnMappedGuitaristLessonDto_WhenExists()
        {
            // Arrange
            var guitaristLesson = new GuitaristLesson { Id = 1, GuitaristId = 1, LessonId = 1, Status = LessonStatus.InProgress, ProgressPercent = 50 };
            var guitaristLessonDto = new GuitaristLessonDto { Id = 1, GuitaristId = 1, LessonId = 1, Status = LessonStatus.InProgress, ProgressPercent = 50 };

            _guitaristLessonDataMock.Setup(d => d.GetById(1)).ReturnsAsync(guitaristLesson);
            _mapperMock.Setup(m => m.Map<GuitaristLessonDto>(guitaristLesson)).Returns(guitaristLessonDto);

            // Act
            var result = await _business.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result.ProgressPercent.Should().Be(50);
            result.Status.Should().Be(LessonStatus.InProgress);
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
        public async Task GetById_ShouldThrowEntityNotFoundException_WhenGuitaristLessonDoesNotExist()
        {
            // Arrange
            _guitaristLessonDataMock.Setup(d => d.GetById(999)).ReturnsAsync((GuitaristLesson)null!);

            // Act
            Func<Task> act = async () => await _business.GetById(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*GuitaristLesson*999*");
        }

        // ========================================================
        // TEST 3: Save
        // ========================================================
        [Fact]
        public async Task Save_ShouldMapAndSaveGuitaristLessonDto()
        {
            // Arrange
            var guitaristLessonDto = new GuitaristLessonDto { GuitaristId = 1, LessonId = 1, Status = LessonStatus.NotStarted, ProgressPercent = 0 };
            var guitaristLesson = new GuitaristLesson { Id = 0, GuitaristId = 1, LessonId = 1, Status = LessonStatus.NotStarted, ProgressPercent = 0, IsDeleted = false };
            var savedGuitaristLesson = new GuitaristLesson { Id = 1, GuitaristId = 1, LessonId = 1, Status = LessonStatus.NotStarted, ProgressPercent = 0, IsDeleted = false };
            var savedGuitaristLessonDto = new GuitaristLessonDto { Id = 1, GuitaristId = 1, LessonId = 1, Status = LessonStatus.NotStarted, ProgressPercent = 0 };

            _mapperMock.Setup(m => m.Map<GuitaristLesson>(guitaristLessonDto)).Returns(guitaristLesson);
            _guitaristLessonDataMock.Setup(d => d.Save(guitaristLesson)).ReturnsAsync(savedGuitaristLesson);
            _mapperMock.Setup(m => m.Map<GuitaristLessonDto>(savedGuitaristLesson)).Returns(savedGuitaristLessonDto);

            // Act
            var result = await _business.Save(guitaristLessonDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.ProgressPercent.Should().Be(0);
        }

        [Fact]
        public async Task Save_ShouldThrowBusinessException_WhenSaveFails()
        {
            // Arrange
            var guitaristLessonDto = new GuitaristLessonDto { GuitaristId = 1, LessonId = 1 };
            var guitaristLesson = new GuitaristLesson { GuitaristId = 1, LessonId = 1 };

            _mapperMock.Setup(m => m.Map<GuitaristLesson>(guitaristLessonDto)).Returns(guitaristLesson);
            _guitaristLessonDataMock.Setup(d => d.Save(guitaristLesson)).ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _business.Save(guitaristLessonDto);

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
            var guitaristLessonDto = new GuitaristLessonDto { Id = 1, ProgressPercent = 75 };
            var guitaristLesson = new GuitaristLesson { Id = 1, ProgressPercent = 75 };

            _mapperMock.Setup(m => m.Map<GuitaristLesson>(guitaristLessonDto)).Returns(guitaristLesson);
            _guitaristLessonDataMock.Setup(d => d.Update(guitaristLesson)).ThrowsAsync(new Exception("Update error"));

            // Act
            Func<Task> act = async () => await _business.Update(guitaristLessonDto);

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
            _guitaristLessonDataMock.Setup(d => d.Delete(1)).ReturnsAsync(1);

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
            _guitaristLessonDataMock.Setup(d => d.PermanentDelete(1)).ReturnsAsync(true);

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
        public async Task PermanentDelete_ShouldThrowEntityNotFoundException_WhenGuitaristLessonDoesNotExist()
        {
            // Arrange
            _guitaristLessonDataMock.Setup(d => d.PermanentDelete(999)).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _business.PermanentDelete(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*GuitaristLesson*999*");
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenGuitaristLessonExists()
        {
            // Arrange
            _guitaristLessonDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<GuitaristLesson, bool>>>())).ReturnsAsync(true);

            // Act
            var result = await _business.ExistsAsync(e => e.Id == 1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldThrowBusinessException_WhenExistsFails()
        {
            // Arrange
            _guitaristLessonDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<GuitaristLesson, bool>>>())).ThrowsAsync(new Exception("Exists error"));

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
            _guitaristLessonDataMock.Setup(d => d.GetAllDynamicAsync()).ReturnsAsync(dynamicObjects);

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
        public async Task GetAllJoinAsync_ShouldReturnMappedGuitaristLessonDtos()
        {
            // Arrange
            var guitaristLessons = new List<GuitaristLesson>
            {
                new GuitaristLesson { Id = 1, GuitaristId = 1, LessonId = 1, Status = LessonStatus.InProgress, ProgressPercent = 50 },
                new GuitaristLesson { Id = 2, GuitaristId = 2, LessonId = 2, Status = LessonStatus.Completed, ProgressPercent = 100 }
            };

            var guitaristLessonDtos = new List<GuitaristLessonDto>
            {
                new GuitaristLessonDto { Id = 1, GuitaristId = 1, LessonId = 1, Status = LessonStatus.InProgress, ProgressPercent = 50 },
                new GuitaristLessonDto { Id = 2, GuitaristId = 2, LessonId = 2, Status = LessonStatus.Completed, ProgressPercent = 100 }
            };

            _guitaristLessonDataMock.Setup(d => d.GetAllJoinAsync()).ReturnsAsync(guitaristLessons);
            _mapperMock.Setup(m => m.Map<IEnumerable<GuitaristLessonDto>>(guitaristLessons)).Returns(guitaristLessonDtos);

            // Act
            var result = await _business.GetAllJoinAsync();

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
            result.First().ProgressPercent.Should().Be(50);
            result.Last().ProgressPercent.Should().Be(100);
        }
    }
}