using AutoMapper;
using Business.Implementations;
using Data.Interfaces;
using FluentAssertions;
using Moq;
using System.Dynamic;
using System.Linq.Expressions;
using test.Dtos;
using Utilities.Exceptions;

namespace test.Business
{
    public class BusinessBasicTests
    {
        private readonly Mock<IRepositoryData<FakeEntity>> _mockData;
        private readonly Mock<IMapper> _mockMapper;
        private readonly RepositoryBusiness<FakeEntity, FakeDto> _service;

        public BusinessBasicTests()
        {
            _mockData = new Mock<IRepositoryData<FakeEntity>>();
            _mockMapper = new Mock<IMapper>();

            _service = new RepositoryBusiness<FakeEntity, FakeDto>(_mockData.Object, _mockMapper.Object);
        }

        // ==========================================
        // ✅ TEST 1: GetAll()
        // ==========================================
        [Fact]
        public async Task GetAll_ShouldReturnMappedDtos()
        {
            // Arrange
            var entities = new List<FakeEntity> { new FakeEntity { Id = 1, Name = "A" } };
            _mockData.Setup(d => d.GetAll()).ReturnsAsync(entities);
            _mockMapper.Setup(m => m.Map<List<FakeDto>>(entities))
                       .Returns(new List<FakeDto> { new FakeDto { Name = "A" } });

            // Act
            var result = await _service.GetAll();

            // Assert
            result.Should().NotBeNull().And.HaveCount(1);
            result.First().Name.Should().Be("A");
        }

        // ==========================================
        // ✅ TEST 2: GetById()
        // ==========================================
        [Fact]
        public async Task GetById_ShouldReturnMappedDto_WhenExists()
        {
            var entity = new FakeEntity { Id = 1, Name = "A" };
            _mockData.Setup(d => d.GetById(1)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<FakeDto>(entity))
                       .Returns(new FakeDto { Name = "A" });

            var result = await _service.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("A");
        }

        [Fact]
        public async Task GetById_ShouldThrow_WhenNotFound()
        {
            _mockData.Setup(d => d.GetById(1)).ReturnsAsync((FakeEntity)null!);
            _mockMapper.Setup(m => m.Map<FakeDto>((FakeEntity)null!)).Returns((FakeDto)null!);

            Func<Task> act = async () => await _service.GetById(1);
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("*FakeEntity*");
        }

        [Fact]
        public async Task GetById_ShouldThrow_WhenIdIsInvalid()
        {
            Func<Task> act = async () => await _service.GetById(0);
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*El ID debe ser mayor que cero.*");
        }

        // ==========================================
        // ✅ TEST 3: Save()
        // ==========================================
        [Fact]
        public async Task Save_ShouldMapAndPersistEntity()
        {
            var dto = new FakeDto { Name = "New" };
            var entity = new FakeEntity { Id = 1, Name = "New" };

            _mockMapper.Setup(m => m.Map<FakeEntity>(dto)).Returns(entity);
            _mockData.Setup(d => d.Save(entity)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<FakeDto>(entity)).Returns(dto);

            var result = await _service.Save(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("New");
        }

        [Fact]
        public async Task Save_ShouldThrow_WhenDtoIsNull()
        {
            Func<Task> act = async () => await _service.Save(null!);
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*Error al crear la entidad*");
        }

        // ==========================================
        // ✅ TEST 4: Update()
        // ==========================================
        [Fact]
        public async Task Update_ShouldReturnTrue_WhenSuccessful()
        {
            var dto = new FakeDto { Name = "Update" };
            var entity = new FakeEntity { Id = 1, Name = "Update" };

            _mockMapper.Setup(m => m.Map<FakeEntity>(dto)).Returns(entity);
            _mockData.Setup(d => d.Update(entity)).Returns(Task.CompletedTask);

            await _service.Update(dto);

            _mockData.Verify(d => d.Update(entity), Times.Once);
        }

        // ==========================================
        // ✅ TEST 5: Delete()
        // ==========================================
        [Fact]
        public async Task Delete_ShouldReturnInt_WhenDeleted()
        {
            _mockData.Setup(d => d.Delete(1)).ReturnsAsync(1);

            var result = await _service.Delete(1);

            result.Should().Be(1);
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenIdInvalid()
        {
            Func<Task> act = async () => await _service.Delete(0);
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*El ID debe ser mayor que cero.*");
        }

        // ==========================================
        // ✅ TEST 6: PermanentDelete()
        // ==========================================
        [Fact]
        public async Task PermanentDelete_ShouldReturnTrue_WhenDeleted()
        {
            _mockData.Setup(d => d.PermanentDelete(1)).ReturnsAsync(true);

            var result = await _service.PermanentDelete(1);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PermanentDelete_ShouldThrow_WhenIdInvalid()
        {
            Func<Task> act = async () => await _service.PermanentDelete(0);
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*El ID debe ser mayor que cero.*");
        }

        // ==========================================
        // ✅ TEST 7: ExistsAsync()
        // ==========================================
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
        {
            _mockData.Setup(d => d.ExistsAsync(It.IsAny<Expression<Func<FakeEntity, bool>>>())).ReturnsAsync(true);

            var result = await _service.ExistsAsync(e => e.Id == 1);

            result.Should().BeTrue();
        }

        // ==========================================
        // ✅ TEST 8: GetAllDynamicAsync()
        // ==========================================
        [Fact]
        public async Task GetAllDynamicAsync_ShouldReturnDynamicList()
        {
            var dynamicList = new List<ExpandoObject> { new ExpandoObject() };
            _mockData.Setup(d => d.GetAllDynamicAsync()).ReturnsAsync(dynamicList);

            var result = await _service.GetAllDynamicAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }
    }
}
