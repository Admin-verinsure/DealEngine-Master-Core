using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.Tasking;
using System.Threading.Tasks;

namespace TechCertain.Services.Impl
{
    public class MilestoneService : IMilestoneService
    {
        IUnitOfWork _unitOfWork;
        IMapperSession<Milestone> _milestoneRepository;
        ISystemEmailService _systemEmailRepository;
        //ITaskingService _taskingService;
        IMilestoneTemplateService _milestoneTemplateService;


        public MilestoneService(IUnitOfWork unitOfWork,
                                IMapperSession<Milestone> milestoneRepository,
                                ISystemEmailService systemEmailService,
                                //ITaskingService taskingService,
                                IMilestoneTemplateService milestoneTemplateService)
        {
            _unitOfWork = unitOfWork;
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
            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                _milestoneRepository.Add(milestone);
                uow.Commit();
            }

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
            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                _milestoneRepository.Update(milestone);
                uow.Commit();
            }

        }

        public void CreateAdvisory(Milestone milestone, string advisory)
        {
            if (milestone == null)
                throw new ArgumentNullException(nameof(milestone));
            if (string.IsNullOrWhiteSpace(advisory))
                throw new ArgumentNullException(nameof(advisory));

            milestone.Advisory = new Advisory(advisory);
            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                _milestoneRepository.Update(milestone);
                uow.Commit();
            }
        }

        public void CreateUserTask(Milestone milestone, UserTask userTask)
        {
            if (milestone == null)
                throw new ArgumentNullException(nameof(milestone));
            if (userTask == null)
                throw new ArgumentNullException(nameof(userTask));

           //milestone.Task = _taskingService.CreateTaskFor(userTask);
            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                _milestoneRepository.Update(milestone);
                uow.Commit();
            }
        }

        public MilestoneTemplate GetMilestoneTemplate(Guid ClientprogrammeID, string milestoneActivity)
        {
            MilestoneTemplate milestoneTemplate = _milestoneTemplateService.GetMilestoneTemplate(ClientprogrammeID, milestoneActivity);

            return milestoneTemplate;
        }

        public Task CloseMileTask(Guid id, string method)
        {
            Milestone milestone = _milestoneRepository.GetById(id);
            milestone.Advisory.Method = method;
            milestone.HasTriggered = true;
            milestone.Task.IsActive = true;
            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                _milestoneRepository.Update(milestone);
                uow.Commit();
            }

            return null;
        }

        public Milestone GetMilestoneProcess(Guid programmeId, string programmeProcess, string Activity)
        {
            return _milestoneRepository.FindAll().FirstOrDefault(m => m.ProgrammeProcess == programmeProcess
                                                                                            && m.Activity == Activity
                                                                                                && m.HasTriggered == false);
        }
    }
}
