using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task<AuditLog> CreateNewAuditLog(AuditLog auditLog);

        Task DeleteAuditLog(User deletedBy, AuditLog auditLog);

        Task<List<AuditLog>> GetAllAuditLogs();

        Task<AuditLog> GetAuditLog(Guid auditLogId);

        Task UpdateAuditLog(AuditLog auditLog);

        Task<List<AuditLog>> GetAuditLogForAuditLogClientInformationSheet(ClientInformationSheet clientInformationSheet);

        Task<List<AuditLog>> GetAuditLogForAuditLogClientAgreement(ClientAgreement clientAgreement);
    }
}
