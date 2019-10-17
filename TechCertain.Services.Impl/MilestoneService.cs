using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.Tasking;
using System.Threading.Tasks;

namespace TechCertain.Services.Impl
{
    public class MilestoneService : IMilestoneService
    {
        IMapperSession<Milestone> _milestoneRepository;
        ISystemEmailService _systemEmailRepository;
        //ITaskingService _taskingService;
        IMilestoneTemplateService _milestoneTemplateService;


        public MilestoneService(IMapperSession<Milestone> milestoneRepository,
                                ISystemEmailService systemEmailService,
                                //ITaskingService taskingService,
                                IMilestoneTemplateService milestoneTemplateService)
        {
            _milestoneRepository = milestoneRepository;
            _systemEmailRepository = systemEmailService;
            //_taskingService = taskingService;
            _milestoneTemplateService = milestoneTemplateService;
        }

        public Milestone CreateMilestone(User createdBy, string programmeProcess, string activity, Programme programmeId)
        {
            Milestone milestone = new Milestone(createdBy);
            milestone.ProgrammeProcess = programmeProcess;
            milestone.Activity = activity;
            milestone.HasTriggered = false;
            milestone.Programme = programmeId;
            _milestoneRepository.AddAsync(milestone);

            return milestone;
        }


        public Milestone GetMilestone(string type)
        {
            return _milestoneRepository.FindAll().FirstOrDefault(m => m.Type == type);
        }

        public void CreateEmailTemplate(User user, Milestone milestone, string subject, string emailContent, string template)
        {

            if (milestone == null)
                throw new ArgumentNullException(nameof(milestone));
            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentNullException(nameof(template));
            if (string.IsNullOrWhiteSpace(emailContent))
                throw new ArgumentNullException(nameof(emailContent));

            SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType(template);
            if (systemEmailTemplate != null)
            {
                _systemEmailRepository.AddNewSystemEmail(user, milestone.Type, null, subject, emailContent, template);
            }
            else
            {
                //update function
            }

            milestone.EmailTemplates.Add(systemEmailTemplate);
            _milestoneRepository.UpdateAsync(milestone);
        }

        public void CreateAdvisory(Milestone milestone, string advisory)
        {
            if (milestone == null)
                throw new ArgumentNullException(nameof(milestone));
            if (string.IsNullOrWhiteSpace(advisory))
                throw new ArgumentNullException(nameof(advisory));

            milestone.Advisory = new Advisory(advisory);
            _milestoneRepository.UpdateAsync(milestone);
        }

        public void CreateUserTask(Milestone milestone, UserTask userTask)
        {
            if (milestone == null)
                throw new ArgumentNullException(nameof(milestone));
            if (userTask == null)
                throw new ArgumentNullException(nameof(userTask));

            _milestoneRepository.UpdateAsync(milestone);
        }

        public MilestoneTemplate GetMilestoneTemplate(Guid ClientprogrammeID, string milestoneActivity)
        {
            MilestoneTemplate milestoneTemplate = _milestoneTemplateService.GetMilestoneTemplate(ClientprogrammeID, milestoneActivity);

            return milestoneTemplate;
        }

        public Task CloseMileTask(Guid id, string method)
        {
            Milestone milestone = _milestoneRepository.GetByIdAsync(id).Result;
            milestone.Advisory.Method = method;
            milestone.HasTriggered = true;
            milestone.Task.IsActive = true;
            _milestoneRepository.UpdateAsync(milestone);

            return Task.CompletedTask;
        }

        public Milestone GetMilestoneProcess(Guid programmeId, string programmeProcess, string Activity)
        {
            return _milestoneRepository.FindAll().FirstOrDefault(m => m.ProgrammeProcess == programmeProcess
                                                                                            && m.Activity == Activity
                                                                                                && m.HasTriggered == false);
        }
    }
}
