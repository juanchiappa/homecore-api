using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.DAL
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
