using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IEmailTemplateService
    {
        void AddEmailTemplate(User createdBy, string name, string type, string subject, string body, Product product, Programme programme);

        IQueryable<EmailTemplate> GetEmailTemplateFor(Product product, string type);

        IQueryable<EmailTemplate> GetEmailTemplateFor(Programme programme, string type);
    }
}