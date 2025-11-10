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
    public class TechniqueBusinessTests
    {
        private readonly Mock<ITechniqueData> _techniqueDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TechniqueBusiness _business;

        public TechniqueBusinessTests()
        {
            _techniqueDataMock = new Mock<ITechniqueData>();
            _mapperMock = new Mock<IMapper>();
            _business = new TechniqueBusiness(_techniqueDataMock.Object, _mapperMock.Object);
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnMappedTechniqueDtos()
        {
            // Arrange
            var techniques = new List<Technique>
            {
                new Technique { Id = 1, Name = "Fingerpicking", Description = "Technique for fingerpicking", IsDeleted = false },
                new Technique { Id = 2, Name = "Bending", Description = "String bending technique",  IsDeleted = false }
            };

            var techniqueDtos = new List<TechniqueDto>
            {
                new TechniqueDto { Id = 1, Name = "Fingerpicking", Description = "Technique for fingerpicking" },
                new TechniqueDto { Id = 2, Name = "Bending", Description = "String bending technique" }
            };

            _techniqueDataMock.Setup(d => d.GetAll()).ReturnsAsync(techniques);
            _mapperMock.Setup(m => m.Map<List<TechniqueDto>>(techniques)).Returns(techniqueDtos);

            // Act
            var result = await _business.GetAll();

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
            result.First().Name.Should().Be("Fingerpicking");
            result.Last().Name.Should().Be("Bending");
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnMappedTechniqueDto_WhenExists()
        {
            // Arrange
            var technique = new Technique { Id = 1, Name = "Fingerpicking", Description = "Technique for fingerpicking", IsDeleted = false };
            var techniqueDto = new TechniqueDto { Id = 1, Name = "Fingerpicking", Description = "Technique for fingerpicking" };

            _techniqueDataMock.Setup(d => d.GetById(1)).ReturnsAsync(technique);
            _mapperMock.Setup(m => m.Map<TechniqueDto>(technique)).Returns(techniqueDto);

            // Act
            var result = await _business.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Fingerpicking");
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
        public async Task GetById_ShouldThrowEntityNotFoundException_WhenTechniqueDoesNotExist()
        {
            // Arrange
            _techniqueDataMock.Setup(d => d.GetById(999)).ReturnsAsync((Technique)null!);

            // Act
            Func<Task> act = async () => await _business.GetById(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*Technique*999*");
        }

        // ========================================================
        // TEST 3: Save
        // ========================================================
        [Fact]
        public async Task Save_ShouldMapAndSaveTechniqueDto()
        {
            // Arrange
            var techniqueDto = new TechniqueDto { Name = "New Technique", Description = "A new technique" };
            var technique = new Technique { Id = 0, Name = "New Technique", Description = "A new technique",  IsDeleted = false };
            var savedTechnique = new Technique { Id = 1, Name = "New Technique", Description = "A new technique",  IsDeleted = false };
            var savedTechniqueDto = new TechniqueDto { Id = 1, Name = "New Technique", Description = "A new technique" };

            _mapperMock.Setup(m => m.Map<Technique>(techniqueDto)).Returns(technique);
            _techniqueDataMock.Setup(d => d.Save(technique)).ReturnsAsync(savedTechnique);
            _mapperMock.Setup(m => m.Map<TechniqueDto>(savedTechnique)).Returns(savedTechniqueDto);

            // Act
            var result = await _business.Save(techniqueDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("New Technique");
        }

        [Fact]
        public async Task Save_ShouldThrowBusinessException_WhenSaveFails()
        {
            // Arrange
            var techniqueDto = new TechniqueDto { Name = "New Technique" };
            var technique = new Technique { Name = "New Technique" };

            _mapperMock.Setup(m => m.Map<Technique>(techniqueDto)).Returns(technique);
            _techniqueDataMock.Setup(d => d.Save(technique)).ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _business.Save(techniqueDto);

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
            var techniqueDto = new TechniqueDto { Id = 1, Name = "Updated Technique" };
            var technique = new Technique { Id = 1, Name = "Updated Technique" };

            _mapperMock.Setup(m => m.Map<Technique>(techniqueDto)).Returns(technique);
            _techniqueDataMock.Setup(d => d.Update(technique)).ThrowsAsync(new Exception("Update error"));

            // Act
            Func<Task> act = async () => await _business.Update(techniqueDto);

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
            _techniqueDataMock.Setup(d => d.Delete(1)).ReturnsAsync(1);

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
            _techniqueDataMock.Setup(d => d.PermanentDelete(1)).ReturnsAsync(true);

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
        public async Task PermanentDelete_ShouldThrowEntityNotFoundException_WhenTechniqueDoesNotExist()
        {
            // Arrange
            _techniqueDataMock.Setup(d => d.PermanentDelete(999)).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _business.PermanentDelete(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*Technique*999*");
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenTechniqueExists()
        {
            // Arrange
            _techniqueDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Technique, bool>>>())).ReturnsAsync(true);

            // Act
            var result = await _business.ExistsAsync(e => e.Name == "Test");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldThrowBusinessException_WhenExistsFails()
        {
            // Arrange
            _techniqueDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Technique, bool>>>())).ThrowsAsync(new Exception("Exists error"));

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
            _techniqueDataMock.Setup(d => d.GetAllDynamicAsync()).ReturnsAsync(dynamicObjects);

            // Act
            var result = await _business.GetAllDynamicAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dynamicObjects);
        }
    }
}