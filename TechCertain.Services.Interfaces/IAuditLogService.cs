using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IAuditLogService
    {
        AuditLog CreateNewAuditLog(AuditLog auditLog);

        bool DeleteAuditLog(User deletedBy, AuditLog auditLog);

        IQueryable<AuditLog> GetAllAuditLogs();

        AuditLog GetAuditLog(Guid auditLogId);

        bool UpdateAuditLog(AuditLog auditLog);

        IQueryable<AuditLog> GetAuditLogForAuditLogClientInformationSheet(ClientInformationSheet clientInformationSheet);

        IQueryable<AuditLog> GetAuditLogForAuditLogClientAgreement(ClientAgreement clientAgreement);
    }
}
