using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IInformationTemplateService
    {
        Task<InformationTemplate> CreateInformationTemplate(User createdBy, string name, IList<InformationSection> sections);

        Task<List<InformationTemplate>> GetAllTemplates();

        Task<InformationTemplate> GetTemplate(Guid templateId);

        Task<InformationTemplate> AddProductTo(Guid templateId, Product product);

        Task<InformationTemplate> AddProductTo(InformationTemplate template, Product product);
    }
}
