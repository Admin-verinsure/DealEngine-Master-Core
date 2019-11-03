using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace TechCertain.Services.Impl
{
    public class ProgrammeProcessService : IProgrammeProcessService
    {
        IMapperSession<ProgrammeProcess> _programmeProcessRepository;

        public ProgrammeProcessService(IMapperSession<ProgrammeProcess> programmeProcessRepository)
        {
            _programmeProcessRepository = programmeProcessRepository;
        }

        public async Task<ProgrammeProcess> GetProcess(string Name)
        {
            return await _programmeProcessRepository.FindAll().FirstOrDefaultAsync(p => p.Name == Name);
        }

    }
}

