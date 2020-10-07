using System;
using System.Linq;
using System.Collections.Generic;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using System.Threading.Tasks;
using NHibernate.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;


namespace DealEngine.Services.Impl
{
	public class DataService : IDataService
    {
        ISerializerationService _serializerationService;
        IProgrammeService _programmeService;
        IClientAgreementService _agreementService;
        IMapperSession<ClientProgramme> _programmeRepository;               
        IMapperSession<ClientInformationSheet> _clientSheetRepository;
        IMapperSession<Data> _dataRepository;
        private readonly IHostingEnvironment _hostingEnv;
        IApplicationLoggingService _applicationLoggingService;
        ILogger<Data> _logger;

        public DataService(
            IMapperSession<ClientProgramme> programmeRepository,
            IMapperSession<ClientInformationSheet> clientSheetRepository,
            ISerializerationService serializerationService,
            IProgrammeService programmeService,
            IClientAgreementService agreementService,
            IHostingEnvironment hostingEnv,
            IMapperSession<Data> dataRepository,
            IApplicationLoggingService applicationLoggingService,
            ILogger<Data> logger
            )
		{
            _programmeRepository = programmeRepository;
            _clientSheetRepository = clientSheetRepository;
            _serializerationService = serializerationService;
            _programmeService = programmeService;
            _agreementService = agreementService;
            _dataRepository = dataRepository;
            _hostingEnv = hostingEnv;
            _applicationLoggingService = applicationLoggingService;
		}

        public async Task<Data> Add (User user)
        {
            Data clientData = new Data(user);
            await _dataRepository.AddAsync(clientData);
            return clientData;
        }

        public async Task<Data> Update (Data data, Guid clientProgrammeId, string bindType)
        {
            //User user = await CurrentUser();
            ClientProgramme clientProgramme = await _programmeService.GetClientProgrammebyId(clientProgrammeId);
            DateTime date = DateTime.Today;

            // common attributes 
            data.BindType = bindType;
            data.FileName = clientProgramme.Id.ToString() + bindType + ".json";
            //data.BindType = "NoType";
            data.AgreementDate = date;
            data.ClientName = clientProgramme.Owner.Name.ToString();
            try
            {
                data.Brokerage = clientProgramme.Agreements.FirstOrDefault().ClientAgreementTerms.FirstOrDefault().Brokerage.ToString();
                data.Coy = (clientProgramme.Agreements.FirstOrDefault().ClientAgreementTerms.FirstOrDefault().Premium - clientProgramme.Agreements.FirstOrDefault().ClientAgreementTerms.FirstOrDefault().FSL).ToString();
                data.Excess = clientProgramme.Agreements.FirstOrDefault().ClientAgreementTerms.FirstOrDefault().Excess.ToString();
                data.TotalPremium = clientProgramme.Agreements.FirstOrDefault().ClientAgreementTerms.FirstOrDefault().Premium.ToString();
                data.GST = (clientProgramme.Agreements.FirstOrDefault().ClientAgreementTerms.FirstOrDefault().Premium * clientProgramme.Agreements.FirstOrDefault().Product.TaxRate).ToString();
                data.TotalSumInsured = clientProgramme.Agreements.FirstOrDefault().ClientAgreementTerms.FirstOrDefault().TermLimit.ToString();
                data.FENZ = clientProgramme.Agreements.FirstOrDefault().ClientAgreementTerms.FirstOrDefault().FSL.ToString();

                // programme specific attributes
                if (clientProgramme.InformationSheet.Boats != null)
                {
                    // TODO if dataBoats already exist we need to update the DataBoats rather than create new ones each time???
                    IList<DataBoat> dataBoats = new List<DataBoat>();
                    foreach (Boat b in clientProgramme.InformationSheet.Boats)
                    {
                        DataBoat dataBoat = new DataBoat();
                        dataBoat.Year = b.YearOfManufacture.ToString();
                        dataBoat.Make = b.BoatMake;
                        dataBoat.Model = b.BoatModel;
                        dataBoat.Type = b.BoatType2;
                        dataBoat.Construction = b.HullConstruction;
                        if (b.BoatLandLocation != null)
                        {
                            string location = b.BoatLandLocation.Location.LocationType + ", " + b.BoatLandLocation.Location.Street + ", " + b.BoatLandLocation.Location.Suburb + ", " + b.BoatLandLocation.Location.City;
                            dataBoat.Location = location;
                        }
                        dataBoat.SumInsured = b.Sum.ToString();
                        dataBoat.Hull = b.HullConfiguration;
                        if (b.BoatTrailers.FirstOrDefault() != null)
                        {
                            string trailer = b.BoatTrailers.FirstOrDefault().Year + ", " + b.BoatTrailers.FirstOrDefault().Make + ", " + b.BoatTrailers.FirstOrDefault().Model + ", " + b.BoatTrailers.FirstOrDefault().GroupSumInsured.ToString() + ", " + b.BoatTrailers.FirstOrDefault().VehicleEffectiveDate.ToString();
                            dataBoat.Trailer = trailer;
                        }
                        if (b.BoatUses.FirstOrDefault() != null)
                        {
                            if (b.BoatUses.FirstOrDefault().BoatUseRace != null)
                            {
                                dataBoat.RacingRisk = b.BoatUses.FirstOrDefault().BoatUseRace;
                            }
                        }
                        dataBoats.Add(dataBoat);
                    }
                    data.Boats = dataBoats;
                }

            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, null, null);
            }

            await _dataRepository.AddAsync(data);

            return data;
        }

        public async Task<Data> ToJson (Data data, string dataTemplate, Guid clientProgrammeId)
        {
            // If specific values aren't needed which exist in Data you can alter it using dataTemplate below before serialization
            ClientProgramme clientProgramme = await _programmeService.GetClientProgrammebyId(clientProgrammeId);
            var path = _hostingEnv.WebRootPath + "\\Data\\" + clientProgramme.BaseProgramme.Id;

            // Create folder if it doesn't exist
            if (!(Directory.Exists(path)))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = path + "\\" + data.FileName;

            data.FullPath = filepath;
            await _dataRepository.AddAsync(data);

            var json = await _serializerationService.GetSerializedObject(data);
            
            System.IO.File.WriteAllText(filepath, json);
            return data;
        }
    }
}