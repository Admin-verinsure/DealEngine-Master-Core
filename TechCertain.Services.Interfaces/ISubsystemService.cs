using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface ISubsystemService
    {
        Task CreateSubObjects(Guid programmeId, ClientInformationSheet sheet);
    }
}
