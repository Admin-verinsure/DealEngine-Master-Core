using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IRevenueActivityService
    {
        Task AddRevenueByActivity(RevenueByActivity revenueData);
    }

}
