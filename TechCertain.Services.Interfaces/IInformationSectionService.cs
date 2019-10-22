using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IInformationSectionService
    {
        Task<InformationSection> CreateNewSection(User createdBy, string name, IList<InformationItem> items);
        Task<List<InformationSection>> GetAllSections();
    }
}
