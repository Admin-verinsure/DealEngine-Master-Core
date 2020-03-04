using DealEngine.Services.Interfaces;
using DealEngine.Domain.Entities;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DealEngine.Services.Impl
{
    public class SubsystemService : ISubsystemService
    {
        IOrganisationService _organisationService;
        IUserService _userService;
        IProgrammeService _programmeService;
        IProductService _productService;
        IClientInformationService _clientInformationService;
        IInformationTemplateService _informationTemplateService;
        IInformationSectionService _informationSectionService;

        public SubsystemService(
            IInformationSectionService informationSectionService,
            IOrganisationService organisationService,
            IUserService userService,
            IProgrammeService programmeService,            
            IClientInformationService clientInformationService,
            IInformationTemplateService informationTemplateService)
        {
            _informationSectionService = informationSectionService;
            _informationTemplateService = informationTemplateService;
            _programmeService = programmeService;
            _organisationService = organisationService;
            _userService = userService;            
            _clientInformationService = clientInformationService;
        }

        public async Task CreateSubObjects(Guid clientProgrammeId, ClientInformationSheet sheet)
        {
            var principalOrganisations = await _organisationService.GetOrganisationPrincipals(sheet);
            var clientProgramme = await _programmeService.GetClientProgrammebyId(clientProgrammeId);
            foreach (var org in principalOrganisations)
            {
                //var subInformationTemplate = await CreateSubInformationTemplate(sheet);
                var user = await _userService.GetUserByOrganisation(org);
                var subProduct = await CreateSubProduct(clientProgramme);
                var subClientProgramme = await CreateSubClientProgramme(clientProgramme, org);                          
                var subsheet = await CreateSubInformationSheet(subClientProgramme, sheet);                
            }            
        }

        private async Task<SubClientInformationSheet> CreateSubInformationSheet(SubClientProgramme subClientProgramme, ClientInformationSheet sheet)
        {
            var subSheet = await _clientInformationService.IssueSubInformationFor(subClientProgramme);
            sheet.SubClientInformationSheets.Add(subSheet);            
            await _clientInformationService.UpdateInformation(sheet);

            return subSheet;
        }

        private async Task<SubClientProgramme> CreateSubClientProgramme(ClientProgramme clientProgramme, Organisation org)
        {
            var subClientProgramme = await _programmeService.CreateSubClientProgrammeFor(clientProgramme.Id, org);
            clientProgramme.SubClientProgrammes.Add(subClientProgramme);
            await _programmeService.Update(clientProgramme);

            return subClientProgramme;
        }

        private async Task<SubProduct> CreateSubProduct(ClientProgramme clientProgramme)
        {
            SubProduct subProduct = null;
            Product prod = null;
            try
            {                
                if (clientProgramme.BaseProgramme.Products.Count > 1)
                {
                    foreach (var prodMaster in clientProgramme.BaseProgramme.Products.Where(progp => progp.IsMasterProduct))
                    {
                        prod = prodMaster;
                        subProduct = new SubProduct(prod);
                        break;
                    }
                }
                else
                {
                    prod = clientProgramme.BaseProgramme.Products.FirstOrDefault();
                    subProduct = new SubProduct(prod); 
                }
                subProduct.CopyProduct(prod);
                List<InformationTemplate> informationTemplate = await _informationTemplateService.GetAllTemplatesbyproduct(subProduct.BaseProduct.Id);                
                if (informationTemplate.Count > 1)
                {
                    throw new Exception("informationTemplate cannot have a count higher than 1");
                }
                var template = informationTemplate.FirstOrDefault();
                List<InformationSection> sections = new List<InformationSection>();
                InformationTemplate subInfomationSheet = new InformationTemplate(template.CreatedBy, template.Name);
                
                sections = await _informationSectionService.GetInformationSectionsbyTemplateId(template.Id);
                //search any section using a where statement
                foreach (var section in sections)
                {
                    section.Items = section.Items.OrderBy(i => i.ItemOrder).ToList();
                }
                                               
                subInfomationSheet.Sections = sections.Where(s => s.Position == 1 || s.Position == 2 || s.Position == 15).ToList();
                subProduct.InformationTemplate = subInfomationSheet;
                await _productService.AddSubProduct(subProduct);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }       

            return subProduct;
        }
    }
}

