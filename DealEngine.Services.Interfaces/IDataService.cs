using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DealEngine.Services.Interfaces
{
    public interface IDataService
    {
        Task<Data> Add(User user);

        Task<Data> Update(Data data, Guid clientProgrammeId, string bindType);

        Task<Data> ToJson(Data data, string dataTemplate, Guid clientProgrammeId);
    }
}
