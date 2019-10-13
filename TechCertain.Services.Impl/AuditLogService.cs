using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class AuditLogService : IAuditLogService
    {      
        IMapperSession<AuditLog> _auditLogRepository;

        public AuditLogService(IMapperSession<AuditLog> auditLogRepository)
        {          
            _auditLogRepository = auditLogRepository;
        }

        public AuditLog CreateNewAuditLog(AuditLog auditLog)
        {
            UpdateAuditLog(auditLog);
            return auditLog;
        }

        public void DeleteAuditLog(User deletedBy, AuditLog auditLog)
        {
            auditLog.Delete(deletedBy);
            UpdateAuditLog(auditLog);
        }

        public IQueryable<AuditLog> GetAllAuditLogs()
        {
            return _auditLogRepository.FindAll().Where(al => al.DateDeleted != null);
        }

        public AuditLog GetAuditLog(Guid auditLogId)
        {
            AuditLog auditLog = _auditLogRepository.GetById(auditLogId).Result;
            if (auditLog != null)
                return auditLog;
            if (auditLog != null)
            {
                UpdateAuditLog(auditLog);
                return auditLog;
            }
            throw new Exception("AuditLog with id [" + auditLogId + "] does not exist in the system");
        }

        public void UpdateAuditLog(AuditLog auditLog)
        {
            _auditLogRepository.UpdateAsync(auditLog);
        }

        public IQueryable<AuditLog> GetAuditLogForAuditLogClientInformationSheet(ClientInformationSheet clientInformationSheet)
        {
            return _auditLogRepository.FindAll().Where(al => al.AuditLogClientInformationSheet == clientInformationSheet && al.AuditLogClientAgreement == null && al.DateDeleted != null);
        }

        public IQueryable<AuditLog> GetAuditLogForAuditLogClientAgreement(ClientAgreement clientAgreement)
        {
            return _auditLogRepository.FindAll().Where(al => al.AuditLogClientAgreement == clientAgreement && al.DateDeleted != null);
        }
    }
}

