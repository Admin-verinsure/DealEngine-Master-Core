using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
    public class AuditLogService : IAuditLogService
    {      
        IMapperSession<AuditLog> _auditLogRepository;

        public AuditLogService(IMapperSession<AuditLog> auditLogRepository)
        {          
            _auditLogRepository = auditLogRepository;
        }

        public async Task<AuditLog> CreateNewAuditLog(AuditLog auditLog)
        {
            await UpdateAuditLog(auditLog);
            return auditLog;
        }

        public async Task DeleteAuditLog(User deletedBy, AuditLog auditLog)
        {
            auditLog.Delete(deletedBy);
            await UpdateAuditLog(auditLog);
        }

        public async Task<List<AuditLog>> GetAllAuditLogs()
        {
            return await _auditLogRepository.FindAll().Where(al => al.DateDeleted != null).ToListAsync();
        }

        public async Task<AuditLog> GetAuditLog(Guid auditLogId)
        {
            AuditLog auditLog = await _auditLogRepository.GetByIdAsync(auditLogId);
            if (auditLog != null)
                return auditLog;
            if (auditLog != null)
            {
                await UpdateAuditLog(auditLog);
                return auditLog;
            }
            throw new Exception("AuditLog with id [" + auditLogId + "] does not exist in the system");
        }

        public async Task UpdateAuditLog(AuditLog auditLog)
        {
            await _auditLogRepository.UpdateAsync(auditLog);
        }

        public async Task<List<AuditLog>> GetAuditLogForAuditLogClientInformationSheet(ClientInformationSheet clientInformationSheet)
        {
            return await _auditLogRepository.FindAll().Where(al => al.AuditLogClientInformationSheet == clientInformationSheet && al.AuditLogClientAgreement == null && al.DateDeleted != null).ToListAsync();
        }

        public async Task<List<AuditLog>> GetAuditLogForAuditLogClientAgreement(ClientAgreement clientAgreement)
        {
            return await _auditLogRepository.FindAll().Where(al => al.AuditLogClientAgreement == clientAgreement && al.DateDeleted != null).ToListAsync();
        }
    }
}

