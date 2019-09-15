using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;


namespace TechCertain.Infrastructure.Tasking
{
	public class TaskingService : ITaskingService
	{
		IUnitOfWorkFactory _unitOfWork;
		IRepository<UserTask> _taskRespository;

		public TaskingService (IUnitOfWorkFactory unitOfWork, IRepository<UserTask> taskRespository)
		{
			_unitOfWork = unitOfWork;
			_taskRespository = taskRespository;
		}

		//public UserTask CreateTaskFor (User createdBy, Organisation createdFor, string name, DateTime dueDate)
		//{
		//	if (createdBy == null)
		//		throw new ArgumentNullException (nameof (createdBy));
		//	if (createdFor == null)
		//		throw new ArgumentNullException (nameof (createdFor));
		//	if (string.IsNullOrWhiteSpace(name))
		//		throw new ArgumentNullException (nameof (name));
		//	if (dueDate == null)
		//		throw new ArgumentNullException (nameof (dueDate));

		//	UserTask task = new UserTask (createdBy, createdFor, name, dueDate);
		//	UpdateTask (task);
		//	return task;
		//}

		public IList<UserTask> GetAllTasksFor (Organisation organisation)
		{
			if (organisation == null)
				throw new ArgumentNullException (nameof (organisation));
			
			var tasks = _taskRespository.FindAll ().Where (t => t.For == organisation);
			return tasks.ToList ();
		}

		public IList<UserTask> GetAllTasksFor (User user)
		{
			if (user == null)
				throw new ArgumentNullException (nameof (user));

			List<UserTask> tasks = new List<UserTask> ();
			foreach (Organisation org in user.Organisations)
				tasks.AddRange (_taskRespository.FindAll ().Where (t => t.For == org));
			return tasks;
		}

        public UserTask GetTask(Guid Id)
        {
            var task = _taskRespository.FindAll().FirstOrDefault(t => t.Id == Id);
            return task;
        }

		public UserTask CreateTaskFor(UserTask task)
		{
			if (task == null)
				throw new ArgumentNullException (nameof (task));

			using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork ()) {
				_taskRespository.Add (task);
				uow.Commit ();
			}
			return task;
		}
	}
}

