using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IProgrammeProcessService
    {
        Task<ProgrammeProcess> GetProcessId(Guid processId);
    }
}
