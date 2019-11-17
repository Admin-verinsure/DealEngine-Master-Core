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
            Claim v1 = new Claim("View", "ComingUpTask", primaryOrganisation);
            Claim v2 = new Claim("View", "MilestoneTasks", primaryOrganisation);
            Claim v3 = new Claim("View", "ClientAgreements", primaryOrganisation);
            Claim v4 = new Claim("View", "TermSheets", primaryOrganisation);
            Claim v5 = new Claim("View", "Libraries", primaryOrganisation);
            Claim v6 = new Claim("View", "Admin", primaryOrganisation);
            await _claimsRepository.AddAsync(v1);
            await _claimsRepository.AddAsync(v2);
            await _claimsRepository.AddAsync(v3);
            await _claimsRepository.AddAsync(v4);
            await _claimsRepository.AddAsync(v5);
            await _claimsRepository.AddAsync(v6);

            //view term sheet rules
            Claim s1 = new Claim("View", "Programme", primaryOrganisation);
            Claim s2 = new Claim("View", "Activities", primaryOrganisation);
            Claim s3 = new Claim("View", "ManageDocuments", primaryOrganisation);
            Claim s4 = new Claim("View", "Rules", primaryOrganisation);
            Claim s5 = new Claim("View", "Emails", primaryOrganisation);
            Claim s6 = new Claim("View", "Milestones", primaryOrganisation);
            Claim s7 = new Claim("View", "Marinas/FinancialInstitutions", primaryOrganisation);
            Claim s8 = new Claim("View", "Motor", primaryOrganisation);
            Claim s9 = new Claim("View", "Products", primaryOrganisation);
            Claim s10 = new Claim("View", "Sections", primaryOrganisation);
            Claim s11 = new Claim("View", "SpecialTerms", primaryOrganisation);
            Claim s12 = new Claim("View", "UnderwritingRules", primaryOrganisation);
            Claim s13 = new Claim("View", "Users", primaryOrganisation);
            Claim s14 = new Claim("View", "Water", primaryOrganisation);
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
            Claim e1 = new Claim("Edit", "Programme", primaryOrganisation);
            Claim e2 = new Claim("Edit", "Activities", primaryOrganisation);
            Claim e3 = new Claim("Edit", "ManageDocuments", primaryOrganisation);
            Claim e4 = new Claim("Edit", "Rules", primaryOrganisation);
            Claim e5 = new Claim("Edit", "Emails", primaryOrganisation);
            Claim e6 = new Claim("Edit", "Milestones", primaryOrganisation);
            Claim e7 = new Claim("Edit", "Marinas/FinancialInstitutions", primaryOrganisation);
            Claim e8 = new Claim("Edit", "Motor", primaryOrganisation);
            Claim e9 = new Claim("Edit", "Products", primaryOrganisation);
            Claim e10 = new Claim("Edit", "Sections", primaryOrganisation);
            Claim e11 = new Claim("Edit", "SpecialTerms", primaryOrganisation);
            Claim e12 = new Claim("Edit", "UnderwritingRules", primaryOrganisation);
            Claim e13 = new Claim("Edit", "Users", primaryOrganisation);
            Claim e14 = new Claim("Edit", "Water", primaryOrganisation);
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
