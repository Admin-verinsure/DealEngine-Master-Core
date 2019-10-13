using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface ISystemEmailService
    {

        void AddNewSystemEmail(User createdBy, string systemEmailName, string internalNotes, string subject, string body, string systemEmailType);

        void RemoveSystemEmail(User deletedBy, string systemEmailName);

        bool CheckExists(string systemEmailName);

        IQueryable<SystemEmail> GetAllSystemEmails();

        SystemEmail GetSystemEmailByName(string name);

        SystemEmail GetSystemEmailByType(string systemEmailType);
    }
}

