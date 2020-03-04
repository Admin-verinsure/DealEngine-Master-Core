﻿using NHibernate.Linq;
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

		public TaskingService (IMapperSession<UserTask> taskRespository)
		{
			_taskRespository = taskRespository;
		}

        public async Task<UserTask> CreateTaskFor(User createdBy, Organisation createdFor, DateTime dueDate)
        {
            if (createdBy == null)
                throw new ArgumentNullException(nameof(createdBy));
            if (createdFor == null)
                throw new ArgumentNullException(nameof(createdFor));
            if (dueDate == null)
                throw new ArgumentNullException(nameof(dueDate));

            UserTask task = new UserTask(createdBy, createdFor, dueDate);
            await _taskRespository.AddAsync(task);
            return task;
        }

        public async Task<List<UserTask>> GetAllTasksFor(Organisation organisation)
		{
			if (organisation == null)
				throw new ArgumentNullException (nameof (organisation));
			
			return await _taskRespository.FindAll().Where(t => t.For == organisation).ToListAsync();			
		}

		public async Task<List<UserTask>> GetAllTasksFor(User user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof (user));

			List<UserTask> tasks = new List<UserTask>();
            List<UserTask> list;
            foreach (Organisation org in user.Organisations)
            {
                list = await GetAllTasksFor(org);
                tasks.AddRange(list);
            }
                                
            return tasks;
		}

        public async Task<UserTask> GetTask(Guid Id)
        {
            return await _taskRespository.GetByIdAsync(Id);            
        }

		public async Task CreateTaskFor(UserTask task)
		{
			if (task == null)
				throw new ArgumentNullException (nameof (task));
            await _taskRespository.AddAsync(task);			
		}

        public async Task<List<UserTask>> GetUserTasksByMilestone(Milestone milestone)
        {
            return await _taskRespository.FindAll().Where(t => t.Milestone == milestone).ToListAsync();            
        }

        public async Task UpdateUserTask(UserTask userTask)
        {
           await _taskRespository.UpdateAsync(userTask);
        }

    }
}

