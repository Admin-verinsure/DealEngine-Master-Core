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

        public async Task CreateAllClaims()
        {
            //claim(type, value) in this case the type is type of rule and value is the rule

            //view dashboard rules
            Claim v1 = new Claim("View", "ViewComingUpTask");
            Claim v2 = new Claim("View", "ViewMilestoneTasks");
            Claim v3 = new Claim("View", "ViewClientAgreements");
            Claim v4 = new Claim("View", "ViewTermSheets");
            Claim v5 = new Claim("View", "ViewLibraries");
            Claim v6 = new Claim("View", "ViewAdmin");
            await _claimsRepository.AddAsync(v1);
            await _claimsRepository.AddAsync(v2);
            await _claimsRepository.AddAsync(v3);
            await _claimsRepository.AddAsync(v4);
            await _claimsRepository.AddAsync(v5);
            await _claimsRepository.AddAsync(v6);

            //view term sheet rules
            Claim s1 = new Claim("View", "ViewProgramme");
            Claim s2 = new Claim("View", "ViewActivities");
            Claim s3 = new Claim("View", "ViewManageDocuments");
            Claim s4 = new Claim("View", "ViewRules");
            Claim s5 = new Claim("View", "ViewEmails");
            Claim s6 = new Claim("View", "ViewMilestones");
            Claim s7 = new Claim("View", "ViewMarinas/FinancialInstitutions");
            Claim s8 = new Claim("View", "ViewMotor");
            Claim s9 = new Claim("View", "ViewProducts");
            Claim s10 = new Claim("View", "ViewSections");
            Claim s11 = new Claim("View", "ViewSpecialTerms");
            Claim s12 = new Claim("View", "ViewUnderwritingRules");
            Claim s13 = new Claim("View", "ViewUsers");
            Claim s14 = new Claim("View", "ViewWater");
            Claim s15 = new Claim("View", "ViewTerritories");
            Claim s16 = new Claim("View", "ViewEmploymentPrincipals");
            Claim s17 = new Claim("View", "ViewAddresses");
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
            await _claimsRepository.AddAsync(s15);
            await _claimsRepository.AddAsync(s16);
            await _claimsRepository.AddAsync(s17);

            //edit term sheet rules
            Claim e1 = new Claim("Edit", "EditProgramme");
            Claim e2 = new Claim("Edit", "EditActivities");
            Claim e3 = new Claim("Edit", "EditManageDocuments");
            Claim e4 = new Claim("Edit", "EditRules");
            Claim e5 = new Claim("Edit", "EditEmails");
            Claim e6 = new Claim("Edit", "EditMilestones");
            Claim e7 = new Claim("Edit", "EditMarinas/FinancialInstitutions");
            Claim e8 = new Claim("Edit", "EditMotor");
            Claim e9 = new Claim("Edit", "EditProducts");
            Claim e10 = new Claim("Edit", "EditSections");
            Claim e11 = new Claim("Edit", "EditSpecialTerms");
            Claim e12 = new Claim("Edit", "EditUnderwritingRules");
            Claim e13 = new Claim("Edit", "EditUsers");
            Claim e14 = new Claim("Edit", "EditWater");
            Claim e15 = new Claim("Edit", "EditTerritories");
            Claim e16 = new Claim("Edit", "EditEmploymentPrincipals");
            Claim e17 = new Claim("Edit", "EditAddresses");
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
            await _claimsRepository.AddAsync(e15);
            await _claimsRepository.AddAsync(e16);
            await _claimsRepository.AddAsync(e17);
        }
    }
}
