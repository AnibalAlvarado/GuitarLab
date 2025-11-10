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
    public class GuitaristBusinessTests
    {
        private readonly Mock<IGuitaristData> _guitaristDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GuitaristBusiness _business;

        public GuitaristBusinessTests()
        {
            _guitaristDataMock = new Mock<IGuitaristData>();
            _mapperMock = new Mock<IMapper>();
            _business = new GuitaristBusiness(_guitaristDataMock.Object, _mapperMock.Object);
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnMappedGuitaristDtos()
        {
            // Arrange
            var guitarists = new List<Guitarist>
            {
                new Guitarist { Id = 1, Name = "John Doe", SkillLevel = SkillLevel.Beginner, ExperienceYears = 2, Asset = true },
                new Guitarist { Id = 2, Name = "Jane Smith", SkillLevel = SkillLevel.Intermediate, ExperienceYears = 5, Asset = true }
            };

            var guitaristDtos = new List<GuitaristDto>
            {
                new GuitaristDto { Id = 1, Name = "John Doe", SkillLevel = SkillLevel.Beginner, ExperienceYears = 2 },
                new GuitaristDto { Id = 2, Name = "Jane Smith", SkillLevel = SkillLevel.Intermediate, ExperienceYears = 5 }
            };

            _guitaristDataMock.Setup(d => d.GetAll()).ReturnsAsync(guitarists);
            _mapperMock.Setup(m => m.Map<List<GuitaristDto>>(guitarists)).Returns(guitaristDtos);

            // Act
            var result = await _business.GetAll();

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
            result.First().Name.Should().Be("John Doe");
            result.Last().Name.Should().Be("Jane Smith");
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnMappedGuitaristDto_WhenExists()
        {
            // Arrange
            var guitarist = new Guitarist { Id = 1, Name = "John Doe", SkillLevel = SkillLevel.Beginner, ExperienceYears = 2, Asset = true };
            var guitaristDto = new GuitaristDto { Id = 1, Name = "John Doe", SkillLevel = SkillLevel.Beginner, ExperienceYears = 2 };

            _guitaristDataMock.Setup(d => d.GetById(1)).ReturnsAsync(guitarist);
            _mapperMock.Setup(m => m.Map<GuitaristDto>(guitarist)).Returns(guitaristDto);

            // Act
            var result = await _business.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("John Doe");
            result.ExperienceYears.Should().Be(2);
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
        public async Task GetById_ShouldThrowEntityNotFoundException_WhenGuitaristDoesNotExist()
        {
            // Arrange
            _guitaristDataMock.Setup(d => d.GetById(999)).ReturnsAsync((Guitarist)null!);

            // Act
            Func<Task> act = async () => await _business.GetById(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*Guitarist*999*");
        }

        // ========================================================
        // TEST 3: Save
        // ========================================================
        [Fact]
        public async Task Save_ShouldMapAndSaveGuitaristDto()
        {
            // Arrange
            var guitaristDto = new GuitaristDto { Name = "New Guitarist", SkillLevel = SkillLevel.Beginner, ExperienceYears = 1 };
            var guitarist = new Guitarist { Id = 0, Name = "New Guitarist", SkillLevel = SkillLevel.Beginner, ExperienceYears = 1, Asset = true };
            var savedGuitarist = new Guitarist { Id = 1, Name = "New Guitarist", SkillLevel = SkillLevel.Beginner, ExperienceYears = 1, Asset = true };
            var savedGuitaristDto = new GuitaristDto { Id = 1, Name = "New Guitarist", SkillLevel = SkillLevel.Beginner, ExperienceYears = 1 };

            _mapperMock.Setup(m => m.Map<Guitarist>(guitaristDto)).Returns(guitarist);
            _guitaristDataMock.Setup(d => d.Save(guitarist)).ReturnsAsync(savedGuitarist);
            _mapperMock.Setup(m => m.Map<GuitaristDto>(savedGuitarist)).Returns(savedGuitaristDto);

            // Act
            var result = await _business.Save(guitaristDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("New Guitarist");
            result.ExperienceYears.Should().Be(1);
        }

        [Fact]
        public async Task Save_ShouldThrowBusinessException_WhenSaveFails()
        {
            // Arrange
            var guitaristDto = new GuitaristDto { Name = "New Guitarist" };
            var guitarist = new Guitarist { Name = "New Guitarist" };

            _mapperMock.Setup(m => m.Map<Guitarist>(guitaristDto)).Returns(guitarist);
            _guitaristDataMock.Setup(d => d.Save(guitarist)).ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _business.Save(guitaristDto);

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
            var guitaristDto = new GuitaristDto { Id = 1, Name = "Updated Guitarist" };
            var guitarist = new Guitarist { Id = 1, Name = "Updated Guitarist" };

            _mapperMock.Setup(m => m.Map<Guitarist>(guitaristDto)).Returns(guitarist);
            _guitaristDataMock.Setup(d => d.Update(guitarist)).ThrowsAsync(new Exception("Update error"));

            // Act
            Func<Task> act = async () => await _business.Update(guitaristDto);

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
            _guitaristDataMock.Setup(d => d.Delete(1)).ReturnsAsync(1);

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
            _guitaristDataMock.Setup(d => d.PermanentDelete(1)).ReturnsAsync(true);

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
        public async Task PermanentDelete_ShouldThrowEntityNotFoundException_WhenGuitaristDoesNotExist()
        {
            // Arrange
            _guitaristDataMock.Setup(d => d.PermanentDelete(999)).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _business.PermanentDelete(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*Guitarist*999*");
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenGuitaristExists()
        {
            // Arrange
            _guitaristDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Guitarist, bool>>>())).ReturnsAsync(true);

            // Act
            var result = await _business.ExistsAsync(e => e.Name == "Test");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldThrowBusinessException_WhenExistsFails()
        {
            // Arrange
            _guitaristDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Guitarist, bool>>>())).ThrowsAsync(new Exception("Exists error"));

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
            _guitaristDataMock.Setup(d => d.GetAllDynamicAsync()).ReturnsAsync(dynamicObjects);

            // Act
            var result = await _business.GetAllDynamicAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dynamicObjects);
        }
    }
}