using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class SystemEmailService : ISystemEmailService
    {
        IMapperSession<SystemEmail> _systemEmailRepository;

        public SystemEmailService(IMapperSession<SystemEmail> systemEmailRepository)
        {
            _systemEmailRepository = systemEmailRepository;
        }

        public void AddNewSystemEmail(User createdBy, string systemEmailName, string internalNotes, string subject, string body, string systemEmailType)
        {
            if (string.IsNullOrWhiteSpace(systemEmailName))
                throw new ArgumentNullException(nameof(systemEmailName));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentNullException(nameof(body));

            _systemEmailRepository.AddAsync(new SystemEmail(createdBy, systemEmailName, internalNotes, subject, body, systemEmailType));
            CheckExists(systemEmailName);
        }

        public bool CheckExists(string systemEmailName)
        {
            if (string.IsNullOrWhiteSpace(systemEmailName))
                throw new ArgumentNullException(nameof(systemEmailName));
            return _systemEmailRepository.FindAll().FirstOrDefault(se => se.SystemEmailName == systemEmailName) != null;
        }

        public IQueryable<SystemEmail> GetAllSystemEmails()
        {
            var systemEmails = _systemEmailRepository.FindAll();
            return systemEmails.Where(se => se.DateDeleted == null).OrderBy(se => se.SystemEmailName);
        }

        public void RemoveSystemEmail(User deletedBy, string systemEmailName)
        {
            SystemEmail systemEmail = GetAllSystemEmails().FirstOrDefault(se => se.SystemEmailName == systemEmailName);
            if (systemEmail != null)
            {
                _systemEmailRepository.AddAsync(systemEmail);
            }
        }

        public SystemEmail GetSystemEmailByName(string name)
        {
            SystemEmail systemEmail = GetAllSystemEmails().FirstOrDefault(se => se.SystemEmailName == name);
            if (systemEmail != null)
            {
                return systemEmail;
            } else
            {
                throw new Exception("System Email with name '" + name + "' does not exist in the system");
            }
            
        }

        public SystemEmail GetSystemEmailByType(string systemEmailType)
        {
            SystemEmail systemEmail = GetAllSystemEmails().FirstOrDefault(se => se.SystemEmailType == systemEmailType);
            if (systemEmail != null)
            {
                return systemEmail;
            }
            else
            {
                throw new Exception("System Email with type '" + systemEmailType + "' does not exist in the system");
            }

        }
    }
}
