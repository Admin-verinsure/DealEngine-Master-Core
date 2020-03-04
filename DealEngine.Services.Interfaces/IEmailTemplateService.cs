using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IEmailTemplateService
    {
        Task AddEmailTemplate(User createdBy, string name, string type, string subject, string body, Product product, Programme programme);

        Task<List<EmailTemplate>> GetEmailTemplateFor(Product product, string type);

        Task<List<EmailTemplate>> GetEmailTemplateFor(Programme programme, string type);
    }
}