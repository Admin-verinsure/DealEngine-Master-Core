using System;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IProgrammeProcessService
    {
        Task<ProgrammeProcess> GetProcessId(Guid processId);
        Task UpdateProgrammeProcess(ProgrammeProcess programmeProcess);
    }
}
