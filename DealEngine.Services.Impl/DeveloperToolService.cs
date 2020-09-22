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



            //Organisation marina1 = new Organisation(null, Guid.NewGuid());
            //OrganisationType organisationType = new OrganisationType("Corporation – Limited liability");
            //InsuranceAttribute insuranceAttribute = new InsuranceAttribute(null, "Marina");
            //MarinaUnit marinaUnit = new MarinaUnit(null, "Marina", "Corporation – Limited liability", null);
            //WaterLocation defaultMar1 = new WaterLocation(null);
            //defaultMar1.MarinaName = "Hobsonville";
            //defaultMar1.IsPublic = true;
            //marinaUnit.WaterLocation = defaultMar1;
            //marina1.OrganisationType = organisationType;
            //marina1.InsuranceAttributes.Add(insuranceAttribute);
            //marina1.OrganisationalUnits.Add(marinaUnit);

            //await _OrganisationRepository.AddAsync(marina1);

            //Organisation marina2 = new Organisation(null, Guid.NewGuid());
            //OrganisationType organisationType2 = new OrganisationType("Corporation – Limited liability");
            //InsuranceAttribute insuranceAttribute2 = new InsuranceAttribute(null, "Marina");
            //MarinaUnit marinaUnit2 = new MarinaUnit(null, "Marina", "Corporation – Limited liability", null);
            //WaterLocation DefaultMar2 = new WaterLocation(null);
            //DefaultMar2.MarinaName = "Pine Harbour";
            //DefaultMar2.IsPublic = true;
            //marinaUnit2.WaterLocation = DefaultMar2;
            //marina2.OrganisationType = organisationType2;
            //marina2.InsuranceAttributes.Add(insuranceAttribute2);
            //marina2.OrganisationalUnits.Add(marinaUnit2);

            //await _OrganisationRepository.AddAsync(marina2);


            //Organisation marina3 = new Organisation(null, Guid.NewGuid());
            //OrganisationType organisationType3 = new OrganisationType("Corporation – Limited liability");
            //InsuranceAttribute insuranceAttribute3 = new InsuranceAttribute(null, "Marina");
            //MarinaUnit marinaUnit3 = new MarinaUnit(null, "Marina", "Corporation – Limited liability", null);
            //WaterLocation DefaultMar3 = new WaterLocation(null);
            //DefaultMar3.MarinaName = "West Haven";
            //DefaultMar3.IsPublic = true;
            //marinaUnit3.WaterLocation = DefaultMar3;
            //marina3.OrganisationType = organisationType3;
            //marina3.InsuranceAttributes.Add(insuranceAttribute3);
            //marina3.OrganisationalUnits.Add(marinaUnit3);

            //await _OrganisationRepository.AddAsync(marina3);


            //Organisation marina4 = new Organisation(null, Guid.NewGuid());
            //OrganisationType organisationType4 = new OrganisationType("Corporation – Limited liability");
            //InsuranceAttribute insuranceAttribute4 = new InsuranceAttribute(null, "Marina");
            //MarinaUnit marinaUnit4 = new MarinaUnit(null, "Marina", "Corporation – Limited liability", null);
            //WaterLocation DefaultMar4 = new WaterLocation(null);
            //DefaultMar4.MarinaName = "Silo Marina";
            //DefaultMar4.IsPublic = true;
            //marinaUnit4.WaterLocation = DefaultMar4;
            //marina4.OrganisationType = organisationType4;
            //marina4.InsuranceAttributes.Add(insuranceAttribute4);
            //marina4.OrganisationalUnits.Add(marinaUnit4);

            //await _OrganisationRepository.AddAsync(marina4);

            //Organisation marina5 = new Organisation(null, Guid.NewGuid());
            //OrganisationType organisationType5 = new OrganisationType("Corporation – Limited liability");
            //InsuranceAttribute insuranceAttribute5 = new InsuranceAttribute(null, "Marina");
            //MarinaUnit marinaUnit5 = new MarinaUnit(null, "Marina", "Corporation – Limited liability", null);
            //WaterLocation DefaultMar5 = new WaterLocation(null);
            //DefaultMar5.MarinaName = "Clyde Quay Boat Harbour";
            //DefaultMar5.IsPublic = true;
            //marinaUnit5.WaterLocation = DefaultMar5;
            //marina5.OrganisationType = organisationType5;
            //marina5.InsuranceAttributes.Add(insuranceAttribute5);
            //marina5.OrganisationalUnits.Add(marinaUnit5);

            //await _OrganisationRepository.AddAsync(marina5);



        }
    }
}

