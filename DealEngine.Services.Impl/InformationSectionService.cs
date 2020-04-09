using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
    public class InformationSectionService : IInformationSectionService
    {        
        public IMapperSession<InformationSection> _informationSectionRepository;

        public InformationSectionService(IMapperSession<InformationSection> informationSectionRepository)
        {
            _informationSectionRepository = informationSectionRepository;
        }

        public async Task<InformationSection> CreateNewSection(User createdBy, string name, IList<InformationItem> items)
        {
			InformationSection section = new InformationSection(createdBy, name, items);

            await _informationSectionRepository.AddAsync(section);
            return section;
        }

        public  IQueryable<InformationSection> GetAllSections()
        {
            return  _informationSectionRepository.FindAll();
        }

        public async Task<InformationSection> GetSection(Guid Id)
        {
            return await _informationSectionRepository.GetByIdAsync(Id);
        }

        public async Task<List<InformationSection>> GetInformationSectionsbyTemplateId(Guid Id)
        {
            List<InformationSection> sections = new List<InformationSection>();

            sections =  await _informationSectionRepository.FindAll().Where(i => i.InformationTemplate.Id == Id).ToListAsync();
            //sections.AddRange (Products.SelectMany ((arg) => arg.SharedViews));
            // TODO modify to include shared panels
            //sections.AddRange(Products.SelectMany((arg) => arg.UniqueQuestions));

            return sections;
        }

        public async Task CreateNewSection(InformationSection section)
        {
            await _informationSectionRepository.AddAsync(section);
        }
    }
}
