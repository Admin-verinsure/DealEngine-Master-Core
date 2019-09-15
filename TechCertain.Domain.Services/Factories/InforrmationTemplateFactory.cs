using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Domain.Services.Factories
{
    public class InformationTemplateFactory : IEntityFactory
    {
        public InformationTemplate CreateNewTemplate(User createdBy, string name, IList<InformationSection> sections = null)
        {
            return new InformationTemplate(createdBy, name, sections);
        }        
    }
}
