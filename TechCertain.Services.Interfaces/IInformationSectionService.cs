using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IInformationSectionService
    {
        InformationSection CreateNewSection(User createdBy, string name, IList<InformationItem> items);
        IQueryable<InformationSection> GetAllSections();
    }
}
