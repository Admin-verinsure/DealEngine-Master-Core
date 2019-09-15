using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IInformationTemplateService
    {
        InformationTemplate CreateInformationTemplate(User createdBy, string name, IList<InformationSection> sections);

        IQueryable<InformationTemplate> GetAllTemplates();

		InformationTemplate GetTemplate (Guid templateId);

		InformationTemplate AddProductTo (Guid templateId, Product product);

		InformationTemplate AddProductTo (InformationTemplate template, Product product);
    }
}
