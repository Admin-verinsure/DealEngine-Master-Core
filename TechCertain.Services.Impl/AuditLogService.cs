using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class AuditLogService : IAuditLogService
    {
        IUnitOfWorkFactory _unitOfWork;
        ILogger _logging;
        IRepository<AuditLog> _auditLogRepository;

        public AuditLogService(IUnitOfWorkFactory unitOfWork, ILogger logging, IRepository<AuditLog> auditLogRepository)
        {
            _unitOfWork = unitOfWork;
            _logging = logging;
            _auditLogRepository = auditLogRepository;
        }

        public AuditLog CreateNewAuditLog(AuditLog auditLog)
        {
            UpdateAuditLog(auditLog);
            return auditLog;
        }

        public bool DeleteAuditLog(User deletedBy, AuditLog auditLog)
        {
            auditLog.Delete(deletedBy);
            return UpdateAuditLog(auditLog);
        }

        public IQueryable<AuditLog> GetAllAuditLogs()
        {
            return _auditLogRepository.FindAll().Where(al => al.DateDeleted != null);
        }

        public AuditLog GetAuditLog(Guid auditLogId)
        {
            AuditLog auditLog = _auditLogRepository.GetById(auditLogId);
            if (auditLog != null)
                return auditLog;
            if (auditLog != null)
            {
                UpdateAuditLog(auditLog);
                return auditLog;
            }
            throw new Exception("AuditLog with id [" + auditLogId + "] does not exist in the system");
        }

        public bool UpdateAuditLog(AuditLog auditLog)
        {
            _auditLogRepository.Add(auditLog);
            return true;
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

