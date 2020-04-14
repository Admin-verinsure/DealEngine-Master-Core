using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using NHibernate.Linq;
using AutoMapper;

namespace DealEngine.Services.Impl
{
    public class ClientInformationService : IClientInformationService
    {
        IMapperSession<ClientInformationSheet> _customerInformationRepository;
        IMapperSession<Boat> _boatRepository;
        IMapper _mapper;

        public ClientInformationService(
            IMapperSession<ClientInformationSheet> customerInformationRepository, 
            IMapperSession<Boat> boatRepository,
            IMapper mapper
            )
        {
            _mapper = mapper;
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
            var list = new List<ClientInformationSheet>();
            var sheetList = await _customerInformationRepository.FindAll().Where(s => owner.Organisations.Contains(s.Owner)).ToListAsync();
            foreach(var sheet in sheetList)
            {
                var isBaseClass = await IsBaseClass(sheet);
                if (isBaseClass)
                {
                    list.Add(sheet);
                }
            }
            return list;
        }

        public async Task<List<ClientInformationSheet>> GetAllInformationFor(Organisation owner)
        {
            var list = new List<ClientInformationSheet>();
            var sheetList = await _customerInformationRepository.FindAll().Where(s => s.Owner == owner).ToListAsync();
            foreach (var sheet in sheetList)
            {
                var isBaseClass = await IsBaseClass(sheet);
                if (isBaseClass)
                {
                    list.Add(sheet);
                }
            }
            return list;
        }

        public async Task<List<ClientInformationSheet>> GetAllInformationFor(String referenceId)
        {
            var list = new List<ClientInformationSheet>();
            var sheetList = await _customerInformationRepository.FindAll().Where(s => s.ReferenceId == referenceId).ToListAsync();
            foreach (var sheet in sheetList)
            {
                var isBaseClass = await IsBaseClass(sheet);
                if (isBaseClass)
                {
                    list.Add(sheet);
                }
            }
            return list;
        }

        public async Task<bool> IsBaseClass(ClientInformationSheet sheet)
        {
            var objectType = sheet.GetType();
            if (!objectType.IsSubclassOf(typeof(ClientInformationSheet)))
            {
                return true;
            }
            return false;
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
                //foreach (string value in collection[key])
                //{
                    //break the collection into objects                    
                    sheet.AddAnswer(key, collection[key]);
                //}
            }

            // get activity/revenue data
            var activityRevenue = collection.Keys.Where(s => s.StartsWith("RevenueDataViewModel", StringComparison.CurrentCulture));
            NameValueCollection activityRevenueData = new NameValueCollection();
            foreach (string key in activityRevenue)
            {
                var value = collection[key];

                var modeltype = typeof(RevenueDataViewModel).GetProperty(split.FirstOrDefault());
                var infomodel = modeltype.GetValue(model);
                var property = infomodel.GetType().GetProperty(split.LastOrDefault());

                switch (property.PropertyType.Name)
                {
                    case "Int32":
                        int.TryParse(answer.Value, out value);
                        property.SetValue(infomodel, value);
                        break;
                    case "IList`1":
                        var propertylist = (IList<SelectListItem>)property.GetValue(infomodel);
                        var options = answer.Value.Split(',').ToList();
                        foreach (var option in options)
                        {
                            propertylist.FirstOrDefault(i => i.Value == option).Selected = true;
                        }
                        property.SetValue(infomodel, propertylist);
                        break;
                    case "DateTime":
                        property.SetValue(infomodel, DateTime.Parse(answer.Value));
                        break;
                    default:
                        property.SetValue(infomodel, answer.Value);
                        break;
                }
            }
                activityRevenueData.Add(key, collection[key].FirstOrDefault());
            await SaveRevenueData(sheet, activityRevenueData);

            // get shared data
            var sharedKeys = collection.Keys.Where(s => s.StartsWith("shared", StringComparison.CurrentCulture));
            NameValueCollection sharedData = new NameValueCollection();
            foreach (string key in sharedKeys)
                sharedData.Add(key, collection[key].FirstOrDefault());

            // setup the shared data just in case it doesn't exist for some reasons
            //ConfigureSharedData (sheet);

            // save data to shared data
        }

        private Task SaveRevenueData(ClientInformationSheet sheet, NameValueCollection activityRevenueData)
        {
            RevenueData revenueData;
            try
            {
                revenueData = _mapper.Map<RevenueData>(activityRevenueData);
            }
            catch(Exception ex)
            {
                revenueData = null;
                Console.WriteLine(ex.Message);
            }
            
            sheet.RevenueData = revenueData;            
            throw new NotImplementedException();
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

        public async Task<SubClientInformationSheet> IssueSubInformationFor(ClientInformationSheet clientInformationSheet)
        {
            SubClientInformationSheet sheet = _mapper.Map<SubClientInformationSheet>(clientInformationSheet);
            return sheet;
        }
    }
}

