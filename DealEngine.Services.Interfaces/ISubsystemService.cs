using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface ISubsystemService
    {
        Task<bool> CreateSubObjects(Guid clientProgrammeId, ClientInformationSheet sheet, User user);
        Task<bool> ValidateProgramme(ClientInformationSheet informationSheet,User user);
    }
}
