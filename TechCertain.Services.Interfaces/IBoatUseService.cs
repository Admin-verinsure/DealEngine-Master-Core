using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IBoatUseService
    {
        Task<BoatUse> CreateNewBoatUse(BoatUse boatUse);

        Task DeleteBoatUse(User deletedBy, BoatUse boatUse);

        Task<List<BoatUse>> GetAllBoatUses();

        Task<BoatUse> GetBoatUse(Guid boatUseId);

        Task UpdateBoatUse(BoatUse boatUse);

    }
}
