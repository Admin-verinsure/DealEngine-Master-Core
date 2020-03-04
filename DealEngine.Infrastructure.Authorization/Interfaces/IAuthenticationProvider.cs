
namespace TechCertain.Infrastructure.Authorization
{
	public interface IAuthenticationProvider
	{
		bool Authenticate (string username, string password);

		bool Authenticate (string username, string password, string token);

		bool AuthenticateWithExternal (string username, string password, string token, string providerType);
	}
}

