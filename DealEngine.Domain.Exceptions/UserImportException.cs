using System;

namespace DealEngine.Domain.Exceptions
{
	public class UserImportException : Exception
	{
		public UserImportException (string message)
			: base(message)
		{
			
		}

		public UserImportException (string message, Exception innerException)
			: base (message, innerException)
		{

		}

//		public override string ToString ()
//		{
//			string message = string.Format ("{0}: {1}", this.GetType ().FullName, Message);
//			if (InnerException != null)
//				message = string.Format ("{0} ---> {1}", message, InnerException.ToString ());
//
//			return message;
//		}
	}
}

