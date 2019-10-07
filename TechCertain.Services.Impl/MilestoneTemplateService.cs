using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Services.Impl
{
    public class MilestoneTemplateService : IMilestoneTemplateService
    {
        IUnitOfWork _unitOfWork;
        IMapperSession<MilestoneTemplate> _milestoneTemplateRepository;

        public MilestoneTemplateService(IUnitOfWork unitOfWork, IMapperSession<MilestoneTemplate> milestoneTemplateRepository)
        {
            _unitOfWork = unitOfWork;
            _milestoneTemplateRepository = milestoneTemplateRepository;
        }

        public MilestoneTemplate GetMilestoneTemplate(Guid clientprogrammeID, string milestoneActivity)
        {
            MilestoneTemplate milestoneTemplate = _milestoneTemplateRepository.FindAll().FirstOrDefault(m => m.ClientProgramme == clientprogrammeID && m.Activity == milestoneActivity);
            if (milestoneTemplate == null)
            {
                milestoneTemplate = CreateMilestoneTemplate(clientprogrammeID, milestoneActivity);
            }
            return milestoneTemplate;
        }

        public MilestoneTemplate CreateMilestoneTemplate(Guid clientprogrammeID, string milestoneActivity)
        {
            MilestoneTemplate milestoneTemplate = new MilestoneTemplate();
            milestoneTemplate.ClientProgramme = clientprogrammeID;
            milestoneTemplate.Activity = milestoneActivity;
            milestoneTemplate.Templates = new List<string>();
            milestoneTemplate.Templates.Add("Agreement Status - Not Started");
            milestoneTemplate.Templates.Add("Agreement Status - Started");
            milestoneTemplate.Templates.Add("Agreement Status - Completed");
            milestoneTemplate.Templates.Add("Agreement Status - Quoted");
            milestoneTemplate.Templates.Add("Agreement Status - Declined");
            milestoneTemplate.Templates.Add("Agreement Status - Bound and Waiting Payment");
            milestoneTemplate.Templates.Add("Agreement Status - Bound and Waiting Invoice");

            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                _milestoneTemplateRepository.Add(milestoneTemplate);
                uow.Commit();
            }
            return milestoneTemplate;
        }
    }
}
