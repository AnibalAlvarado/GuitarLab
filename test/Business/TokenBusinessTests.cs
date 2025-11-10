using AutoMapper;
using Business.Custom;
using Business.Interfaces;
using Data.interfaces.Auth;
using Entity.Dtos;
using Entity.Dtos.Config;
using Entity.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Utilities.Exceptions;

namespace test.Business
{
    public class TokenBusinessTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IUserBusiness> _userBusinessMock;
        private readonly Mock<ILogger<TokenBusiness>> _loggerMock;
        private readonly Mock<IRefreshTokenData> _refreshTokenDataMock;
        private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
        private readonly TokenBusiness _business;

        public TokenBusinessTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _userBusinessMock = new Mock<IUserBusiness>();
            _loggerMock = new Mock<ILogger<TokenBusiness>>();
            _refreshTokenDataMock = new Mock<IRefreshTokenData>();
            _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();

            var jwtSettings = new JwtSettings
            {
                Key = "supersecretkeythatislongenoughforhmacsha256algorithm",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenExpirationMinutes = 15,
                RefreshTokenExpirationDays = 7
            };

            _jwtSettingsMock.Setup(x => x.Value).Returns(jwtSettings);

            _business = new TokenBusiness(
                _configurationMock.Object,
                _userBusinessMock.Object,
                _loggerMock.Object,
                _refreshTokenDataMock.Object,
                _jwtSettingsMock.Object
            );
        }

        // ========================================================
        // TEST 1: GenerateTokensAsync
        // ========================================================
        [Fact]
        public async Task GenerateTokensAsync_ShouldReturnTokens_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginDto = new LoginRequestDto { Email = "user@example.com", Password = "password123" };
            var userDto = new UserDto { Id = 1, Username = "user", Email = "user@example.com" };

            _userBusinessMock.Setup(u => u.LoginUser(loginDto)).ReturnsAsync(userDto);
            _refreshTokenDataMock.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
            _refreshTokenDataMock.Setup(r => r.GetValidTokensByUserAsync(1)).ReturnsAsync(new List<RefreshToken>());

            // Act
            var result = await _business.GenerateTokensAsync(loginDto);

            // Assert
            result.AccessToken.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeNullOrEmpty();
            result.CsrfToken.Should().NotBeNullOrEmpty();

            // Verify JWT structure
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(result.AccessToken);

            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1");
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "user@example.com");
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Name && c.Value == "user");
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Iat);
        }

        [Fact]
        public async Task GenerateTokensAsync_ShouldRevokeExcessTokens_WhenUserHasTooMany()
        {
            // Arrange
            var loginDto = new LoginRequestDto { Email = "user@example.com", Password = "password123" };
            var userDto = new UserDto { Id = 1, Username = "user", Email = "user@example.com" };

            var existingTokens = new List<RefreshToken>
            {
                new RefreshToken { Id = 1, UserId = 1, IsRevoked = false },
                new RefreshToken { Id = 2, UserId = 1, IsRevoked = false },
                new RefreshToken { Id = 3, UserId = 1, IsRevoked = false },
                new RefreshToken { Id = 4, UserId = 1, IsRevoked = false },
                new RefreshToken { Id = 5, UserId = 1, IsRevoked = false },
                new RefreshToken { Id = 6, UserId = 1, IsRevoked = false } // 6 tokens, max is 5
            };

            _userBusinessMock.Setup(u => u.LoginUser(loginDto)).ReturnsAsync(userDto);
            _refreshTokenDataMock.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
            _refreshTokenDataMock.Setup(r => r.GetValidTokensByUserAsync(1)).ReturnsAsync(existingTokens);
            _refreshTokenDataMock.Setup(r => r.RevokeAsync(It.IsAny<RefreshToken>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            await _business.GenerateTokensAsync(loginDto);

            // Assert
            _refreshTokenDataMock.Verify(
                    r => r.RevokeAsync(It.IsAny<RefreshToken>(), It.IsAny<string>()),
                    Times.AtLeastOnce
                );

        }

        // ========================================================
        // TEST 2: RefreshAsync
        // ========================================================
        [Fact]
        public async Task RefreshAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
        {
            // Arrange
            var refreshTokenPlain = "validrefreshtoken";
            var hashedToken = "hashedtoken"; // This would be the actual hash
            var refreshEntity = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                TokenHash = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsRevoked = false
            };
            var userDto = new UserDto { Id = 1, Username = "user", Email = "user@example.com" };

            _refreshTokenDataMock.Setup(r => r.GetByHashAsync(It.IsAny<string>())).ReturnsAsync(refreshEntity);
            _userBusinessMock.Setup(u => u.GetById(1)).ReturnsAsync(userDto);
            _refreshTokenDataMock.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
            _refreshTokenDataMock.Setup(r => r.RevokeAsync(It.Is<RefreshToken>(t => t == refreshEntity), It.Is<string>(s => true))).Returns(Task.CompletedTask);

            // Act
            var result = await _business.RefreshAsync(refreshTokenPlain);

            // Assert
            result.NewAccessToken.Should().NotBeNullOrEmpty();
            result.NewRefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RefreshAsync_ShouldThrowSecurityTokenException_WhenRefreshTokenNotFound()
        {
            // Arrange
            var refreshTokenPlain = "invalidtoken";

            _refreshTokenDataMock.Setup(r => r.GetByHashAsync(It.IsAny<string>())).ReturnsAsync((RefreshToken)null!);

            // Act
            Func<Task> act = async () => await _business.RefreshAsync(refreshTokenPlain);

            // Assert
            await act.Should().ThrowAsync<SecurityTokenException>()
                .WithMessage("Refresh token inválido");
        }

        [Fact]
        public async Task RefreshAsync_ShouldThrowSecurityTokenException_WhenRefreshTokenExpired()
        {
            // Arrange
            var refreshTokenPlain = "expiredtoken";
            var hashedToken = "hashedtoken";
            var refreshEntity = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                TokenHash = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddHours(-1), // Expired
                IsRevoked = false
            };

            _refreshTokenDataMock.Setup(r => r.GetByHashAsync(It.IsAny<string>())).ReturnsAsync(refreshEntity);

            // Act
            Func<Task> act = async () => await _business.RefreshAsync(refreshTokenPlain);

            // Assert
            await act.Should().ThrowAsync<SecurityTokenException>()
                .WithMessage("Refresh token expirado");
        }

        [Fact]
        public async Task RefreshAsync_ShouldThrowSecurityTokenException_WhenRefreshTokenRevoked()
        {
            // Arrange
            var refreshTokenPlain = "revokedtoken";
            var hashedToken = "hashedtoken";
            var refreshEntity = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                TokenHash = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsRevoked = true
            };

            _refreshTokenDataMock.Setup(r => r.GetByHashAsync(It.IsAny<string>())).ReturnsAsync(refreshEntity);
            _refreshTokenDataMock.Setup(r => r.GetValidTokensByUserAsync(1)).ReturnsAsync(new List<RefreshToken>());

            // Act
            Func<Task> act = async () => await _business.RefreshAsync(refreshTokenPlain);

            // Assert
            await act.Should().ThrowAsync<SecurityTokenException>()
                .WithMessage("Refresh token inválido o reutilizado");
        }

        [Fact]
        public async Task RefreshAsync_ShouldRevokeAllValidTokens_WhenReusedTokenDetected()
        {
            // Arrange
            var refreshTokenPlain = "reusedtoken";
            var hashedToken = "hashedtoken";
            var refreshEntity = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                TokenHash = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsRevoked = true
            };
            var validTokens = new List<RefreshToken>
            {
                new RefreshToken { Id = 2, UserId = 1, IsRevoked = false },
                new RefreshToken { Id = 3, UserId = 1, IsRevoked = false }
            };

            _refreshTokenDataMock.Setup(r => r.GetByHashAsync(It.IsAny<string>())).ReturnsAsync(refreshEntity);
            _refreshTokenDataMock.Setup(r => r.GetValidTokensByUserAsync(1)).ReturnsAsync(validTokens);
            _refreshTokenDataMock.Setup(r => r.RevokeAsync(It.IsAny<RefreshToken>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            Func<Task> act = async () => await _business.RefreshAsync(refreshTokenPlain);

            // Assert
            await act.Should().ThrowAsync<SecurityTokenException>();
            _refreshTokenDataMock.Verify(r => r.RevokeAsync(It.IsAny<RefreshToken>(), It.IsAny<string>()), Times.Exactly(2));
        }

        // ========================================================
        // TEST 3: RevokeRefreshTokenAsync
        // ========================================================
        [Fact]
        public async Task RevokeRefreshTokenAsync_ShouldRevokeToken_WhenExistsAndNotRevoked()
        {
            // Arrange
            var refreshTokenPlain = "tokentorevoke";
            var hashedToken = "hashedtoken";
            var refreshEntity = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                TokenHash = hashedToken,
                IsRevoked = false
            };

            _refreshTokenDataMock.Setup(r => r.GetByHashAsync(It.IsAny<string>())).ReturnsAsync(refreshEntity);
            _refreshTokenDataMock.Setup(r => r.RevokeAsync(It.Is<RefreshToken>(t => t == refreshEntity), It.Is<string>(s => s == null))).Returns(Task.CompletedTask);

            // Act
            await _business.RevokeRefreshTokenAsync(refreshTokenPlain);

            // Assert
            _refreshTokenDataMock.Verify(r => r.RevokeAsync(It.Is<RefreshToken>(t => t == refreshEntity), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_ShouldDoNothing_WhenTokenNotFound()
        {
            // Arrange
            var refreshTokenPlain = "nonexistenttoken";

            _refreshTokenDataMock.Setup(r => r.GetByHashAsync(It.IsAny<string>())).ReturnsAsync((RefreshToken)null!);

            // Act
            await _business.RevokeRefreshTokenAsync(refreshTokenPlain);

            // Assert
            _refreshTokenDataMock.Verify(r => r.RevokeAsync(It.IsAny<RefreshToken>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_ShouldDoNothing_WhenTokenAlreadyRevoked()
        {
            // Arrange
            var refreshTokenPlain = "alreadyrevokedtoken";
            var hashedToken = "hashedtoken";
            var refreshEntity = new RefreshToken
            {
                Id = 1,
                UserId = 1,
                TokenHash = hashedToken,
                IsRevoked = true
            };

            _refreshTokenDataMock.Setup(r => r.GetByHashAsync(It.IsAny<string>())).ReturnsAsync(refreshEntity);

            // Act
            await _business.RevokeRefreshTokenAsync(refreshTokenPlain);

            // Assert
            _refreshTokenDataMock
                .Setup(r => r.GetByHashAsync(It.IsAny<string>()))
                .ReturnsAsync(refreshEntity);
        }
    }
}