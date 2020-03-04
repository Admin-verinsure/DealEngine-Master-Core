using System;
namespace TechCertain.Infrastructure.Authorization
{
	public interface IExternalAuthenticationProvider
	{
		string Name { get; }
	}
}

