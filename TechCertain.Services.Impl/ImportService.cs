using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;
using System.IO;

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

        IInsuranceAttributeService _InsuranceAttributeService;

        public ImportService(IOrganisationService organisationService, IUserService userService, 
            IProgrammeService programmeService, IReferenceService referenceService, IClientInformationService clientInformationService,
            IUnitOfWork unitOfWork, IOrganisationTypeService organisationTypeService, IInsuranceAttributeService insuranceAttributeService,
            IMapperSession<Organisation> organisationRepository)
        {
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
            var userFileName = "C:\\tmp\\NZACS Individuals Demo.csv";
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
                    if (!readFirstLine)
                    {
                        line = reader.ReadLine();
                        readFirstLine = true;
                    }
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');

                    if (parts[0] == "f")
                    {
                        organisation = await _organisationService.GetOrganisationByEmail(parts[4]);

                        if (organisation == null)
                        {
                            var organisationType = await _organisationTypeService.GetOrganisationTypeByName("Person - Individual");
                            organisation = new Organisation(currentUser, Guid.NewGuid(), parts[2]+" "+ parts[3], organisationType);                            
                            await _organisationService.CreateNewOrganisation(organisation);
                        }

                        user = new User(currentUser, Guid.NewGuid(), parts[8]);
                        user.FirstName = parts[2];
                        user.LastName = parts[3];
                        user.FullName = parts[2] + " " + parts[3];
                        user.Email = parts[4];
                        user.Address = parts[5];
                        user.Phone = "12345";

                        if (!user.Organisations.Contains(organisation))
                            user.Organisations.Add(organisation);

                        user.SetPrimaryOrganisation(organisation);
                        await _userService.GetUser(user.UserName);

                        var programme = await _programmeService.GetProgramme(programmeID);
                        var clientProgramme = await _programmeService.CreateClientProgrammeFor(programme.Id, user, organisation);
                        try
                        {
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
        }

        public async Task ImportAOEServicePrincipals(User CreatedUser)
        {
            var currentUser = CreatedUser;
            StreamReader reader;
            User user;
            Organisation organisation;
            bool readFirstLine = false;
            string line;
            var principalsFileName = "C:\\tmp\\NZACS Principals Data Demo.csv";
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
                    if (!readFirstLine)
                    {
                        line = reader.ReadLine();
                        readFirstLine = true;
                    }
                    line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    
                    organisation = new Organisation(currentUser, Guid.Parse(parts[0]), parts[2], organisationType);
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

                    using(var uom = _unitOfWork.BeginUnitOfWork())
                    {
                        insuranceAttribute.IAOrganisations.Add(organisation);
                        try
                        {
                            await uom.Commit();
                        }
                        catch(Exception ex)
                        {
                            await uom.Rollback();
                        }
                    }
                    
                        await _organisationService.CreateNewOrganisation(organisation);
                        await _programmeService.AddOrganisationByMembership(organisation);                                            
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
            var claimFileName = "C:\\tmp\\NZACS Claims Data Demo.csv";
            using (reader = new StreamReader(claimFileName))
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
                    claimNotification = new ClaimNotification(currentUser);
                    claimNotification.ClaimMembershipNumber = parts[0];
                    claimNotification.ClaimTitle = parts[1];
                    claimNotification.ClaimReference = parts[2];
                    claimNotification.ClaimNotifiedDate = DateTime.Parse(parts[3]);
                    claimNotification.Claimant = parts[4];
                    claimNotification.ClaimStatus = parts[5];

                    await _programmeService.AddClaimNotificationByMembership(claimNotification);
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
            var contractFileName = "C:\\tmp\\NZACS Contractors Principals Demo.csv";
            using (reader = new StreamReader(contractFileName))
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
                    businessContract = new BusinessContract(currentUser);
                    businessContract.MembershipNumber = parts[4];
                    businessContract.ContractTitle = parts[1];
                    businessContract.Year = parts[0];
                    businessContract.ConstructionValue = parts[2];
                    businessContract.Fees = parts[3];

                    await _programmeService.AddBusinessContractByMembership(businessContract);
                }
            }
        }

        public async Task ImportAOEService(User user)
        {
            await ImportAOEServiceIndividuals(user);            
            await ImportAOEServicePrincipals(user);
            await ImportAOEServiceClaims(user);
            await ImportAOEServiceBusinessContract(user);
        }
    }
}

