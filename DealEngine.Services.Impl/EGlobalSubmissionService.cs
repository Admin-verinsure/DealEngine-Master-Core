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
    public class EGlobalSubmissionService : IEGlobalSubmissionService
    {
        IMapperSession<EGlobalSubmission> _eGlobalSubmissionRepository;

        public EGlobalSubmissionService(IMapperSession<EGlobalSubmission> eGlobalSubmissionRepository)
        {
            _eGlobalSubmissionRepository = eGlobalSubmissionRepository;
        }

        public async Task<EGlobalSubmission> CreateNewEGlobalSubmission(EGlobalSubmission eGlobalSubmission)
        {
            await UpdateEGlobalSubmission(eGlobalSubmission);
            return eGlobalSubmission;
        }

        public async Task<bool> DeleteEGlobalSubmission(User deletedBy, EGlobalSubmission eGlobalSubmission)
        {
            eGlobalSubmission.Delete(deletedBy);
            return await UpdateEGlobalSubmission(eGlobalSubmission);
        }

        public async Task<List<EGlobalSubmission>> GetAllEGlobalSubmissions()
        {
            return await _eGlobalSubmissionRepository.FindAll().Where(egs => egs.DateDeleted == null).ToListAsync();
        }

        public async Task<EGlobalSubmission> GetEGlobalSubmission(Guid eGlobalSubmissionId)
        {
            EGlobalSubmission eGlobalSubmission = await _eGlobalSubmissionRepository.GetByIdAsync(eGlobalSubmissionId);
            if (eGlobalSubmission != null)
                return eGlobalSubmission;
            if (eGlobalSubmission != null)
            {
                await UpdateEGlobalSubmission(eGlobalSubmission);
                return eGlobalSubmission;
            }
            throw new Exception("EGlobalSubmission with id [" + eGlobalSubmissionId + "] does not exist in the system");
        }

        public async Task<EGlobalSubmission> GetEGlobalSubmissionByTransaction(Guid transactionreferenceid)
        {
            EGlobalSubmission eGlobalSubmission = await _eGlobalSubmissionRepository.FindAll().FirstOrDefaultAsync(egbt => egbt.DateDeleted == null && egbt.TransactionReferenceID == transactionreferenceid);
            if (eGlobalSubmission != null) {
                return eGlobalSubmission;
            } else
            {
                throw new Exception("EGlobalSubmission with Transaction ID [" + transactionreferenceid + "] does not exist in the system");
            }
        }

        public async Task<bool> UpdateEGlobalSubmission(EGlobalSubmission eGlobalSubmission)
        {
            await _eGlobalSubmissionRepository.UpdateAsync(eGlobalSubmission);
            return true;
        }


    }
}

