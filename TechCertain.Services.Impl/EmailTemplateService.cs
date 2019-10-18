using System;
using System.Linq;
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

        public void AddEmailTemplate(User createdBy, string name, string type, string subject, string body, Product product, Programme programme)
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
            _emailTemplateRepository.AddAsync(emailTemplate);
            _programmeRepository.UpdateAsync(programme);

        }


        public IQueryable<EmailTemplate> GetEmailTemplateFor(Product product, string type)
        {
            var emailTemplate = _emailTemplateRepository.FindAll().Where(et => et.Product == product &&
                                                                                    et.Type == type);
            return emailTemplate;
        }

        public IQueryable<EmailTemplate> GetEmailTemplateFor(Programme programme, string type)
        {
            var emailTemplate = _emailTemplateRepository.FindAll().Where(et => et.Programme == programme &&
                                                                                    et.Type == type);
            return emailTemplate;
        }
    }
}
