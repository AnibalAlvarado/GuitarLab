using Data.Implementations.Auth;
using Entity.Contexts;
using Entity.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace test.Data.Auth
{
    public class RefreshTokenRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly RefreshTokenData _repository;

        public RefreshTokenRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new RefreshTokenData(_context);

            // ==========================
            // Datos iniciales
            // ==========================
            _context.Set<RefreshToken>().AddRange(
                new RefreshToken
                {
                    Id = 1,
                    UserId = 10,
                    TokenHash = "hash_valid_1",
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IsRevoked = false
                },
                new RefreshToken
                {
                    Id = 2,
                    UserId = 10,
                    TokenHash = "hash_expired",
                    ExpiresAt = DateTime.UtcNow.AddHours(-1),
                    IsRevoked = false
                },
                new RefreshToken
                {
                    Id = 3,
                    UserId = 10,
                    TokenHash = "hash_revoked",
                    ExpiresAt = DateTime.UtcNow.AddHours(2),
                    IsRevoked = true
                }
            );
            _context.SaveChanges();
        }

        // =====================================================
        // TEST 1: AddAsync
        // =====================================================
        [Fact]
        public async Task AddAsync_ShouldInsertNewToken()
        {
            var newToken = new RefreshToken
            {
                Id = 4,
                UserId = 20,
                TokenHash = "hash_new",
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                IsRevoked = false
            };

            await _repository.AddAsync(newToken);

            var stored = await _context.Set<RefreshToken>().FindAsync(4);
            stored.Should().NotBeNull();
            stored!.TokenHash.Should().Be("hash_new");
        }

        // =====================================================
        // TEST 2: GetByHashAsync
        // =====================================================
        [Fact]
        public async Task GetByHashAsync_ShouldReturnToken_WhenExists()
        {
            var result = await _repository.GetByHashAsync("hash_valid_1");

            result.Should().NotBeNull();
            result!.UserId.Should().Be(10);
            result.IsRevoked.Should().BeFalse();
        }

        [Fact]
        public async Task GetByHashAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetByHashAsync("not_found");
            result.Should().BeNull();
        }

        // =====================================================
        // TEST 3: RevokeAsync
        // =====================================================
        [Fact]
        public async Task RevokeAsync_ShouldSetIsRevokedTrue_AndSetReplacedByHash()
        {
            var token = await _context.RefreshTokens.FindAsync(1);
            token.Should().NotBeNull();

            await _repository.RevokeAsync(token!, "hash_replacement");

            var updated = await _context.Set<RefreshToken>().FindAsync(1);
            updated!.IsRevoked.Should().BeTrue();
            updated.ReplacedByTokenHash.Should().Be("hash_replacement");
        }

        [Fact]
        public async Task RevokeAsync_ShouldDoNothing_WhenTokenNotExists()
        {
            var fakeToken = new RefreshToken { Id = 999 };

            await _repository.RevokeAsync(fakeToken);

            var countRevoked = _context.Set<RefreshToken>().Count(t => t.IsRevoked);
            countRevoked.Should().Be(1); // Solo el del test anterior
        }

        // =====================================================
        // TEST 4: GetValidTokensByUserAsync
        // =====================================================
        [Fact]
        public async Task GetValidTokensByUserAsync_ShouldReturnOnlyNonRevokedAndNonExpiredTokens()
        {
            var result = await _repository.GetValidTokensByUserAsync(10);

            result.Should().HaveCount(1);
            result.First().TokenHash.Should().Be("hash_valid_1");
        }

        [Fact]
        public async Task GetValidTokensByUserAsync_ShouldReturnEmpty_WhenNoValidTokens()
        {
            var result = await _repository.GetValidTokensByUserAsync(99);

            result.Should().BeEmpty();
        }
    }
}
