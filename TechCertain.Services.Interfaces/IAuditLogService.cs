using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IAuditLogService
    {
        AuditLog CreateNewAuditLog(AuditLog auditLog);

        void DeleteAuditLog(User deletedBy, AuditLog auditLog);

        IQueryable<AuditLog> GetAllAuditLogs();

        AuditLog GetAuditLog(Guid auditLogId);

        void UpdateAuditLog(AuditLog auditLog);

        IQueryable<AuditLog> GetAuditLogForAuditLogClientInformationSheet(ClientInformationSheet clientInformationSheet);

        IQueryable<AuditLog> GetAuditLogForAuditLogClientAgreement(ClientAgreement clientAgreement);
    }
}
