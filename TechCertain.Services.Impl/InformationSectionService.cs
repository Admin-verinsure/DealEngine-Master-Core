using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class InformationSectionService : IInformationSectionService
    {        
        public IMapperSession<InformationSection> _informationSectionRepository;

        public InformationSectionService(IMapperSession<InformationSection> informationSectionRepository)
        {
            _informationSectionRepository = informationSectionRepository;
        }

        public InformationSection CreateNewSection(User createdBy, string name, IList<InformationItem> items)
        {
			InformationSection section = new InformationSection(createdBy, name, items);

            _informationSectionRepository.AddAsync(section);
            return section;
        }

        public IQueryable<InformationSection> GetAllSections()
        {
            return _informationSectionRepository.FindAll();
        }
    }
}
