using DealEngine.Services.Interfaces;
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
            WorkingDirectory = "/tmp/";
            //WorkingDirectory = "C:\\Users\\Public\\"; //Ray Local
            _businessActivityService = businessActivityService;
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
                            var email = parts[7] + "@techcertain.com";
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
            var principalsFileName = WorkingDirectory + "NZACSPrincipals2018.csv";
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
                                email = parts[2] + "@techcertain.com";
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
            var claimFileName = WorkingDirectory + "nzacs//NZACSClaimsData2018.csv";
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
            var contractFileName = WorkingDirectory + "nzacs//NZACSContractorsPrincipals2018.csv";

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
            var fileName = WorkingDirectory + "ceas//CEASClients2019.csv";
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
                            email = parts[8] + "@techcertain.com";
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

        public async Task ImportPMINZServiceIndividuals(User CreatedUser)
        {
            //addresses need to be on one line            
            var fileName = WorkingDirectory + "PMINZClients2019Final.csv";
            var currentUser = CreatedUser;
            Guid programmeID = Guid.Parse("6a82f324-964f-47a6-b1cd-78848c62f616"); //PMINZ Programme ID
            StreamReader reader;
            User user = null;
            Organisation organisation = null;
            bool readFirstLine = true;
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
                            email = parts[8] + "@techcertain.com";
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

                        user.FirstName = parts[2];
                        user.LastName = parts[3];
                        user.FullName = parts[2] + " " + parts[3];
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
            var fileName = WorkingDirectory + "ceas//CEASPrincipals2019.csv";
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
                                email = parts[2] + "@techcertain.com";
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

        public async Task ImportPMINZServicePrincipals(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            User user = null;
            Organisation organisation = null;
            bool readFirstLine = true;
            string line;
            string email;
            string userName;
            //addresses need to be on one line            
            var fileName = WorkingDirectory + "PMINZPersonnel2019Final.csv";
            var insuranceAttribute = await _InsuranceAttributeService.GetInsuranceAttributeByName("project management personnel");
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
                        var hasProgramme = await _programmeService.HasProgrammebyMembership(parts[13]);
                        var fullname = parts[0] + " " + parts[1];
                        if (hasProgramme)
                        {
                            userName = parts[0].Replace(" ", string.Empty) + "_" + parts[1].Replace(" ", string.Empty);

                            if (string.IsNullOrWhiteSpace(parts[2]))
                            {
                                email = parts[0].Replace(" ", string.Empty) + parts[1].Replace(" ", string.Empty) + "@techcertain.com";
                            }
                            else
                            {
                                email = parts[2];
                            }

                            organisation = new Organisation(currentUser, Guid.NewGuid(), fullname, organisationType, email);
                            organisation.InsuranceAttributes.Add(insuranceAttribute);
                            organisation.NZIAmembership = parts[13];
                            organisation.Email = email;
                            organisation.Phone = "12345";

                            if (!string.IsNullOrEmpty(parts[3]))
                            {
                                organisation.JobTitle = parts[3];
                            }
                            if (!string.IsNullOrEmpty(parts[4]))
                            {
                                organisation.Qualifications = parts[4];
                            }
                            if (!string.IsNullOrEmpty(parts[5]))
                            {
                                organisation.ProfAffiliation = parts[5];
                            }
                            if (!string.IsNullOrEmpty(parts[6]))
                            {
                                organisation.CurrentMembershipNo = parts[6];
                            }
                            if (!string.IsNullOrEmpty(parts[7]))
                            {
                                if (parts[7] == "1")
                                {
                                    organisation.IsCurrentMembership = true;
                                } else
                                {
                                    organisation.IsCurrentMembership = false;
                                }
                            }
                            if (!string.IsNullOrEmpty(parts[9]))
                            {
                                if (parts[9] == "1")
                                {
                                    organisation.CertType = "PMP";
                                } else if (parts[9] == "2") 
                                {
                                    organisation.CertType = "CAPM";
                                } else if (parts[9] == "3")
                                {
                                    organisation.CertType = "ProjectDirector";
                                } else 
                                { 
                                    organisation.CertType = "Ordinary"; 
                                }
                            } else
                            {
                                organisation.CertType = "Ordinary";
                            }
                            if (!string.IsNullOrEmpty(parts[10]))
                            {
                                if (parts[10] == "1")
                                {
                                    organisation.InsuredEntityRelation = "Director";
                                }
                                else if (parts[10] == "2")
                                {
                                    organisation.InsuredEntityRelation = "Employee";
                                }
                                else if (parts[10] == "3")
                                {
                                    organisation.InsuredEntityRelation = "Contractor";
                                }
                            }
                            if (!string.IsNullOrEmpty(parts[11]))
                            {
                                if (parts[11] == "1")
                                {
                                    organisation.IsContractorInsured = true;
                                }
                                else
                                {
                                    organisation.IsContractorInsured = false;
                                }
                            }
                            if (!string.IsNullOrEmpty(parts[12]))
                            {
                                if (parts[12] == "1")
                                {
                                    organisation.IsInsuredRequired = true;
                                }
                                else
                                {
                                    organisation.IsInsuredRequired = false;
                                }
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
                                user.FirstName = parts[0];
                                user.LastName = parts[1];
                                user.FullName = fullname;
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
            var fileName = WorkingDirectory + "ceas//CEASClaims2019.csv";
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
            var fileName = WorkingDirectory + "ceas//CEASContracts2019.csv";

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

        public async Task ImportPMINZServiceContract(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            BusinessContract businessContract;
            bool readFirstLine = true;
            string line;
            var fileName = WorkingDirectory + "PMINZProjects2019Final.csv";

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
                        businessContract.MembershipNumber = parts[7];
                        if (!string.IsNullOrEmpty(parts[0]))
                            businessContract.ProjectDescription = parts[0];
                        if (!string.IsNullOrEmpty(parts[1]))
                            businessContract.Fees = parts[1];
                        if (!string.IsNullOrEmpty(parts[2]))
                            businessContract.ConstructionValue = parts[2];
                        if (!string.IsNullOrEmpty(parts[3]))
                            businessContract.ProjectDuration = parts[3];
                        if (parts[4] == "1")
                        {
                            businessContract.ProjectDirector = true;
                        } else
                        {
                            businessContract.ProjectDirector = false;
                        }
                        if (parts[5] == "1")
                        {
                            businessContract.ProjectManager = true;
                        }
                        else
                        {
                            businessContract.ProjectManager = false;
                        }
                        if (parts[6] == "1")
                        {
                            businessContract.ProjectCoordinator = true;
                        }
                        else
                        {
                            businessContract.ProjectCoordinator = false;
                        }

                        await _programmeService.AddBusinessContractByMembership(businessContract);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public async Task ImportPMINZServicePreRenewData(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            PreRenewOrRefData preRenewOrRefData;
            bool readFirstLine = true;
            string line;
            var fileName = WorkingDirectory + "PMINZPolicyData2019Final.csv";

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
                        preRenewOrRefData = new PreRenewOrRefData(currentUser, parts[1], parts[0]);
                        if (!string.IsNullOrEmpty(parts[2]))
                            preRenewOrRefData.PIBoundLimit = parts[2];
                        if (!string.IsNullOrEmpty(parts[3]))
                            preRenewOrRefData.PIBoundPremium = parts[3];
                        if (!string.IsNullOrEmpty(parts[4]))
                            preRenewOrRefData.PIRetro = parts[4];
                        if (!string.IsNullOrEmpty(parts[5]))
                            preRenewOrRefData.GLRetro = parts[5];
                        if (!string.IsNullOrEmpty(parts[6]))
                            preRenewOrRefData.DORetro = parts[6];
                        if (!string.IsNullOrEmpty(parts[7]))
                            preRenewOrRefData.ELRetro = parts[7];
                        if (!string.IsNullOrEmpty(parts[8]))
                            preRenewOrRefData.EDRetro = parts[8];
                        if (!string.IsNullOrEmpty(parts[9]))
                            preRenewOrRefData.SLRetro = parts[9];
                        if (!string.IsNullOrEmpty(parts[10]))
                            preRenewOrRefData.CLRetro = parts[10];
                        if (!string.IsNullOrEmpty(parts[11]))
                            preRenewOrRefData.EndorsementProduct = parts[11];
                        if (!string.IsNullOrEmpty(parts[12]))
                            preRenewOrRefData.EndorsementTitle = parts[12];
                        if (!string.IsNullOrEmpty(parts[13]))
                            preRenewOrRefData.EndorsementText = parts[13];

                        await _programmeService.AddPreRenewOrRefDataByMembership(preRenewOrRefData);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public async Task ImportDANZServicePreRenewData(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            PreRenewOrRefData preRenewOrRefData;
            bool readFirstLine = true;
            string line;
            var fileName = WorkingDirectory + "DANZPolicyData2019.csv";

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
                        preRenewOrRefData = new PreRenewOrRefData(currentUser, parts[1], parts[0]);                        
                        if (!string.IsNullOrEmpty(parts[2]))
                            preRenewOrRefData.PIRetro = parts[2];
                        if (!string.IsNullOrEmpty(parts[3]))
                            preRenewOrRefData.GLRetro = parts[3];
                        if (!string.IsNullOrEmpty(parts[4]))
                            preRenewOrRefData.DORetro = parts[4];
                        if (!string.IsNullOrEmpty(parts[5]))
                            preRenewOrRefData.ELRetro = parts[5];
                        if (!string.IsNullOrEmpty(parts[6]))
                            preRenewOrRefData.EDRetro = parts[6];
                        if (!string.IsNullOrEmpty(parts[7]))
                            preRenewOrRefData.SLRetro = parts[7];
                        if (!string.IsNullOrEmpty(parts[8]))
                            preRenewOrRefData.CLRetro = parts[8];
                        if (!string.IsNullOrEmpty(parts[9]))
                            preRenewOrRefData.EndorsementProduct = parts[9];
                        if (!string.IsNullOrEmpty(parts[10]))
                            preRenewOrRefData.EndorsementTitle = parts[10];
                        if (!string.IsNullOrEmpty(parts[11]))
                            preRenewOrRefData.EndorsementText = parts[11];

                        await _programmeService.AddPreRenewOrRefDataByMembership(preRenewOrRefData);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public async Task ImportDANZServiceIndividuals(User CreatedUser)
        {
            //addresses need to be on one line            
            var fileName = WorkingDirectory + "DANZClients2019.csv";
            var currentUser = CreatedUser;
            Guid programmeID = Guid.Parse("226ca7cb-8145-4ac4-87dd-7f5dcc6358f4"); 
            StreamReader reader;
            User user = null;
            Organisation organisation = null;
            bool readFirstLine = true;
            string line;
            string email;
            int lineCount = 0;
            using (reader = new StreamReader(fileName))
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
                    email = "";
                    try
                    {
                        if (string.IsNullOrWhiteSpace(parts[4]))
                        {
                            email = parts[8] + "@techcertain.com";
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

                        user.FirstName = parts[2];
                        user.LastName = parts[3];
                        user.FullName = parts[2] + " " + parts[3];
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
                        lineCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + lineCount);
                    }
                }
            }
        }

        public async Task ImportDANZServicePersonnel(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            User user = null;
            Organisation organisation = null;
            bool readFirstLine = true;
            string line;
            string email;
            string userName;
            //addresses need to be on one line            
            var fileName = WorkingDirectory + "DANZPersonnel2019.csv";
            var insuranceAttribute = await _InsuranceAttributeService.GetInsuranceAttributeByName("project management personnel");
            var organisationType = await _organisationTypeService.GetOrganisationTypeByName("Person - Individual");
            int lineCount = 0;
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
                        var hasProgramme = await _programmeService.HasProgrammebyMembership(parts[12]);
                        var fullname = parts[0] + " " + parts[1];
                        if (hasProgramme)
                        {
                            userName = parts[0].Replace(" ", string.Empty) + "_" + parts[1].Replace(" ", string.Empty);

                            if (string.IsNullOrWhiteSpace(parts[2]))
                            {
                                email = parts[0].Replace(" ", string.Empty) + parts[1].Replace(" ", string.Empty) + "@techcertain.com";
                            }
                            else
                            {
                                email = parts[2];
                            }

                            organisation = new Organisation(currentUser, Guid.NewGuid(), fullname, organisationType, email);
                            organisation.InsuranceAttributes.Add(insuranceAttribute);
                            organisation.NZIAmembership = parts[12];
                            organisation.Email = email;
                            organisation.Phone = "12345";

                            if (!string.IsNullOrEmpty(parts[5]))
                            {
                                organisation.Qualifications = parts[5];
                            }
                            if (!string.IsNullOrEmpty(parts[3]))
                            {
                                organisation.DateQualified = parts[3];
                            }
                            if (!string.IsNullOrEmpty(parts[6]))
                            {
                                if(parts[6] == "1")
                                {
                                    organisation.IsRegisteredLicensed = true;
                                }
                                else
                                {
                                    organisation.IsRegisteredLicensed = false;
                                }
                                
                            }
                            if (!string.IsNullOrEmpty(parts[7]))
                            {
                                if (parts[7] == "1")
                                {
                                    organisation.DesignLicensed = "Yes";
                                }
                                else
                                {
                                    organisation.DesignLicensed = "No";
                                }
                            }
                            if (!string.IsNullOrEmpty(parts[8]))
                            {
                                if (parts[8] == "1")
                                {
                                    organisation.SiteLicensed = "Yes";
                                }
                                else
                                {
                                    organisation.SiteLicensed = "No";
                                }
                            }
                            if (!string.IsNullOrEmpty(parts[9]))
                            {
                                if (parts[9] == "1")
                                {
                                    organisation.InsuredEntityRelation = "Director";
                                }
                                else if (parts[9] == "2")
                                {
                                    organisation.InsuredEntityRelation = "Employee";
                                }
                                else if (parts[9] == "3")
                                {
                                    organisation.InsuredEntityRelation = "Contractor";
                                }
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
                                user.FirstName = parts[0];
                                user.LastName = parts[1];
                                user.FullName = fullname;
                                user.Email = email;
                                user.Address = "Import Address";
                                user.Phone = "12345";


                                if (!user.Organisations.Contains(organisation))
                                    user.Organisations.Add(organisation);

                                user.SetPrimaryOrganisation(organisation);
                                await _userService.ApplicationCreateUser(user);
                            }
                        }
                        lineCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + lineCount);
                    }

                }
            }
        }

        public async Task ImportDANZServiceClaims(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            ClaimNotification claimNotification;
            var programme = await _programmeService.GetProgramme(Guid.Parse("226ca7cb-8145-4ac4-87dd-7f5dcc6358f4"));
            Product DanzPIProd = programme.Products.FirstOrDefault(p=>p.UnderwritingModuleCode == "DANZ_PI");
            Product DanzEPLProd = programme.Products.FirstOrDefault(p => p.UnderwritingModuleCode == "DANZ_ED");
            bool readFirstLine = false;
            string line;
            var fileName = WorkingDirectory + "DANZClaimsDetails.csv";
            using (reader = new StreamReader(fileName))
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
                        claimNotification.ClaimTitle = parts[4];
                        claimNotification.ClaimReference = parts[3];
                        claimNotification.ClaimDescription = parts[5];
                        claimNotification.ClaimNotifiedDate = DateTime.Parse(parts[7]);
                        claimNotification.ClaimDateOfLoss = DateTime.Parse(parts[8]);
                        claimNotification.ClaimEstimateInsuredLiability = decimal.Parse(parts[2]);
                        claimNotification.Claimant = parts[6];
                        claimNotification.ClaimStatus = parts[9];
                        if(parts[1] == "PI")
                        {
                            claimNotification.ClaimProducts.Add(DanzPIProd);
                        }
                        else
                        {
                            claimNotification.ClaimProducts.Add(DanzEPLProd);
                        }

                        await _programmeService.AddClaimNotificationByMembership(claimNotification);
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

