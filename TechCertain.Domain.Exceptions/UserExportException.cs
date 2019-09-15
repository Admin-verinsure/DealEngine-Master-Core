using System;

namespace TechCertain.Domain.Exceptions
{
	public class UserExportException : Exception
	{
		public UserExportException (string message)
			: base(message)
		{

		}

		public UserExportException (string message, Exception innerException)
			: base (message, innerException)
		{
			
		}
	}
}

