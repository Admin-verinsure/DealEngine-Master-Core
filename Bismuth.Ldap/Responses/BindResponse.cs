using Bismuth.Ldap.Utils;
using System.Net.Sockets;

namespace Bismuth.Ldap.Responses
{
	public class BindResponse : LdapResponse
	{
		public BindResponse (NetworkStream stream)
			: base (stream)
		{
			ReadResponse (new LdapStreamReader (stream), ProtocolOperation.BindResponse);
		}
	}
}

