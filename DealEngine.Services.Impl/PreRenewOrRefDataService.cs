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
    public class PreRenewOrRefDataService : IPreRenewOrRefDataService
    {
        IMapperSession<PreRenewOrRefData> _preRenewOrRefDataRepository;

        public PreRenewOrRefDataService(IMapperSession<PreRenewOrRefData> preRenewOrRefDataRepository)
        {
            _preRenewOrRefDataRepository = preRenewOrRefDataRepository;
        }

        public async Task<PreRenewOrRefData> CreateNewPreRenewOrRefData(PreRenewOrRefData preRenewOrRefData)
        {
            await UpdatePreRenewOrRefData(preRenewOrRefData);
            return preRenewOrRefData;
        }

        public async Task DeletePreRenewOrRefData(User deletedBy, PreRenewOrRefData preRenewOrRefData)
        {
            preRenewOrRefData.Delete(deletedBy);
            await UpdatePreRenewOrRefData(preRenewOrRefData);
        }

        public async Task<List<PreRenewOrRefData>> GetAllPreRenewOrRefDatas()
        {
            return await _preRenewOrRefDataRepository.FindAll().Where(al => al.DateDeleted != null).ToListAsync();
        }

        public async Task<PreRenewOrRefData> GetPreRenewOrRefData(Guid preRenewOrRefDataId)
        {
            PreRenewOrRefData preRenewOrRefData = await _preRenewOrRefDataRepository.GetByIdAsync(preRenewOrRefDataId);
            if (preRenewOrRefData != null)
                return preRenewOrRefData;
            if (preRenewOrRefData != null)
            {
                await UpdatePreRenewOrRefData(preRenewOrRefData);
                return preRenewOrRefData;
            }
            throw new Exception("PreRenewOrRefData with id [" + preRenewOrRefDataId + "] does not exist in the system");
        }

        public async Task UpdatePreRenewOrRefData(PreRenewOrRefData preRenewOrRefData)
        {
            await _preRenewOrRefDataRepository.UpdateAsync(preRenewOrRefData);
        }

        public async Task<List<PreRenewOrRefData>> GetPreRenewOrRefDataForTypeAndRef(string dataType, string refField)
        {
            return await _preRenewOrRefDataRepository.FindAll().Where(prord => prord.DataType == dataType && prord.RefField == refField && prord.DateDeleted != null).ToListAsync();
        }


    }
}

