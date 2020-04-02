﻿using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DealEngine.Services.Impl
{
    public class ImportService : IImportService
    {
        IOrganisationService _organisationService;
        IUserService _userService;
        IProgrammeService _programmeService;
        IReferenceService _referenceService;
        IClientInformationService _clientInformationService;
        IUnitOfWork _unitOfWork;
        IOrganisationTypeService _organisationTypeService;
        IMapperSession<Organisation> _organisationRepository;
        IBusinessActivityService _businessActivityService;
        IInsuranceAttributeService _InsuranceAttributeService;
        private readonly string WorkingDirectory;

        public ImportService(
            IOrganisationService organisationService, 
            IUserService userService,
            IProgrammeService programmeService, 
            IReferenceService referenceService, 
            IClientInformationService clientInformationService,
            IUnitOfWork unitOfWork, 
            IOrganisationTypeService organisationTypeService, 
            IInsuranceAttributeService insuranceAttributeService,
            IMapperSession<Organisation> organisationRepository, 
            IBusinessActivityService businessActivityService)
        {
            WorkingDirectory = "C://tmp//"; //"/tmp/ImportData/"; 
            _businessActivityService = businessActivityService;
            _organisationRepository = organisationRepository;
            _InsuranceAttributeService = insuranceAttributeService;
            _organisationTypeService = organisationTypeService;
            _organisationService = organisationService;
            _userService = userService;
            _programmeService = programmeService;
            _referenceService = referenceService;
            _clientInformationService = clientInformationService;
            _unitOfWork = unitOfWork;
        }

        public async Task ImportAOEServiceIndividuals(User CreatedUser)
        {
            //addresses need to be on one line
            //var userFileName = "C:\\tmp\\testclientdata\\NZACSUsers2018.csv";
            var userFileName = "/tmp/NZACSUsers2018.csv";
            var currentUser = CreatedUser;
            Guid programmeID = Guid.Parse("214efe24-552d-46a3-a666-6bede7c88ca1");
            StreamReader reader;
            User user = null;
            Organisation organisation = null;
            bool readFirstLine = false;
            string line;
            using (reader = new StreamReader(userFileName))
            {
                while (!reader.EndOfStream)
                {
                    //if has a title row
                    //if (!readFirstLine)
                    //{
                    //    line = reader.ReadLine();
                    //    readFirstLine = true;
                    //}
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    user = null;
                    organisation = null;
                    try
                    {
                        if (parts[0] == "f")
                        {
                            if (!string.IsNullOrWhiteSpace(parts[4]))
                            {
                                organisation = await _organisationService.GetOrganisationByEmail(parts[4]);
                            }
                            if (organisation == null)
                            {
                                var organisationType = await _organisationTypeService.GetOrganisationTypeByName("Person - Individual");
                                organisation = new Organisation(currentUser, Guid.NewGuid(), parts[2] + " " + parts[3], organisationType, parts[4]);
                                await _organisationService.CreateNewOrganisation(organisation);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(parts[4]))
                            {
                                organisation = await _organisationService.GetOrganisationByEmail(parts[4]);
                            }
                            if (organisation == null)
                            {
                                var organisationType = await _organisationTypeService.GetOrganisationTypeByName("Corporation – Limited liability");
                                organisation = new Organisation(currentUser, Guid.NewGuid(), parts[1], organisationType, parts[4]);
                                await _organisationService.CreateNewOrganisation(organisation);
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(parts[4]))
                        {
                            var email = parts[7] + "@DealEngine.com";
                            user = await _userService.GetUserByEmail(email);
                        }                            
                        if(user == null)
                        {
                            user = new User(currentUser, Guid.NewGuid(), parts[7]);
                        }
                                                
                        user.FirstName = parts[2];
                        user.LastName = parts[3];
                        user.FullName = parts[2] + " " + parts[3];
                        user.Email = parts[4];
                        user.Address = parts[5];
                        user.Phone = "12345";

                        if (!user.Organisations.Contains(organisation))
                            user.Organisations.Add(organisation);
                        user.SetPrimaryOrganisation(organisation);

                        await _userService.ApplicationCreateUser(user);

                        var programme = await _programmeService.GetProgramme(programmeID);
                        var clientProgramme = await _programmeService.CreateClientProgrammeFor(programme.Id, user, organisation);

                        var reference = await _referenceService.GetLatestReferenceId();
                        var sheet = await _clientInformationService.IssueInformationFor(user, organisation, clientProgramme, reference);
                        await _referenceService.CreateClientInformationReference(sheet);

                        using (var uow = _unitOfWork.BeginUnitOfWork())
                        {

                            clientProgramme.BrokerContactUser = programme.BrokerContactUser;
                            clientProgramme.ClientProgrammeMembershipNumber = parts[6];
                            sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(user, sheet, null, programme.Name + "UIS issue Process Completed"));
                            try
                            {
                                await uow.Commit();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        public async Task ImportAOEServicePrincipals(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            User user = null;
            Organisation organisation = null;
            bool readFirstLine = false;
            string line;
            var email = "";

            //addresses need to be on one line
            //var principalsFileName = "C:\\tmp\\testclientdata\\NZACSPrincipals2018.csv";
            var principalsFileName = "/tmp/NZACSPrincipals2018.csv";
            var insuranceAttribute = await _InsuranceAttributeService.GetInsuranceAttributeByName("Principal");
            var organisationType = await _organisationTypeService.GetOrganisationTypeByName("Person - Individual");
            if (organisationType == null)
            {
                organisationType = await _organisationTypeService.CreateNewOrganisationType(null, "Person - Individual");
            }
            using (reader = new StreamReader(principalsFileName))
            {
                while (!reader.EndOfStream)
                {
                    //if has a title row
                    //if (!readFirstLine)
                    //{
                    //    line = reader.ReadLine();
                    //    readFirstLine = true;
                    //}
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    user = null;
                    organisation = null;
                    try
                    {
                        var hasProgramme = await _programmeService.HasProgrammebyMembership(parts[1]);
                        if (hasProgramme)
                        {
                            string userName = parts[4] + "_" + parts[3];

                            if (string.IsNullOrWhiteSpace(parts[5]))
                            {
                                email = parts[2] + "@DealEngine.com";
                            }
                            else
                            {
                                email = parts[5];
                            }
                            
                            organisation = new Organisation(currentUser, Guid.NewGuid(), parts[2], organisationType, email);                            
                            organisation.InsuranceAttributes.Add(insuranceAttribute);
                            organisation.NZIAmembership = parts[1];
                            organisation.Email = email;
                            organisation.Phone = "12345";

                            if (!string.IsNullOrEmpty(parts[6]))
                            {
                                organisation.Qualifications = parts[6];
                            }
                            if (!string.IsNullOrEmpty(parts[7]))
                            {
                                if (parts[7] == "1")
                                {
                                    organisation.IsNZIAmember = true;
                                }
                                else
                                    organisation.IsNZIAmember = false;
                            }
                            if (!string.IsNullOrEmpty(parts[8]))
                            {
                                if (parts[8] == "1")
                                {
                                    organisation.IsADNZmember = true;
                                }
                                else
                                    organisation.IsADNZmember = false;
                            }
                            //clarify correct field
                            if (!string.IsNullOrEmpty(parts[9]))
                            {
                                if (parts[9] == "1")
                                {
                                    organisation.IsOtherdirectorship = true;
                                }
                                else
                                    organisation.IsOtherdirectorship = false;
                            }

                            using (var uom = _unitOfWork.BeginUnitOfWork())
                            {
                                insuranceAttribute.IAOrganisations.Add(organisation);
                                try
                                {
                                    await uom.Commit();
                                }
                                catch (Exception ex)
                                {
                                    await uom.Rollback();
                                }
                            }

                            await _organisationService.CreateNewOrganisation(organisation);
                            await _programmeService.AddOrganisationByMembership(organisation);
                                                       
                            user = await _userService.GetUserByEmail(email);                            
                            
                            if (user == null)
                            {
                                userName = parts[4] + "_" + parts[3];
                                try
                                {
                                    user = await _userService.GetUser(userName);
                                }                                
                                catch(Exception ex)
                                {
                                    Random random = new Random();
                                    int randomNumber = random.Next(10, 99);
                                    userName = userName + randomNumber.ToString();
                                }
                                user = new User(currentUser, Guid.NewGuid(), userName);
                                user.FirstName = parts[4];
                                user.LastName = parts[3];
                                user.FullName = parts[4] + " " + parts[3];
                                user.Email = email;
                                user.Address = "Import Address";
                                user.Phone = "12345";


                                if (!user.Organisations.Contains(organisation))
                                    user.Organisations.Add(organisation);

                                user.SetPrimaryOrganisation(organisation);
                                await _userService.ApplicationCreateUser(user);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }
        }
        public async Task ImportAOEServiceClaims(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            ClaimNotification claimNotification;
            bool readFirstLine = false;
            string line;

            //var claimFileName = "C:\\tmp\\testclientdata\\NZACSClaimsData2018.csv";
            var claimFileName = "/tmp/NZACSClaimsData2018.csv";
            using (reader = new StreamReader(claimFileName))
            {
                while (!reader.EndOfStream)
                {
                    //if (!readFirstLine)
                    //{
                    //    line = reader.ReadLine();
                    //    readFirstLine = true;
                    //}
                    try
                    {
                        line = reader.ReadLine();
                        string[] parts = line.Split(',');
                        claimNotification = new ClaimNotification(currentUser);
                        claimNotification.ClaimMembershipNumber = parts[0];
                        claimNotification.ClaimTitle = parts[1];
                        claimNotification.ClaimReference = parts[2];
                        claimNotification.ClaimNotifiedDate = DateTime.Parse(parts[3]);
                        claimNotification.Claimant = parts[4];
                        claimNotification.ClaimStatus = parts[5];

                        await _programmeService.AddClaimNotificationByMembership(claimNotification);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        public async Task ImportAOEServiceContract(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            BusinessContract businessContract;
            bool readFirstLine = false;
            string line;
            //special characters /,/
            //var contractFileName = "C:\\tmp\\testclientdata\\NZACSContractorsPrincipals2018.csv";
            var contractFileName = "/tmp/NZACSContractorsPrincipals2018.csv";

            using (reader = new StreamReader(contractFileName))
            {
                while (!reader.EndOfStream)
                {
                    //if (!readFirstLine)
                    //{
                    //    line = reader.ReadLine();
                    //    readFirstLine = true;
                    //}
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    try
                    {
                        businessContract = new BusinessContract(currentUser);
                        businessContract.MembershipNumber = parts[4];
                        businessContract.ContractTitle = parts[1];
                        businessContract.Year = parts[0];
                        businessContract.ConstructionValue = parts[2];
                        businessContract.Fees = parts[3];

                        await _programmeService.AddBusinessContractByMembership(businessContract);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        public async Task ImportAOEService(User user)
        {
            //await ImportAOEServiceIndividuals(user);
            //await ImportAOEServicePrincipals(user);
            //await ImportAOEServiceClaims(user);
            //await ImportAOEServiceBusinessContract(user);
        }

        public async Task ImportActivities(User user)
        {                       
            var fileName = WorkingDirectory + "anzsic06completeclassification.csv";
            var currentTemplateList = await _businessActivityService.GetBusinessActivitiesTemplates();
            List<BusinessActivityTemplate> BAList = new List<BusinessActivityTemplate>();

            using (StreamReader reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    string[] parts = line.Split(';');
                    BusinessActivityTemplate ba = new BusinessActivityTemplate(user);

                    if (!string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]))
                    {
                        //classification 1
                        ba.Classification = 1;
                        ba.AnzsciCode = parts[0];
                        ba.Description = parts[1];
                    }
                    if (!string.IsNullOrEmpty(parts[1]) && !string.IsNullOrEmpty(parts[2]))
                    {
                        //classification 2
                        ba.Classification = 2;
                        ba.AnzsciCode = parts[1];
                        ba.Description = parts[2];
                    }
                    if (!string.IsNullOrEmpty(parts[2]) && !string.IsNullOrEmpty(parts[3]))
                    {
                        //classification 3
                        ba.Classification = 3;
                        ba.AnzsciCode = parts[2];
                        ba.Description = parts[3];

                    }
                    if (!string.IsNullOrEmpty(parts[3]) && !string.IsNullOrEmpty(parts[4]))
                    {
                        //classification 4
                        ba.Classification = 4;
                        ba.AnzsciCode = parts[3];
                        ba.Description = parts[4];
                    }

                    if (ba.AnzsciCode != null)
                    {
                        var test = currentTemplateList.Where(bat => bat.AnzsciCode == ba.AnzsciCode).ToList();
                        if(test.Count == 0)
                        {
                            BAList.Add(ba);
                        }
                    }
                }
            }

            foreach (BusinessActivityTemplate businessActivity in BAList)
            {
                await _businessActivityService.CreateBusinessActivityTemplate(businessActivity);
            }
        }
        public async Task ImportCEASServiceIndividuals(User CreatedUser)
        {
            //addresses need to be on one line            
            var fileName = WorkingDirectory + "CEASClients2019.csv";
            var currentUser = CreatedUser;
            Guid programmeID = Guid.Parse("48ce028d-1fcb-4f3b-881b-9fd769b87643");
            StreamReader reader;
            User user = null;
            Organisation organisation = null;
            bool readFirstLine = false;
            string line;
            string email;
            using (reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    //if has a title row
                    if (!readFirstLine)
                    {
                        line = reader.ReadLine();
                        readFirstLine = true;
                    }
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    user = null;
                    organisation = null;
                    email = "";
                    try
                    {
                        if (string.IsNullOrWhiteSpace(parts[4]))
                        {
                            email = parts[8] + "@DealEngine.com";
                            user = await _userService.GetUserByEmail(email);
                        }
                        else
                        {
                            email = parts[4];
                        }

                        organisation = await _organisationService.GetOrganisationByEmail(email);

                        if (user == null)
                        {
                            user = new User(currentUser, Guid.NewGuid(), parts[8]);
                        }
                        organisation = await _organisationService.GetOrganisationByEmail(email);
                        if (parts[0] == "f")
                        {
                            if (organisation == null)
                            {
                                var organisationType = await _organisationTypeService.GetOrganisationTypeByName("Person - Individual");
                                organisation = new Organisation(currentUser, Guid.NewGuid(), parts[2] + " " + parts[3], organisationType, email);
                                await _organisationService.CreateNewOrganisation(organisation);
                            }
                        }
                        else
                        { 
                            if (organisation == null)
                            {
                                var organisationType = await _organisationTypeService.GetOrganisationTypeByName("Corporation – Limited liability");
                                organisation = new Organisation(currentUser, Guid.NewGuid(), parts[1], organisationType, parts[4]);
                                await _organisationService.CreateNewOrganisation(organisation);
                            }
                        }

                        user.FirstName = parts[3];
                        user.LastName = parts[4];
                        user.FullName = parts[3] + " " + parts[4];
                        user.Email = email;
                        user.Address = "";
                        user.Phone = "12345";

                        if (!user.Organisations.Contains(organisation))
                            user.Organisations.Add(organisation);
                        user.SetPrimaryOrganisation(organisation);

                        await _userService.ApplicationCreateUser(user);

                        var programme = await _programmeService.GetProgramme(programmeID);
                        var clientProgramme = await _programmeService.CreateClientProgrammeFor(programme.Id, user, organisation);

                        var reference = await _referenceService.GetLatestReferenceId();
                        var sheet = await _clientInformationService.IssueInformationFor(user, organisation, clientProgramme, reference);
                        await _referenceService.CreateClientInformationReference(sheet);

                        using (var uow = _unitOfWork.BeginUnitOfWork())
                        {

                            clientProgramme.BrokerContactUser = programme.BrokerContactUser;
                            clientProgramme.ClientProgrammeMembershipNumber = parts[7];
                            sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(user, sheet, null, programme.Name + "UIS issue Process Completed"));
                            try
                            {
                                await uow.Commit();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        public async Task ImportCEASServicePrincipals(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            User user = null;
            Organisation organisation = null;
            bool readFirstLine = false;
            string line;
            string email;
            string userName;
            //addresses need to be on one line            
            var fileName = WorkingDirectory + "CEASPrincipals2019.csv";
            var insuranceAttribute = await _InsuranceAttributeService.GetInsuranceAttributeByName("Principal");
            var organisationType = await _organisationTypeService.GetOrganisationTypeByName("Person - Individual");
            using (reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    //if has a title row
                    if (!readFirstLine)
                    {
                        line = reader.ReadLine();
                        readFirstLine = true;
                    }
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    user = null;
                    organisation = null;
                    email = "";
                    try
                    {
                        var hasProgramme = await _programmeService.HasProgrammebyMembership(parts[1]);
                        if (hasProgramme)
                        {
                            userName = parts[4].Replace(" ", string.Empty) + "_" + parts[3];

                            if (string.IsNullOrWhiteSpace(parts[5]))
                            {
                                email = parts[2] + "@DealEngine.com";
                            }
                            else
                            {
                                email = parts[5];
                            }

                            organisation = new Organisation(currentUser, Guid.NewGuid(), parts[2], organisationType, email);
                            organisation.InsuranceAttributes.Add(insuranceAttribute);
                            organisation.NZIAmembership = parts[1];
                            organisation.Email = email;
                            organisation.Phone = "12345";

                            if (!string.IsNullOrEmpty(parts[15]))
                            {
                                organisation.Qualifications = parts[15];
                            }
                            if (!string.IsNullOrEmpty(parts[11]))
                            {
                                organisation.IsIPENZmember = parts[11];
                            }
                            if (!string.IsNullOrEmpty(parts[12]))
                            {
                                organisation.CPEngQualified = parts[12];
                            }                            

                            using (var uom = _unitOfWork.BeginUnitOfWork())
                            {
                                insuranceAttribute.IAOrganisations.Add(organisation);
                                try
                                {
                                    await uom.Commit();
                                }
                                catch (Exception ex)
                                {
                                    await uom.Rollback();
                                }
                            }

                            await _organisationService.CreateNewOrganisation(organisation);
                            await _programmeService.AddOrganisationByMembership(organisation);

                            user = await _userService.GetUserByEmail(email);

                            if (user == null)
                            {                                
                                try
                                {
                                    user = await _userService.GetUser(userName);
                                }
                                catch (Exception ex)
                                {
                                    Random random = new Random();
                                    int randomNumber = random.Next(10, 99);
                                    userName = userName + randomNumber.ToString();
                                }
                                user = new User(currentUser, Guid.NewGuid(), userName);
                                user.FirstName = parts[4];
                                user.LastName = parts[3];
                                user.FullName = parts[4] + " " + parts[3];
                                user.Email = email;
                                user.Address = "Import Address";
                                user.Phone = "12345";


                                if (!user.Organisations.Contains(organisation))
                                    user.Organisations.Add(organisation);

                                user.SetPrimaryOrganisation(organisation);
                                await _userService.ApplicationCreateUser(user);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }
        }
        public async Task ImportCEASServiceClaims(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            ClaimNotification claimNotification;
            bool readFirstLine = false;
            string line;            
            var fileName = WorkingDirectory + "CEASClaims2019.csv";
            using (reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    if (!readFirstLine)
                    {
                        line = reader.ReadLine();
                        readFirstLine = true;
                    }
                    try
                    {
                        line = reader.ReadLine();
                        string[] parts = line.Split(',');
                        claimNotification = new ClaimNotification(currentUser);
                        claimNotification.ClaimMembershipNumber = parts[2];
                        claimNotification.ClaimTitle = parts[3];
                        claimNotification.ClaimReference = parts[4];
                        claimNotification.ClaimNotifiedDate = DateTime.Parse(parts[5]);
                        claimNotification.Claimant = parts[6];
                        claimNotification.ClaimStatus = parts[8];

                        await _programmeService.AddClaimNotificationByMembership(claimNotification);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        public async Task ImportCEASServiceContract(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            BusinessContract businessContract;
            bool readFirstLine = false;
            string line;
            var fileName = WorkingDirectory + "CEASContracts2019.csv";

            using (reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    if (!readFirstLine)
                    {
                        line = reader.ReadLine();
                        readFirstLine = true;
                    }
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    try
                    {
                        businessContract = new BusinessContract(currentUser);
                        businessContract.MembershipNumber = parts[6];
                        businessContract.ContractTitle = parts[0];
                        businessContract.ProjectDescription = parts[1];
                        businessContract.ProjectDuration = parts[5];
                        businessContract.ConstructionValue = parts[4];
                        businessContract.Fees = parts[3];
                        businessContract.MajorResponsibilities = parts[2];

                        await _programmeService.AddBusinessContractByMembership(businessContract);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}

