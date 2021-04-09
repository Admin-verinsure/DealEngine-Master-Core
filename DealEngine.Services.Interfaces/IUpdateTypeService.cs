using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Http;


namespace DealEngine.Services.Interfaces
{
    public interface IUpdateTypeService
    {
        Task<List<UpdateType>> GetAllUpdateTypes();
        Task AddUpdateType(User createdBy,string typeName, string typeValue, bool typeIsTc, bool typeIsBroker, bool typeIsInsurer, bool typeIsClient);
        //Task AddUpdateType(Guid updateTypeId, Guid programmeId);

        Task<UpdateType> GetUpdateType(Guid updateTypeId);
        //Task Update(UpdateType updateType);

    }
}
