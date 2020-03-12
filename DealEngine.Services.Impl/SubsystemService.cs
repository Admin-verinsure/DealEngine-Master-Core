using DealEngine.Services.Interfaces;
using DealEngine.Domain.Entities;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;

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
        IMapper _mapper;

        public SubsystemService(
            IMapper mapper,
            IProductService productService,
            IInformationSectionService informationSectionService,
            IOrganisationService organisationService,
            IUserService userService,
            IProgrammeService programmeService,            
            IClientInformationService clientInformationService,
            IInformationTemplateService informationTemplateService)
        {
            _mapper = mapper;
            _productService = productService;
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
            try
            {
                await SubInformationTemplate(clientProgramme);
                foreach (var org in principalOrganisations)
                {
                    var subClientProgramme = await CreateSubClientProgramme(clientProgramme, sheet, org);
                    var subClientSheet = await CreateSubInformationSheet(subClientProgramme, sheet, org);
                    sheet.SubClientInformationSheets.Add(subClientSheet);
                }
                sheet.Status = "Started";                
                await _clientInformationService.UpdateInformation(sheet);
            }
            catch(Exception ex)
            {
                Exception subSystem = new Exception("Subsystem Failed", ex);                
                throw subSystem;
            }
        }

        private async Task<SubClientInformationSheet> CreateSubInformationSheet(SubClientProgramme subClientProgramme, ClientInformationSheet sheet, Organisation organisation)
        {
            try
            {
                var subSheet = await _clientInformationService.IssueSubInformationFor(sheet);
                subSheet.BaseClientInformationSheet = sheet;
                subSheet.Programme = subClientProgramme;
                subSheet.Status = "Not Started";
                subSheet.Owner = organisation;
                await _clientInformationService.UpdateInformation(subSheet);
                
                subClientProgramme.InformationSheet = subSheet;
                await _programmeService.Update(subClientProgramme);

                return subSheet;

            }
            catch (Exception ex)
            {
                Exception subSystem = new Exception("Create Sub Information Failed", ex);
                throw subSystem;
            }
        }

        private async Task<SubClientProgramme> CreateSubClientProgramme(ClientProgramme clientProgramme, ClientInformationSheet sheet, Organisation org)
        {
            try
            {
                var subClientProgramme = await _programmeService.CreateSubClientProgrammeFor(clientProgramme.Id);
                subClientProgramme.InformationSheet = sheet;
                subClientProgramme.Owner = org;
                clientProgramme.SubClientProgrammes.Add(subClientProgramme);
                await _programmeService.Update(clientProgramme);

                return subClientProgramme;
            }
            catch (Exception ex)
            {
                Exception subSystem = new Exception("Create Sub ClientProgramme Failed", ex);
                throw subSystem;
            }
        }

        private async Task SubInformationTemplate(ClientProgramme clientProgramme)
        {
            SubInformationTemplate subInformationTemplate = null;
            Product prod = null;
            try
            {                
                if (clientProgramme.BaseProgramme.Products.Count > 1)
                {
                    foreach (var prodMaster in clientProgramme.BaseProgramme.Products.Where(progp => progp.IsMasterProduct))
                    {
                        prod = prodMaster;
                        break;
                    }
                }
                else
                {
                    prod = clientProgramme.BaseProgramme.Products.FirstOrDefault();
                }
                
                SubInformationTemplate subInfomationTemplate = _mapper.Map<SubInformationTemplate>(prod.InformationTemplate);                
                subInfomationTemplate.BaseInformationTemplate = prod.InformationTemplate;
                List<InformationSection> sections = new List<InformationSection>();                 
                
                sections = await _informationSectionService.GetInformationSectionsbyTemplateId(subInfomationTemplate.BaseInformationTemplate.Id);
                
                foreach (var section in sections)
                {
                    section.Items = section.Items.OrderBy(i => i.ItemOrder).ToList();
                }
                //search any section using a where statement     
                int count = 1;
                var selectSections = sections.Where(s => s.Position == 1 || s.Position == 15).ToList();
                foreach(var section in selectSections)
                {
                    var newSection = _mapper.Map<InformationSection>(section);
                    newSection.Position = count;
                    subInfomationTemplate.Sections.Add(newSection);
                }
                
                prod.SubInformationTemplate = subInfomationTemplate;
                await _productService.UpdateProduct(prod);
            }
            catch (Exception ex)
            {
                Exception subSystem = new Exception("Create Sub Template Failed", ex);
                throw subSystem;
            }
        }
    }
}

