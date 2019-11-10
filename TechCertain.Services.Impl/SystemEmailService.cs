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
    public class SystemEmailService : ISystemEmailService
    {
        IMapperSession<SystemEmail> _systemEmailRepository;

        public SystemEmailService(IMapperSession<SystemEmail> systemEmailRepository)
        {
            _systemEmailRepository = systemEmailRepository;
        }

        public async Task AddNewSystemEmail(User createdBy, string systemEmailName, string internalNotes, string subject, string body, string systemEmailType)
        {
            if (string.IsNullOrWhiteSpace(systemEmailName))
                throw new ArgumentNullException(nameof(systemEmailName));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentNullException(nameof(body));

            var exists = await CheckExists(systemEmailName);
            if (!exists)
                await _systemEmailRepository.AddAsync(new SystemEmail(createdBy, systemEmailName, internalNotes, subject, body, systemEmailType));
            
        }

        public async Task<bool> CheckExists(string systemEmailName)
        {
            if (string.IsNullOrWhiteSpace(systemEmailName))
                throw new ArgumentNullException(nameof(systemEmailName));
            return await _systemEmailRepository.FindAll().FirstOrDefaultAsync(se => se.SystemEmailName == systemEmailName) != null;
        }

        public async Task<List<SystemEmail>> GetAllSystemEmails()
        {
            return await _systemEmailRepository.FindAll().Where(se => se.DateDeleted == null).OrderBy(se => se.SystemEmailName).ToListAsync();
        }

        public async Task RemoveSystemEmail(User deletedBy, string systemEmailName)
        {
            SystemEmail systemEmail = await _systemEmailRepository.FindAll().FirstOrDefaultAsync(se => se.SystemEmailName == systemEmailName && se.DateDeleted == null);
            if (systemEmail != null)
            {
                await _systemEmailRepository.RemoveAsync(systemEmail);
            }
        }

        public async Task<SystemEmail> GetSystemEmailByName(string name)
        {
            SystemEmail systemEmail = await _systemEmailRepository.FindAll().FirstOrDefaultAsync(se => se.SystemEmailName == name && se.DateDeleted == null);
            if (systemEmail != null)
            {
                return systemEmail;
            } else
            {
                throw new Exception("System Email with name '" + name + "' does not exist in the system");
            }
            
        }

        public async Task<SystemEmail> GetSystemEmailByType(string systemEmailType)
        {
            SystemEmail systemEmail = await _systemEmailRepository.FindAll().FirstOrDefaultAsync(se => se.SystemEmailType == systemEmailType && se.DateDeleted == null);            
            return systemEmail;            
        }

        public async Task UpdateSystemEmailTemplate(SystemEmail systemEmailTemplate)
        {
            await _systemEmailRepository.UpdateAsync(systemEmailTemplate);
        }
    }
}
