using System;
using HomeCore.Entities;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.DAL
{
    public interface IUserRepository
    {
        Task<User?> GetByUserNameAsync(string userName);
    }
}
