using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IBoatUseService
    {
        BoatUse CreateNewBoatUse(BoatUse boatUse);

        void DeleteBoatUse(User deletedBy, BoatUse boatUse);

        IQueryable<BoatUse> GetAllBoatUses();

        BoatUse GetBoatUse(Guid boatUseId);

        void UpdateBoatUse(BoatUse boatUse);

    }
}
