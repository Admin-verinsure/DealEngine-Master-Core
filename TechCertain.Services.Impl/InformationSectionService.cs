﻿using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<InformationSection> CreateNewSection(User createdBy, string name, IList<InformationItem> items)
        {
			InformationSection section = new InformationSection(createdBy, name, items);

            await _informationSectionRepository.AddAsync(section);
            return section;
        }

        public async Task<List<InformationSection>> GetAllSections()
        {
            return await _informationSectionRepository.FindAll().ToListAsync();
        }

        public async Task<InformationSection> GetSection(Guid Id)
        {
            return await _informationSectionRepository.GetByIdAsync(Id);
        }
    }
}
