using System;
using HomeCore.DAL;
using HomeCore.Entities;
using HomeCore.Entities.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.BLL
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _userRepository.GetByUserNameAsync(request.Username);
            if (usuario is null)
                return null;

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario, usuario.PasswordHash, request.Password);

            if (resultado == PasswordVerificationResult.Failed)
                return null;

            var (token, expiresAt) = _tokenService.GenerateToken(usuario);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt
            };
        }
    }
}
