using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class EmailTemplateService : IEmailTemplateService
    {
        IUnitOfWork _unitOfWork;
        IMapperSession<EmailTemplate> _emailTemplateRepository;

        public EmailTemplateService(IUnitOfWork unitOfWork, IMapperSession<EmailTemplate> emailTemplateRepository)
        {
            _unitOfWork = unitOfWork;
            _emailTemplateRepository = emailTemplateRepository;
        }

        public bool AddEmailTemplate(User createdBy, string name, string type, string subject, string body, Product product, Programme programme)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentNullException(nameof(body));

            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
                EmailTemplate emailTemplate = new EmailTemplate(createdBy, name, type, subject, body, product, programme);
                programme.EmailTemplates.Add(emailTemplate);
                work.Commit();
            }

            return true;
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
