using HomeCore.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.BLL
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}
