using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface ISubsystemService
    {
        Task CreateSubObjects(Guid clientProgrammeId, ClientInformationSheet sheet);
        Task ValidateSubObjects(ClientInformationSheet informationSheet);
    }
}
