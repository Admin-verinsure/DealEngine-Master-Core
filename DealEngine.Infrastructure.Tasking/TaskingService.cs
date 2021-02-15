using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;

namespace DealEngine.Infrastructure.Tasking
{
	public class TaskingService : ITaskingService
	{
        IMapperSession<UserTask> _taskRespository;
        //Services.Interfaces.IMilestoneService _milestoneService;

        public TaskingService (IMapperSession<UserTask> taskRespository)
		{
			_taskRespository = taskRespository;

        }

        public async Task<List<UserTask>> GetAllActiveTasksFor(Organisation organisation)
        {
            //if (organisation == null)
                throw new ArgumentNullException(nameof(organisation));

            //return await _taskRespository.FindAll().Where(t => t.For == organisation && t.Completed == false).ToListAsync();
        }

        public async Task Update(UserTask userTask)
        {
           await _taskRespository.UpdateAsync(userTask);
        }

        public async Task CreateTask(UserTask task)
        {
            await _taskRespository.AddAsync(task);
        }

        public async Task JoinOrganisationTask(User user, Organisation organisation)
        {
            //await _milestoneService.CreateMilestone("Rejoin");
            throw new NotImplementedException();
        }
    }
}

