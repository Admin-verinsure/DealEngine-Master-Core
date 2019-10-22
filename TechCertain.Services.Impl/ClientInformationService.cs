using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace TechCertain.Services.Impl
{
    public class ClientInformationService : IClientInformationService
    {
        IMapperSession<ClientInformationSheet> _customerInformationRepository;

        public ClientInformationService(IMapperSession<ClientInformationSheet> customerInformationRepository)
        {
            _customerInformationRepository = customerInformationRepository;
        }

        #region ICustomerInformationService implementation

        public async Task<ClientInformationSheet> IssueInformationFor(User createdBy, Organisation createdFor, InformationTemplate informationTemplate)
        {
            ClientInformationSheet sheet = new ClientInformationSheet(createdBy, createdFor, informationTemplate);
            await UpdateInformation(sheet);
            return sheet;
        }

        public async Task<ClientInformationSheet> IssueInformationFor(User createdBy, Organisation createdFor, ClientProgramme clientProgramme, string reference)
        {
            if (clientProgramme.InformationSheet != null)
                throw new Exception("ClientProgramme [" + clientProgramme.Id + "] already has an InformationSheet assigned");

            ClientInformationSheet sheet = new ClientInformationSheet(createdBy, createdFor, clientProgramme.BaseProgramme.Products.First().InformationTemplate, reference);

            clientProgramme.InformationSheet = sheet;
            sheet.Programme = clientProgramme;
            await _customerInformationRepository.AddAsync(sheet);            
            return sheet;
        }

        public async Task<ClientInformationSheet> GetInformation(Guid informationSheetId)
        {
            return await _customerInformationRepository.GetByIdAsync(informationSheetId);
        }

        public async Task<List<ClientInformationSheet>> GetAllInformationFor(User owner)
        {
            return await _customerInformationRepository.FindAll().Where(s => owner.Organisations.Contains(s.Owner)).ToListAsync();
        }

        public async Task<List<ClientInformationSheet>> GetAllInformationFor(Organisation owner)
        {
            return await _customerInformationRepository.FindAll().Where(s => s.Owner == owner).ToListAsync();
        }

        public async Task<List<ClientInformationSheet>> GetAllInformationFor(String referenceId)
        {
            return await _customerInformationRepository.FindAll().Where(s => s.ReferenceId == referenceId).ToListAsync();
        }

        public async Task UpdateInformation(ClientInformationSheet sheet)
        {
            await _customerInformationRepository.UpdateAsync(sheet);
        }

        public async Task SaveAnswersFor(ClientInformationSheet sheet, IFormCollection collection)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (var key in collection.Keys)
            {
                foreach (string value in collection[key])
                {
                    sheet.AddAnswer(key, value);
                }
            }

            // get activity/revenue data
            var activityRevenue = collection.Keys.Where(s => s.StartsWith("actRevMat", StringComparison.CurrentCulture));
            NameValueCollection activityRevenueData = new NameValueCollection();
            foreach (string key in activityRevenue)
                activityRevenueData.Add(key, collection[key].FirstOrDefault());
            await SaveRevenueData(sheet, activityRevenueData, null);

            // get shared data
            var sharedKeys = collection.Keys.Where(s => s.StartsWith("shared", StringComparison.CurrentCulture));
            NameValueCollection sharedData = new NameValueCollection();
            foreach (string key in sharedKeys)
                sharedData.Add(key, collection[key].FirstOrDefault());

            // setup the shared data just in case it doesn't exist for some reasons
            //ConfigureSharedData (sheet);

            // save data to shared data
            if (sheet.SharedData != null)
            {
                sheet.SharedData.SetData(sharedData);
            }
        }

        #endregion

        //void ConfigureSharedData(ClientInformationSheet sheet)
        //{
        //    // if not shared data object on UIS
        //    if (sheet.SharedData == null)
        //    {
        //        // assume we don't have any shared data
        //        var hasSharedData = false;
        //        // check products for other UIS's, and see if they have a shared data object
        //        foreach (var otherSheet in GetAllInformationFor(sheet.Owner))
        //        {
        //            if (sheet.Product.ProductPackage.Contains(otherSheet.Product))
        //            {
        //                hasSharedData = true;
        //                sheet.SharedData = otherSheet.SharedData;
        //                break;
        //            }
        //        }
        //        // if no shared data object, create one
        //        if (!hasSharedData)
        //            sheet.SharedData = new ClientSharedData(sheet.CreatedBy, sheet.Product.ProductPackage);
        //    }
        //}

        async Task SaveRevenueData(ClientInformationSheet sheet, NameValueCollection revenueData, User creatingUser)
        {
            foreach (string key in revenueData.Keys)
            {
                string[] parts = key.Split('_');

                RevenueByActivity activityRevenue = sheet.RevenueData.FirstOrDefault(rd => rd.Activity == parts[1]);
                if (activityRevenue == null)
                {
                    activityRevenue = new RevenueByActivity(creatingUser)
                    {
                        Activity = parts[1],
                        RevenueByCountry = new List<RevenueByCountry>()
                    };
                    sheet.RevenueData.Add(activityRevenue);
                }
                RevenueByCountry countryRevenue = activityRevenue.RevenueByCountry.FirstOrDefault(rc => rc.Country == parts[2]);
                if (countryRevenue == null)
                {
                    countryRevenue = new RevenueByCountry(creatingUser)
                    {
                        Country = parts[2]
                    };
                    activityRevenue.RevenueByCountry.Add(countryRevenue);
                }
                countryRevenue.DeclaredRevenue = Convert.ToDecimal(revenueData.Get(key));
            }
        }
    }
}

