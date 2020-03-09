using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IInformationSectionService
    {
        Task<InformationSection> CreateNewSection(User createdBy, string name, IList<InformationItem> items);
        IQueryable<InformationSection> GetAllSections();
        Task<InformationSection> GetSection(Guid Id);
        Task<List<InformationSection>> GetInformationSectionsbyTemplateId(Guid Id);


    }
}
