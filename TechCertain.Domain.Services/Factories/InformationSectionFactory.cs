using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Domain.Services.Factories
{
    public class InformationSectionFactory : IEntityFactory
    {
        public InformationSection CreateSection(User createdBy, string name, IList<InformationItem> items = null)
        {
            return new InformationSection(createdBy, name, items);
        }
    }
}
