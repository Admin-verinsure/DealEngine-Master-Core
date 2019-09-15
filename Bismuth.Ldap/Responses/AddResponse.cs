using Bismuth.Ldap.Utils;
using System.Net.Sockets;

namespace Bismuth.Ldap.Responses
{
	public class AddResponse : LdapResponse
	{
		public AddResponse (NetworkStream stream)
			: base (stream)
		{
			ReadResponse (new LdapStreamReader (stream), ProtocolOperation.AddResponse);
		}
	}
}

