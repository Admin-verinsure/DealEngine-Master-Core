using System;
namespace TechCertain.Domain.Exceptions
{
	public class ObjectNotFoundException : Exception
	{
		public ObjectNotFoundException (string message)
			: base (message)
		{

		}

		public ObjectNotFoundException (string message, Exception innerException)
			: base (message, innerException)
		{

		}
	}
}

