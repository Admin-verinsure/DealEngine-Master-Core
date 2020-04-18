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

            await BuildAnswerFromModel(sheet, collection);

            await UpdateInformation(sheet);
        }

        private async Task BuildAnswerFromModel(ClientInformationSheet sheet, IFormCollection collection)
        {
            //find a faster way of getting all models
            AnswerFromRevenue(sheet, collection, collection.Keys.Where(s => s.StartsWith("RevenueDataViewModel", StringComparison.CurrentCulture)));            
            SaveAnswer(sheet, collection, collection.Keys.Where(s => s.StartsWith("PMINZEPLViewModel", StringComparison.CurrentCulture)));
            SaveAnswer(sheet, collection, collection.Keys.Where(s => s.StartsWith("CLIViewModel", StringComparison.CurrentCulture)));
            SaveAnswer(sheet, collection, collection.Keys.Where(s => s.StartsWith("PMINZPIViewModel", StringComparison.CurrentCulture)));
            SaveAnswer(sheet, collection, collection.Keys.Where(s => s.StartsWith("DAOLIViewModel", StringComparison.CurrentCulture)));
            SaveAnswer(sheet, collection, collection.Keys.Where(s => s.StartsWith("ClaimsHistoryViewModel", StringComparison.CurrentCulture)));
            SaveAnswer(sheet, collection, collection.Keys.Where(s => s.StartsWith("DAOLIViewModel", StringComparison.CurrentCulture)));
        }

        private void SaveAnswer(ClientInformationSheet sheet, IFormCollection collection, IEnumerable<string> enumerable)
        {
            foreach (var key in enumerable)
            {
                sheet.AddAnswer(key, collection[key]);
            }
        }

        private void AnswerFromRevenue(ClientInformationSheet sheet, IFormCollection collection, IEnumerable<string> enumerable)
        {
            sheet.RevenueData = new RevenueData(sheet);
            string modelLocation = "DealEngine.WebUI.Models.{1}, DealEngine.WebUI";
            // get activity/revenue data            
            foreach (string key in enumerable)
            {
                int value = 0;
                var modelArray = key.Split('.').ToList();
                Guid id = Guid.Empty;
                var modelType = modelLocation.Replace("{1}", modelArray.FirstOrDefault());
                Type type = Type.GetType(modelType);
                try
                {
                    int percent = 0;
                    var model = Activator.CreateInstance(type);
                    var ModelProperty = model.GetType().GetProperty(modelArray.ElementAt(1));
                    if (ModelProperty.Name == "Territories")
                    {
                        Territory territory;
                        if (modelArray.Count > 2)
                        {
                            Guid.TryParse(modelArray.ElementAt(2), out id);
                            territory = sheet.RevenueData.Territories.FirstOrDefault(t => t.TemplateId == id);
                            try
                            {
                                territory.Selected = true;
                                territory.Percentage = decimal.Parse(collection[key].ToString());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                    else if (ModelProperty.Name == "Activities")
                    {
                        BusinessActivity activity;
                        if (modelArray.Count > 2)
                        {
                            activity = sheet.RevenueData.Activities.FirstOrDefault(t => t.AnzsciCode == modelArray.ElementAt(2));
                            try
                            {
                                activity.Selected = true;
                                activity.Percentage = decimal.Parse(collection[key].ToString());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                    else if (ModelProperty.Name == "AdditionalActivityViewModel")
                    {
                        var variabletype = sheet.RevenueData.AdditionalActivityInformation.GetType();
                        var field = variabletype.GetProperty(modelArray.LastOrDefault());

                        if (field.PropertyType.Name == "Decimal")
                        {
                            var total = decimal.Parse(collection[key].ToString());
                            field.SetValue(sheet.RevenueData.AdditionalActivityInformation, total);
                        }
                        else
                        {
                            field.SetValue(sheet.RevenueData.AdditionalActivityInformation, collection[key].ToString());
                        }
                    }
                    else if (ModelProperty.PropertyType.Name == "Decimal")
                    {
                        var variabletype = sheet.RevenueData.GetType();
                        var field = variabletype.GetProperty(ModelProperty.Name);
                        var total = decimal.Parse(collection[key].ToString());
                        field.SetValue(sheet.RevenueData, total);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
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

