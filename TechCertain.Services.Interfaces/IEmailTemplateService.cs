using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IEmailTemplateService
    {
        bool AddEmailTemplate(User createdBy, string name, string type, string subject, string body, Product product, Programme programme);

        IQueryable<EmailTemplate> GetEmailTemplateFor(Product product, string type);

        IQueryable<EmailTemplate> GetEmailTemplateFor(Programme programme, string type);
    }
}