using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DealEngine.Services.Interfaces
{
    public interface IDataService
    {
        Task<string> GetData(Guid ProgrammeId);
    }
}
