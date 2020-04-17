using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IPreRenewOrRefDataService
    {
        Task<PreRenewOrRefData> CreateNewPreRenewOrRefData(PreRenewOrRefData preRenewOrRefData);

        Task DeletePreRenewOrRefData(User deletedBy, PreRenewOrRefData preRenewOrRefData);

        Task<List<PreRenewOrRefData>> GetAllPreRenewOrRefDatas();

        Task<PreRenewOrRefData> GetPreRenewOrRefData(Guid preRenewOrRefDataId);

        Task UpdatePreRenewOrRefData(PreRenewOrRefData preRenewOrRefData);

        Task<List<PreRenewOrRefData>> GetPreRenewOrRefDataForTypeAndRef(string dataType, string refField);

    }
}