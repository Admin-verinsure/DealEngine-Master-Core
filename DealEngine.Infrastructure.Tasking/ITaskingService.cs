using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Infrastructure.Tasking
{
	public interface ITaskingService
	{
        Task<List<UserTask>> GetAllActiveTasksFor(Organisation organisation);
        Task Update(UserTask userTask);
        Task CreateTask(UserTask task);
        Task JoinOrganisationTask(User user, Organisation organisation);
    }
}

