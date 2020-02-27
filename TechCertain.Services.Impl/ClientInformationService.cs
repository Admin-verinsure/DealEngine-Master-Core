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
        IMapperSession<SubClientInformationSheet> _customerSubInformationRepository;
        IMapperSession<Boat> _boatRepository;

        public ClientInformationService(IMapperSession<ClientInformationSheet> customerInformationRepository, 
            IMapperSession<Boat> boatRepository,
            IMapperSession<SubClientInformationSheet> customerSubInformationRepository
            )
        {
            _customerSubInformationRepository = customerSubInformationRepository;
            _customerInformationRepository = customerInformationRepository;
            _boatRepository = boatRepository;
        }

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

            ClientInformationSheet sheet = new ClientInformationSheet(createdBy, createdFor, clientProgramme.BaseProgramme.Products.FirstOrDefault().InformationTemplate, reference);
            
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
            //await SaveRevenueData(sheet, activityRevenueData, null);

            // get shared data
            var sharedKeys = collection.Keys.Where(s => s.StartsWith("shared", StringComparison.CurrentCulture));
            NameValueCollection sharedData = new NameValueCollection();
            foreach (string key in sharedKeys)
                sharedData.Add(key, collection[key].FirstOrDefault());

            // setup the shared data just in case it doesn't exist for some reasons
            //ConfigureSharedData (sheet);

            // save data to shared data
        }

        public async Task<List<ClientInformationSheet>> FindByBoatName(string searchValue)
        {
            var clientList = new List<ClientInformationSheet>();
            var boats = await _boatRepository.FindAll().Where(b => b.BoatName ==searchValue).ToListAsync();
            foreach(var boat in boats)
            {
                clientList.AddRange(_customerInformationRepository.FindAll().Where(c => c.Boats.Contains(boat)).ToList());
            }
            return clientList;
        }

        public async Task<SubClientInformationSheet> IssueSubInformationFor(SubClientProgramme subClientProgramme)
        {
            if (subClientProgramme.InformationSheet != null)
                throw new Exception("ClientProgramme [" + subClientProgramme.Id + "] already has an InformationSheet assigned");

            SubClientInformationSheet sheet = new SubClientInformationSheet(subClientProgramme.InformationSheet);
            sheet.CopyClientInformationSheet(subClientProgramme);
            subClientProgramme.InformationSheet = sheet;

            await _customerSubInformationRepository.AddAsync(sheet);
            return sheet;
        }
    }
}

