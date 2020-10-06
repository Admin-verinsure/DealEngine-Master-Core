using System;
using System.Linq;
using System.Collections.Generic;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using System.Threading.Tasks;
using NHibernate.Linq;
using Microsoft.AspNetCore.Mvc;

namespace DealEngine.Services.Impl
{
	public class DataService : IDataService
    {
        ISerializerationService _serializerationService;
        
        
        IProgrammeService _programmeService;
        IClientAgreementService _agreementService;

        IMapperSession<ClientProgramme> _programmeRepository;
        
        
        IMapperSession<ClientInformationSheet> _clientSheetRepository;

        public DataService(
            IMapperSession<ClientProgramme> programmeRepository, 
            IMapperSession<ClientInformationSheet> clientSheetRepository, 
            ISerializerationService serializerationService, 
            IProgrammeService programmeService,
            IClientAgreementService agreementService)
		{
            _programmeRepository = programmeRepository;
            _clientSheetRepository = clientSheetRepository;
            _serializerationService = serializerationService;
            _programmeService = programmeService;
            _agreementService = agreementService;
		}

        public async Task<string> GetData(Guid ProgrammeId)
        {

            ClientProgramme programme = await _programmeRepository.GetByIdAsync(ProgrammeId);
            ClientProgramme programme2 = await _programmeService.GetClientProgrammebyId(ProgrammeId);
            
            //GetByIdAsync(ProgrammeId);

            var sheetId = programme.InformationSheet.Id;
            ClientInformationSheet sheet = await _clientSheetRepository.GetByIdAsync(sheetId);

            var jsonObjectList = new List<object>();

            foreach (ClientAgreement agreement in programme.Agreements){
                jsonObjectList.Add(await _agreementService.GetAgreement(agreement.Id));
            }

            //jsonObjectList.Add(sheet);

            string test = await _serializerationService.GetSerializedObject(jsonObjectList);
            System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\dealengine\DealEngine.WebUI\wwwroot\Report\test2.json", test);

            //// clientinformationsheet
            //// clientagreement
            //// organisation
            //// boat
            //// vehicle
            //// boatuse
            //// clientagreementbvterm
            //// clientagreementmvterm
            //// clientagreementterm
            ///



            // What do we want to do? 
            // 1. Just to get the data you want from Sheet at highest level without going deep.


            //
            ////var sheet = programme.Agreements.
            //foreach (ClientAgreement agreement in programme.Agreements)
            //{
            //    jsonObjectList.Add(agreement);
            //}
            //// objects to get

            //jsonObjectList.Add(sheet); //works and is huge
            ////jsonObjectList.Add(agreement); //works and also will be huge
            ////
            ////

            return "test";
        }
    }
}

