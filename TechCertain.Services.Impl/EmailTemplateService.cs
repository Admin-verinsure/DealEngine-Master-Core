using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class EmailTemplateService : IEmailTemplateService
    {

        IMapperSession<EmailTemplate> _emailTemplateRepository;
        IMapperSession<Programme> _programmeRepository;

        public EmailTemplateService(IMapperSession<EmailTemplate> emailTemplateRepository, IMapperSession<Programme> programmeRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _programmeRepository = programmeRepository;
        }

        public async Task AddEmailTemplate(User createdBy, string name, string type, string subject, string body, Product product, Programme programme)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentNullException(nameof(body));

            EmailTemplate emailTemplate = new EmailTemplate(createdBy, name, type, subject, body, product, programme);
            programme.EmailTemplates.Add(emailTemplate);
            await _emailTemplateRepository.AddAsync(emailTemplate);
            await _programmeRepository.UpdateAsync(programme);

        }


        public async Task<List<EmailTemplate>> GetEmailTemplateFor(Product product, string type)
        {
            return await _emailTemplateRepository.FindAll().Where(et => et.Product == product &&
                                                                                    et.Type == type).ToListAsync();
        }

        public async Task<List<EmailTemplate>> GetEmailTemplateFor(Programme programme, string type)
        {
            return await _emailTemplateRepository.FindAll().Where(et => et.Programme == programme &&
                                                                                    et.Type == type).ToListAsync();
        }
    }
}
