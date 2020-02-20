using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;
using System.IO;
using System.Collections.Generic;

namespace TechCertain.Services.Impl
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

        public ImportService(IOrganisationService organisationService, IUserService userService,
            IProgrammeService programmeService, IReferenceService referenceService, IClientInformationService clientInformationService,
            IUnitOfWork unitOfWork, IOrganisationTypeService organisationTypeService, IInsuranceAttributeService insuranceAttributeService,
            IMapperSession<Organisation> organisationRepository, IBusinessActivityService businessActivityService)
        {
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
            var userFileName = "/tmp/NZACSUsers2018.csv";
            var currentUser = CreatedUser;
            Guid programmeID = Guid.Parse("214efe24-552d-46a3-a666-6bede7c88ca1");
            StreamReader reader;
            User user;
            Organisation organisation;
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
                    try
                    {
                        if (parts[0] == "f")
                        {
                            organisation = await _organisationService.GetOrganisationByEmail(parts[4]);

                            if (organisation == null)
                            {
                                var organisationType = await _organisationTypeService.GetOrganisationTypeByName("Person - Individual");
                                organisation = new Organisation(currentUser, Guid.NewGuid(), parts[2] + " " + parts[3], organisationType);
                                await _organisationService.CreateNewOrganisation(organisation);
                            }
                        }
                        else
                        {
                            organisation = await _organisationService.GetOrganisationByEmail(parts[4]);

                            if (organisation == null)
                            {
                                var organisationType = await _organisationTypeService.GetOrganisationTypeByName("Corporation – Limited liability");
                                organisation = new Organisation(currentUser, Guid.NewGuid(), parts[1], organisationType);
                                await _organisationService.CreateNewOrganisation(organisation);
                            }
                        }

                        user = await _userService.GetUserByEmail(parts[4]);
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
            Organisation organisation;
            bool readFirstLine = false;
            string line;

            //addresses need to be on one line
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

                    try
                    {
                        organisation = await _organisationService.GetOrganisationByEmail(parts[5]);
                        if(organisation == null)
                        {
                            organisation = new Organisation(currentUser, Guid.NewGuid(), parts[2], organisationType);
                        }                        
                        organisation.InsuranceAttributes.Add(insuranceAttribute);
                        organisation.NZIAmembership = parts[1];
                        organisation.Email = parts[5];
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

                        user = await _userService.GetUserByEmail(parts[5]);
                        if (user == null)
                        {
                            string userName = parts[4] + "_" + parts[3];
                            user = new User(currentUser, Guid.NewGuid(), userName);
                            user.FirstName = parts[4];
                            user.LastName = parts[3];
                            user.FullName = parts[4] + " " + parts[3];
                            user.Email = parts[5];
                            user.Address = "Import Address";
                            user.Phone = "12345";
                        }

                        if (!user.Organisations.Contains(organisation))
                            user.Organisations.Add(organisation);

                        user.SetPrimaryOrganisation(organisation);
                        await _userService.ApplicationCreateUser(user);
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
        public async Task ImportAOEServiceBusinessContract(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            BusinessContract businessContract;
            bool readFirstLine = false;
            string line;
            //special characters /,/
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
            var fileName = "C:\\tmp\\anzsic06completeclassification.csv";
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
                        BAList.Add(ba);
                    }
                }
            }

            foreach (BusinessActivityTemplate businessActivity in BAList)
            {
                await _businessActivityService.CreateBusinessActivityTemplate(businessActivity);
            }
        }
    }
}

