using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Infrastructure.Tasking
{
	public interface ITaskingService
	{
        Task<UserTask> CreateTaskFor(User createdBy, Organisation createdFor, string name, DateTime dueDate);
        Task<List<UserTask>> GetAllTasksFor(User user);
        Task<List<UserTask>> GetAllTasksFor (Organisation organisation);
        Task<UserTask> GetTask(Guid Id);
        Task CreateTaskFor(UserTask task);
    }
}

