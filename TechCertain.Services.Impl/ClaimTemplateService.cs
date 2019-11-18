using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class ClaimTemplateService : IClaimTemplateService
    {
        IMapperSession<Claim> _claimsRepository;

        public ClaimTemplateService(IMapperSession<Claim> claimsRepository)
        {
            _claimsRepository = claimsRepository;
        }

        public async Task CreateClaimsForOrganisation(Organisation primaryOrganisation)
        {
            //claim(type, value) in this case the type is type of rule and value is the rule

            //view dashboard rules
            Claim v1 = new Claim("View", "ViewComingUpTask", primaryOrganisation);
            Claim v2 = new Claim("View", "ViewMilestoneTasks", primaryOrganisation);
            Claim v3 = new Claim("View", "ViewClientAgreements", primaryOrganisation);
            Claim v4 = new Claim("View", "ViewTermSheets", primaryOrganisation);
            Claim v5 = new Claim("View", "ViewLibraries", primaryOrganisation);
            Claim v6 = new Claim("View", "ViewAdmin", primaryOrganisation);
            await _claimsRepository.AddAsync(v1);
            await _claimsRepository.AddAsync(v2);
            await _claimsRepository.AddAsync(v3);
            await _claimsRepository.AddAsync(v4);
            await _claimsRepository.AddAsync(v5);
            await _claimsRepository.AddAsync(v6);

            //view term sheet rules
            Claim s1 = new Claim("View", "ViewProgramme", primaryOrganisation);
            Claim s2 = new Claim("View", "ViewActivities", primaryOrganisation);
            Claim s3 = new Claim("View", "ViewManageDocuments", primaryOrganisation);
            Claim s4 = new Claim("View", "ViewRules", primaryOrganisation);
            Claim s5 = new Claim("View", "ViewEmails", primaryOrganisation);
            Claim s6 = new Claim("View", "ViewMilestones", primaryOrganisation);
            Claim s7 = new Claim("View", "ViewMarinas/FinancialInstitutions", primaryOrganisation);
            Claim s8 = new Claim("View", "ViewMotor", primaryOrganisation);
            Claim s9 = new Claim("View", "ViewProducts", primaryOrganisation);
            Claim s10 = new Claim("View", "ViewSections", primaryOrganisation);
            Claim s11 = new Claim("View", "ViewSpecialTerms", primaryOrganisation);
            Claim s12 = new Claim("View", "ViewUnderwritingRules", primaryOrganisation);
            Claim s13 = new Claim("View", "ViewUsers", primaryOrganisation);
            Claim s14 = new Claim("View", "ViewWater", primaryOrganisation);
            await _claimsRepository.AddAsync(s1);
            await _claimsRepository.AddAsync(s2);
            await _claimsRepository.AddAsync(s3);
            await _claimsRepository.AddAsync(s4);
            await _claimsRepository.AddAsync(s5);
            await _claimsRepository.AddAsync(s6);
            await _claimsRepository.AddAsync(s7);
            await _claimsRepository.AddAsync(s8);
            await _claimsRepository.AddAsync(s9);
            await _claimsRepository.AddAsync(s10);
            await _claimsRepository.AddAsync(s11);
            await _claimsRepository.AddAsync(s12);
            await _claimsRepository.AddAsync(s13);
            await _claimsRepository.AddAsync(s14);

            //edit term sheet rules
            Claim e1 = new Claim("Edit", "EditProgramme", primaryOrganisation);
            Claim e2 = new Claim("Edit", "EditActivities", primaryOrganisation);
            Claim e3 = new Claim("Edit", "EditManageDocuments", primaryOrganisation);
            Claim e4 = new Claim("Edit", "EditRules", primaryOrganisation);
            Claim e5 = new Claim("Edit", "EditEmails", primaryOrganisation);
            Claim e6 = new Claim("Edit", "EditMilestones", primaryOrganisation);
            Claim e7 = new Claim("Edit", "EditMarinas/FinancialInstitutions", primaryOrganisation);
            Claim e8 = new Claim("Edit", "EditMotor", primaryOrganisation);
            Claim e9 = new Claim("Edit", "EditProducts", primaryOrganisation);
            Claim e10 = new Claim("Edit", "EditSections", primaryOrganisation);
            Claim e11 = new Claim("Edit", "EditSpecialTerms", primaryOrganisation);
            Claim e12 = new Claim("Edit", "EditUnderwritingRules", primaryOrganisation);
            Claim e13 = new Claim("Edit", "EditUsers", primaryOrganisation);
            Claim e14 = new Claim("Edit", "EditWater", primaryOrganisation);
            await _claimsRepository.AddAsync(e1);
            await _claimsRepository.AddAsync(e2);
            await _claimsRepository.AddAsync(e3);
            await _claimsRepository.AddAsync(e4);
            await _claimsRepository.AddAsync(e5);
            await _claimsRepository.AddAsync(e6);
            await _claimsRepository.AddAsync(e7);
            await _claimsRepository.AddAsync(e8);
            await _claimsRepository.AddAsync(e9);
            await _claimsRepository.AddAsync(e10);
            await _claimsRepository.AddAsync(e11);
            await _claimsRepository.AddAsync(e12);
            await _claimsRepository.AddAsync(e13);
            await _claimsRepository.AddAsync(e14);
        }
    }
}
