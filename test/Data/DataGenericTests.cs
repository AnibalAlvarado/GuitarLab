using Data.Implementations;
using Entity.Models;
using Entity.Contexts;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Dynamic;
using System.Linq.Expressions;
using test.Context;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace test.Data
{
    // Entidad de prueba simple
    public class FakeEntity : BaseModel
    {
        public string? Name { get; set; }
    }

    public class DataGenericTests
    {
        private readonly ApplicationDbContext _context;
        private readonly RepositoryData<FakeEntity> _repository;

        public DataGenericTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TestDbContext(options);
            var mockConfig = new Mock<IConfiguration>();
            var mockAudit = new Mock<IAuditService>();
            var mockUser = new Mock<ICurrentUserService>();
            _repository = new RepositoryData<FakeEntity>(_context, mockConfig.Object, mockAudit.Object, mockUser.Object);

            // Datos iniciales
            _context.Set<FakeEntity>().AddRange(
                new FakeEntity { Id = 1, Name = "A", IsDeleted = false },
                new FakeEntity { Id = 2, Name = "B", IsDeleted = true },
                new FakeEntity { Id = 3, Name = "C", IsDeleted = false }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOnlyActive()
        {
            var result = await _repository.GetAll();

            result.Should().HaveCount(2);
            result.All(x => !x.IsDeleted).Should().BeTrue();
        }

        // Note: RepositoryData does not have GetDeletes method, so removing this test

        [Fact]
        public async Task GetById_ShouldReturnEntity_WhenExistsAndActive()
        {
            var result = await _repository.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("A");
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenDeleted()
        {
            var result = await _repository.GetById(2);
            result.Should().BeNull();
        }

        [Fact]
        public async Task Save_ShouldAddNewEntity()
        {
            var entity = new FakeEntity { Id = 4, Name = "D" };

            var created = await _repository.Save(entity);

            created.Should().NotBeNull();
            _context.Set<FakeEntity>().Count().Should().Be(4);
        }

        [Fact]
        public async Task Update_ShouldModifyExistingEntity()
        {
            var entity = _context.Set<FakeEntity>().First(x => x.Id == 1);
            entity.Name = "Updated";

            await _repository.Update(entity);

            var updated = await _repository.GetById(1);
            updated!.Name.Should().Be("Updated");
        }

        [Fact]
        public async Task Delete_ShouldMarkAsDeleted()
        {
            var result = await _repository.Delete(1);

            result.Should().Be(1);
            var entity = await _context.Set<FakeEntity>().FindAsync(1);
            entity!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task PermanentDelete_ShouldRemoveEntity()
        {
            var result = await _repository.PermanentDelete(1);

            result.Should().BeTrue();
            _context.Set<FakeEntity>().Find(1).Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
        {
            var result = await _repository.ExistsAsync(e => e.Id == 1);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllDynamicAsync_ShouldReturnDynamicList()
        {
            var result = await _repository.GetAllDynamicAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2); // Only active entities
        }
    }
}