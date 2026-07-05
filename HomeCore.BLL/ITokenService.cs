using HomeCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.BLL
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) GenerateToken(User usuario);
    }
}
