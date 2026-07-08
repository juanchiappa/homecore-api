using HomeCore.BLL;
using HomeCore.DAL;
using HomeCore.Entities;
using HomeCore.Entities.DTOs;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _tokenServiceMock = new Mock<ITokenService>();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "admin", PasswordHash = "hash", Role = "admin" };

            _userRepositoryMock
                .Setup(r => r.GetByUserNameAsync("admin"))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyHashedPassword(user, "hash", "password"))
                .Returns(PasswordVerificationResult.Success);

            _tokenServiceMock
                .Setup(t => t.GenerateToken(user))
                .Returns(("fake-jwt-token", DateTime.UtcNow.AddHours(1)));

            var request = new LoginRequestDto { Username = "admin", Password = "password" };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("fake-jwt-token", result.Token);
        }

        [Fact]
        public async Task Login_UserNotFound_ReturnsNull()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetByUserNameAsync("noexiste"))
                .ReturnsAsync((User?)null);

            var request = new LoginRequestDto { Username = "noexiste", Password = "password" };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsNull()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "admin", PasswordHash = "hash", Role = "admin" };

            _userRepositoryMock
                .Setup(r => r.GetByUserNameAsync("admin"))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyHashedPassword(user, "hash", "wrong-password"))
                .Returns(PasswordVerificationResult.Failed);

            var request = new LoginRequestDto { Username = "admin", Password = "wrong-password" };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.Null(result);
        }
    }
}
