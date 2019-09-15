using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Domain.Services.Factories;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class InformationSectionService : IInformationSectionService
    {
        public InformationSectionFactory _informationSectionFactory;

        public IRepository<InformationSection> _informationSectionRepository;

        public InformationSectionService(
            InformationSectionFactory informationSectionFactory,
            IRepository<InformationSection> informationSectionRepository)
        {
            _informationSectionFactory = informationSectionFactory;
            _informationSectionRepository = informationSectionRepository;
        }

        public InformationSection CreateNewSection(User createdBy, string name, IList<InformationItem> items)
        {
			InformationSection section = _informationSectionFactory.CreateSection(createdBy, name, items);

            _informationSectionRepository.Add(section);

            // TODO: Add these items at templates so it can be clonned properly 
            return section;
        }

        public IQueryable<InformationSection> GetAllSections()
        {
            return _informationSectionRepository.FindAll();
        }
    }
}
