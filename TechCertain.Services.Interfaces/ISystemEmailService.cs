using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface ISystemEmailService
    {

        Task AddNewSystemEmail(User createdBy, string systemEmailName, string internalNotes, string subject, string body, string systemEmailType);

        Task RemoveSystemEmail(User deletedBy, string systemEmailName);

        Task<bool>  CheckExists(string systemEmailName);

        Task<List<SystemEmail>> GetAllSystemEmails();

        Task<SystemEmail> GetSystemEmailByName(string name);

        Task<SystemEmail> GetSystemEmailByType(string systemEmailType);
        Task UpdateSystemEmailTemplate(SystemEmail systemEmailTemplate);
    }
}

