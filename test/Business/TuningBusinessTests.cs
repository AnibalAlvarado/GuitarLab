using AutoMapper;
using Business.Implementations;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Utilities.Exceptions;

namespace test.Business
{
    public class TuningBusinessTests
    {
        private readonly Mock<ITuningData> _tuningDataMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TuningBusiness>> _loggerMock;
        private readonly TuningBusiness _business;

        public TuningBusinessTests()
        {
            _tuningDataMock = new Mock<ITuningData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TuningBusiness>>();
            _business = new TuningBusiness(_tuningDataMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        // ========================================================
        // TEST 1: GetAll
        // ========================================================
        [Fact]
        public async Task GetAll_ShouldReturnMappedTuningDtos()
        {
            // Arrange
            var tunings = new List<Tuning>
            {
                new Tuning { Id = 1, Name = "Standard", Notes = "E A D G B E", IsDeleted = false },
                new Tuning { Id = 2, Name = "Drop D", Notes = "D A D G B E", IsDeleted = false }
            };

            var tuningDtos = new List<TuningDto>
            {
                new TuningDto { Id = 1, Name = "Standard", Notes = "E A D G B E" },
                new TuningDto { Id = 2, Name = "Drop D", Notes = "D A D G B E" }
            };

            _tuningDataMock.Setup(d => d.GetAll()).ReturnsAsync(tunings);
            _mapperMock.Setup(m => m.Map<List<TuningDto>>(tunings)).Returns(tuningDtos);

            // Act
            var result = await _business.GetAll();

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
            result.First().Name.Should().Be("Standard");
            result.Last().Name.Should().Be("Drop D");
        }

        // ========================================================
        // TEST 2: GetById
        // ========================================================
        [Fact]
        public async Task GetById_ShouldReturnMappedTuningDto_WhenExists()
        {
            // Arrange
            var tuning = new Tuning { Id = 1, Name = "Standard", Notes = "E A D G B E", IsDeleted = false };
            var tuningDto = new TuningDto { Id = 1, Name = "Standard", Notes = "E A D G B E" };

            _tuningDataMock.Setup(d => d.GetById(1)).ReturnsAsync(tuning);
            _mapperMock.Setup(m => m.Map<TuningDto>(tuning)).Returns(tuningDto);

            // Act
            var result = await _business.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Standard");
            result.Notes.Should().Be("E A D G B E");
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
        public async Task GetById_ShouldThrowEntityNotFoundException_WhenTuningDoesNotExist()
        {
            // Arrange
            _tuningDataMock.Setup(d => d.GetById(999)).ReturnsAsync((Tuning)null!);

            // Act
            Func<Task> act = async () => await _business.GetById(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*Tuning*999*");
        }

        // ========================================================
        // TEST 3: Save
        // ========================================================
        [Fact]
        public async Task Save_ShouldMapAndSaveTuningDto()
        {
            // Arrange
            var tuningDto = new TuningDto { Name = "New Tuning", Notes = "C G D A E B" };
            var tuning = new Tuning { Id = 0, Name = "New Tuning", Notes = "C G D A E B", IsDeleted = false };
            var savedTuning = new Tuning { Id = 1, Name = "New Tuning", Notes = "C G D A E B", IsDeleted = false };
            var savedTuningDto = new TuningDto { Id = 1, Name = "New Tuning", Notes = "C G D A E B" };

            _mapperMock.Setup(m => m.Map<Tuning>(tuningDto)).Returns(tuning);
            _tuningDataMock.Setup(d => d.Save(tuning)).ReturnsAsync(savedTuning);
            _mapperMock.Setup(m => m.Map<TuningDto>(savedTuning)).Returns(savedTuningDto);

            // Act
            var result = await _business.Save(tuningDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("New Tuning");
            result.Notes.Should().Be("C G D A E B");
        }

        [Fact]
        public async Task Save_ShouldThrowBusinessException_WhenSaveFails()
        {
            // Arrange
            var tuningDto = new TuningDto { Name = "New Tuning" };
            var tuning = new Tuning { Name = "New Tuning" };

            _mapperMock.Setup(m => m.Map<Tuning>(tuningDto)).Returns(tuning);
            _tuningDataMock.Setup(d => d.Save(tuning)).ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _business.Save(tuningDto);

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
            var tuningDto = new TuningDto { Id = 1, Name = "Updated Tuning" };
            var tuning = new Tuning { Id = 1, Name = "Updated Tuning" };

            _mapperMock.Setup(m => m.Map<Tuning>(tuningDto)).Returns(tuning);
            _tuningDataMock.Setup(d => d.Update(tuning)).ThrowsAsync(new Exception("Update error"));

            // Act
            Func<Task> act = async () => await _business.Update(tuningDto);

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
            _tuningDataMock.Setup(d => d.Delete(1)).ReturnsAsync(1);

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
            _tuningDataMock.Setup(d => d.PermanentDelete(1)).ReturnsAsync(true);

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
        public async Task PermanentDelete_ShouldThrowEntityNotFoundException_WhenTuningDoesNotExist()
        {
            // Arrange
            _tuningDataMock.Setup(d => d.PermanentDelete(999)).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _business.PermanentDelete(999);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*Tuning*999*");
        }

        // ========================================================
        // TEST 7: ExistsAsync
        // ========================================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenTuningExists()
        {
            // Arrange
            _tuningDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Tuning, bool>>>())).ReturnsAsync(true);

            // Act
            var result = await _business.ExistsAsync(e => e.Name == "Test");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldThrowBusinessException_WhenExistsFails()
        {
            // Arrange
            _tuningDataMock.Setup(d => d.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Tuning, bool>>>())).ThrowsAsync(new Exception("Exists error"));

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
            _tuningDataMock.Setup(d => d.GetAllDynamicAsync()).ReturnsAsync(dynamicObjects);

            // Act
            var result = await _business.GetAllDynamicAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dynamicObjects);
        }
    }
}