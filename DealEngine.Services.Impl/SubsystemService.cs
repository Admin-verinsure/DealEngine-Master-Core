﻿using DealEngine.Services.Interfaces;
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
        IProgrammeService _programmeService;
        IProductService _productService;
        IClientInformationService _clientInformationService;
        IInformationTemplateService _informationTemplateService;
        IInformationSectionService _informationSectionService;
        IReferenceService _referenceService;
        IEmailService _emailService;
        IMapper _mapper;

        public SubsystemService(
            IEmailService emailService,
            IMapper mapper,
            IProductService productService,
            IInformationSectionService informationSectionService,
            IOrganisationService organisationService,
            IProgrammeService programmeService,            
            IClientInformationService clientInformationService,
            IInformationTemplateService informationTemplateService,
            IReferenceService referenceService
            )
        {
            _emailService = emailService;
            _referenceService = referenceService;
            _mapper = mapper;
            _productService = productService;
            _informationSectionService = informationSectionService;
            _informationTemplateService = informationTemplateService;
            _programmeService = programmeService;
            _organisationService = organisationService;        
            _clientInformationService = clientInformationService;
        }

        public async Task CreateSubObjects(Guid clientProgrammeId, ClientInformationSheet sheet, User user)
        {
            var principalOrganisations = await _organisationService.GetSubsystemOrganisationPrincipals(sheet);
            var clientProgramme = await _programmeService.GetClientProgrammebyId(clientProgrammeId);
            try
            {
                //await SubInformationTemplate(clientProgramme);
                foreach (var org in principalOrganisations)
                {
                    var subClientSheet = await CreateSubObjectProcess(clientProgramme, sheet, org);
                    sheet.SubClientInformationSheets.Add(subClientSheet);
                }
                if(principalOrganisations.Count != 0)
                {
                    sheet.Status = "Submitted";
                    sheet.SubmitDate = DateTime.Now;
                    sheet.SubmittedBy = user;
                }
                             
                await _clientInformationService.UpdateInformation(sheet);
            }
            catch(Exception ex)
            {
                Exception subSystem = new Exception("Subsystem Failed", ex);                
                throw subSystem;
            }
        }

        private async Task<SubClientInformationSheet> CreateSubObjectProcess(ClientProgramme clientProgramme, ClientInformationSheet sheet, Organisation org)
        {

            var subClientSheet = await _clientInformationService.GetSubInformationSheetFor(org);
            if (subClientSheet == null)
            {
                var subClientProgramme = await CreateSubClientProgramme(clientProgramme, sheet, org);
                subClientSheet = await CreateSubInformationSheet(subClientProgramme, sheet, org);
            }
            else
            {
                if (subClientSheet.DateDeleted.HasValue || subClientSheet.DeletedBy != null)
                {
                    SubClientProgramme subProg = (SubClientProgramme)subClientSheet.Programme;
                    subClientSheet.DateDeleted = null;
                    subClientSheet.Programme.DateDeleted = null;
                    subClientSheet.DeletedBy = null;
                    subClientSheet.Programme.DeletedBy = null;

                    clientProgramme.SubClientProgrammes.Add(subProg);
                }
            }

            await _emailService.SendSystemEmailAllSubUISInstruction(org, subClientSheet.Programme.BaseProgramme, subClientSheet);

            return subClientSheet;
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
                subSheet.ReferenceId = await _referenceService.GetLatestReferenceId();
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
                var selectSections = sections.Where(s => s.Position == 1 || s.Position == 9 || s.Position == 11).ToList();
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

        public async Task ValidateSubObjects(ClientInformationSheet informationSheet)
        {
            var principalOrganisations = await _organisationService.GetSubsystemOrganisationPrincipals(informationSheet);

            for (var i = informationSheet.SubClientInformationSheets.Count - 1; i >= 0; i--)
            {
                RemoveSubObjects(informationSheet, informationSheet.SubClientInformationSheets[i]);
            }
            
            if(principalOrganisations.Count > 0)
            {
                List<SubClientInformationSheet> subSheets = new List<SubClientInformationSheet>();

                foreach (var principal in principalOrganisations)
                {
                    var subSheet = await CreateSubObjectProcess(informationSheet.Programme, informationSheet, principal);

                    subSheets.Add(subSheet);
                }

                foreach (var subsheet in subSheets)
                {
                    informationSheet.SubClientInformationSheets.Add(subsheet);
                }
            }

            await _clientInformationService.UpdateInformation(informationSheet);
        }

        private void RemoveSubObjects(ClientInformationSheet informationSheet, SubClientInformationSheet subsheet)
        {
            SubClientProgramme subProg = (SubClientProgramme)subsheet.Programme;

            subsheet.Delete(informationSheet.Organisation.FirstOrDefault().CreatedBy, DateTime.Now);
            subProg.Delete(informationSheet.Organisation.FirstOrDefault().CreatedBy, DateTime.Now);

            try
            {
                informationSheet.Programme.SubClientProgrammes.Remove(subProg);
                informationSheet.SubClientInformationSheets.Remove(subsheet);
            }
            catch(Exception ex)
            {
                Exception subSystem = new Exception("Remove Subobjects Failed", ex);
                throw subSystem;
            }
        }
    }
}

