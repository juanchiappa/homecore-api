using System;
using System.Collections.Generic;
using System.Text;
using HomeCore.Entities;

namespace HomeCore.BLL
{
    public interface IServicesService
    {
        Task<List<ServiceStatus>> CheckAllAsync(CancellationToken ct = default);
    }
}
