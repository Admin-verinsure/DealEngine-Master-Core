using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IBoatUseService
    {
        BoatUse CreateNewBoatUse(BoatUse boatUse);

        bool DeleteBoatUse(User deletedBy, BoatUse boatUse);

        IQueryable<BoatUse> GetAllBoatUses();

        BoatUse GetBoatUse(Guid boatUseId);

        bool UpdateBoatUse(BoatUse boatUse);

    }
}
