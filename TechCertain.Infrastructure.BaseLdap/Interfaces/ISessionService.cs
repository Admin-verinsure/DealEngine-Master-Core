using Novell.Directory.Ldap;

namespace TechCertain.Infrastructure.BaseLdap.Interfaces
{
    public interface ISessionService
    {
        LdapConnection GetConnection();       
    }
}
