using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Infrastructure.Tasking
{
	public interface ITaskingService
	{
		/// <summary>
		/// Creates a new Task for the specified Organisation.
		/// <para>This Task is automatically added to the database.</para>
		/// </summary>
		/// <returns>The new task.</returns>
		/// <param name="createdBy">User who created the task.</param>
		/// <param name="createdFor">Task the Organisation was created for.</param>
		/// <param name="name">Name of the Task.</param>
		/// <param name="dueDate">Date the task needs to be completed by.</param>
		//UserTask CreateTaskFor (User createdBy, Organisation createdFor, string name, DateTime dueDate);

		IList<UserTask> GetAllTasksFor (User user);

		IList<UserTask> GetAllTasksFor (Organisation organisation);

        UserTask GetTask(Guid Id);

        UserTask CreateTaskFor(UserTask task);
	}
}

