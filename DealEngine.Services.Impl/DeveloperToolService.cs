using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;

namespace DealEngine.Services.Impl
{
    public class DeveloperToolService : IDeveloperToolService
    {
        IMapperSession<WaterLocation> _WaterLocationRepository;
        IMapperSession<Organisation> _OrganisationRepository;

        public DeveloperToolService(IMapperSession<WaterLocation> WaterLocationRepository,
            IMapperSession<Organisation> OrganisationRepository)
        {
            _WaterLocationRepository = WaterLocationRepository;
            _OrganisationRepository = OrganisationRepository;
        }

        public async Task CreateMarinas()
        {
            Organisation marina1 = new Organisation(null, Guid.NewGuid());            
            OrganisationType organisationType = new OrganisationType("Corporation – Limited liability");
            InsuranceAttribute insuranceAttribute = new InsuranceAttribute(null, "Marina");
            MarinaUnit marinaUnit = new MarinaUnit(null, "Marina", "Corporation – Limited liability", null);
            WaterLocation defaultMar1 = new WaterLocation(null);
            defaultMar1.MarinaName = "Hobsonville";
            defaultMar1.IsPublic = true;
            marinaUnit.WaterLocation = defaultMar1;
            marina1.OrganisationType = organisationType;
            marina1.InsuranceAttributes.Add(insuranceAttribute);
            marina1.OrganisationalUnits.Add(marinaUnit);

            await _OrganisationRepository.AddAsync(marina1);

            Organisation marina2 = new Organisation(null, Guid.NewGuid());
            OrganisationType organisationType2 = new OrganisationType("Corporation – Limited liability");
            InsuranceAttribute insuranceAttribute2 = new InsuranceAttribute(null, "Marina");
            MarinaUnit marinaUnit2 = new MarinaUnit(null, "Marina", "Corporation – Limited liability", null);
            WaterLocation DefaultMar2 = new WaterLocation(null);
            DefaultMar2.MarinaName = "Pine Harbour";
            DefaultMar2.IsPublic = true;
            marinaUnit2.WaterLocation = DefaultMar2;
            marina2.OrganisationType = organisationType2;
            marina2.InsuranceAttributes.Add(insuranceAttribute2);
            marina2.OrganisationalUnits.Add(marinaUnit2);

            await _OrganisationRepository.AddAsync(marina2);


            Organisation marina3 = new Organisation(null, Guid.NewGuid());
            OrganisationType organisationType3 = new OrganisationType("Corporation – Limited liability");
            InsuranceAttribute insuranceAttribute3 = new InsuranceAttribute(null, "Marina");
            MarinaUnit marinaUnit3 = new MarinaUnit(null, "Marina", "Corporation – Limited liability", null);
            WaterLocation DefaultMar3 = new WaterLocation(null);
            DefaultMar3.MarinaName = "West Haven";
            DefaultMar3.IsPublic = true;
            marinaUnit3.WaterLocation = DefaultMar3;
            marina3.OrganisationType = organisationType3;
            marina3.InsuranceAttributes.Add(insuranceAttribute3);
            marina3.OrganisationalUnits.Add(marinaUnit3);

            await _OrganisationRepository.AddAsync(marina3);


            Organisation marina4 = new Organisation(null, Guid.NewGuid());
            OrganisationType organisationType4 = new OrganisationType("Corporation – Limited liability");
            InsuranceAttribute insuranceAttribute4 = new InsuranceAttribute(null, "Marina");
            MarinaUnit marinaUnit4 = new MarinaUnit(null, "Marina", "Corporation – Limited liability", null);
            WaterLocation DefaultMar4 = new WaterLocation(null);
            DefaultMar4.MarinaName = "Silo Marina";
            DefaultMar4.IsPublic = true;
            marinaUnit4.WaterLocation = DefaultMar4;
            marina4.OrganisationType = organisationType4;
            marina4.InsuranceAttributes.Add(insuranceAttribute4);
            marina4.OrganisationalUnits.Add(marinaUnit4);

            await _OrganisationRepository.AddAsync(marina4);

            Organisation marina5 = new Organisation(null, Guid.NewGuid());
            OrganisationType organisationType5 = new OrganisationType("Corporation – Limited liability");
            InsuranceAttribute insuranceAttribute5 = new InsuranceAttribute(null, "Marina");
            MarinaUnit marinaUnit5 = new MarinaUnit(null, "Marina", "Corporation – Limited liability", null);
            WaterLocation DefaultMar5 = new WaterLocation(null);
            DefaultMar5.MarinaName = "Clyde Quay Boat Harbour";
            DefaultMar5.IsPublic = true;
            marinaUnit5.WaterLocation = DefaultMar5;
            marina5.OrganisationType = organisationType5;
            marina5.InsuranceAttributes.Add(insuranceAttribute5);
            marina5.OrganisationalUnits.Add(marinaUnit5);

            await _OrganisationRepository.AddAsync(marina5);

            Organisation instatute = new Organisation(null, Guid.NewGuid());
            OrganisationType organisationType6 = new OrganisationType("Corporation – Limited liability");
            InsuranceAttribute insuranceAttribute6 = new InsuranceAttribute(null, "Financial");
            InterestedPartyUnit partyUnit = new InterestedPartyUnit(null, "Financial", "Corporation – Limited liability", null);
            partyUnit.Location = new Location(null);
            partyUnit.Location.IsPublic = true;
            instatute.Name = "ANZ Bank New Zealand Ltd";
            instatute.OrganisationType = organisationType6;
            instatute.InsuranceAttributes.Add(insuranceAttribute6);
            instatute.OrganisationalUnits.Add(partyUnit);
            await _OrganisationRepository.AddAsync(instatute);

            Organisation instatute2 = new Organisation(null, Guid.NewGuid());
            OrganisationType organisationType7 = new OrganisationType("Corporation – Limited liability");
            InsuranceAttribute insuranceAttribute7 = new InsuranceAttribute(null, "Financial");
            InterestedPartyUnit partyUnit2 = new InterestedPartyUnit(null, "Financial", "Corporation – Limited liability", null);
            partyUnit2.Location = new Location(null);
            partyUnit2.Location.IsPublic = true;
            instatute2.Name = "ASB Bank Limited";
            instatute2.OrganisationType = organisationType7;
            instatute2.InsuranceAttributes.Add(insuranceAttribute7);
            instatute2.OrganisationalUnits.Add(partyUnit2);
            await _OrganisationRepository.AddAsync(instatute2);

            Organisation instatute3 = new Organisation(null, Guid.NewGuid());
            OrganisationType organisationType8 = new OrganisationType("Corporation – Limited liability");
            InsuranceAttribute insuranceAttribute8 = new InsuranceAttribute(null, "Financial");
            InterestedPartyUnit partyUnit3 = new InterestedPartyUnit(null, "Financial", "Corporation – Limited liability", null);
            partyUnit3.Location = new Location(null);
            partyUnit3.Location.IsPublic = true;
            instatute3.Name = "Australia and New Zealand Banking Group Limited";
            instatute3.OrganisationType = organisationType8;
            instatute3.InsuranceAttributes.Add(insuranceAttribute8);
            instatute3.OrganisationalUnits.Add(partyUnit3);
            await _OrganisationRepository.AddAsync(instatute3);

            Organisation instatute4 = new Organisation(null, Guid.NewGuid());
            OrganisationType organisationType9 = new OrganisationType("Corporation – Limited liability");
            InsuranceAttribute insuranceAttribute9 = new InsuranceAttribute(null, "Financial");
            InterestedPartyUnit partyUnit4 = new InterestedPartyUnit(null, "Financial", "Corporation – Limited liability", null);
            partyUnit4.Location = new Location(null);
            partyUnit4.Location.IsPublic = true;
            instatute4.Name = "Australia and New Zealand Banking Group Limited";
            instatute4.OrganisationType = organisationType9;
            instatute4.InsuranceAttributes.Add(insuranceAttribute9);
            instatute4.OrganisationalUnits.Add(partyUnit4);
            await _OrganisationRepository.AddAsync(instatute4);


            Organisation instatute5 = new Organisation(null, Guid.NewGuid());
            OrganisationType organisationType10 = new OrganisationType("Corporation – Limited liability");
            InsuranceAttribute insuranceAttribute10 = new InsuranceAttribute(null, "Financial");
            InterestedPartyUnit partyUnit5 = new InterestedPartyUnit(null, "Financial", "Corporation – Limited liability", null);
            partyUnit5.Location = new Location(null);
            partyUnit5.Location.IsPublic = true;
            instatute5.Name = "Australia and New Zealand Banking Group Limited";
            instatute5.OrganisationType = organisationType10;
            instatute5.InsuranceAttributes.Add(insuranceAttribute10);
            instatute5.OrganisationalUnits.Add(partyUnit5);
            await _OrganisationRepository.AddAsync(instatute5);
        }
    }
}

