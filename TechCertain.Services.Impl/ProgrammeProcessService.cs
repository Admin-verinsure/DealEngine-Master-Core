using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;

namespace TechCertain.Services.Impl
{
    public class ProgrammeProcessService : IProgrammeProcessService
    {
        IMapperSession<ProgrammeProcess> _programmeProcessRepository;

        public ProgrammeProcessService(IMapperSession<ProgrammeProcess> programmeProcessRepository)
        {
            _programmeProcessRepository = programmeProcessRepository;
        }

        public async Task<ProgrammeProcess> GetProcessId(Guid processId)
        {
            return await _programmeProcessRepository.GetByIdAsync(processId);
        }

        public async Task UpdateProgrammeProcess(ProgrammeProcess programmeProcess)
        {
            await _programmeProcessRepository.UpdateAsync(programmeProcess);
        }
    }
}

