using System;
namespace DealEngine.Domain.Exceptions
{
	public class NoUserFoundException : Exception
	{
		public NoUserFoundException (string message)
			: base (message)
		{

		}
	}

	public class MultipleUsersFoundException : Exception
	{
		public MultipleUsersFoundException (string message)
			: base (message)
		{

		}
	}
}

