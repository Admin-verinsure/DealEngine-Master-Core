using System;
namespace TechCertain.Domain.Exceptions
{
	public class AuthenticationException : Exception
	{
		public string User { get; set; }

		public int ErrorCode { get; set; }

		public AuthenticationException (string message)
			: base (message)
		{
			User = "";
			ErrorCode = -1;
		}

		public override string ToString ()
		{
			return string.Format ("[AuthenticationException: {0}, User={1}, ErrorCode={2}]", Message, User, ErrorCode);
		}
	}
}

