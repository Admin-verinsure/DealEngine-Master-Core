﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IEGlobalSubmissionService
    {
        Task<EGlobalSubmission> CreateNewEGlobalSubmission(EGlobalSubmission eGlobalSubmission);

        Task<bool> DeleteEGlobalSubmission(User deletedBy, EGlobalSubmission eGlobalSubmission);

        Task<List<EGlobalSubmission>> GetAllEGlobalSubmissions();

        Task<EGlobalSubmission> GetEGlobalSubmission(Guid eGlobalSubmissionId);

        Task<EGlobalSubmission> GetEGlobalSubmissionByTransaction(Guid transactionreferenceid);

        Task<bool> UpdateEGlobalSubmission(EGlobalSubmission eGlobalSubmission);

    }
}
