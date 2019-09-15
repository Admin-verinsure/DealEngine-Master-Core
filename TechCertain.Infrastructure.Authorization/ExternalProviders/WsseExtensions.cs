using System;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.Web.Services2.Security.Tokens;
using TechCertain.Infrastructure.Authorization;

namespace MarshRsaBinding
{
	//public partial class AdaptiveAuthenticationSoapBinding
	//{
	//	[XmlIgnore]
	//	public WsseUsernameToken UsernameToken { get; set; }

	//	protected override System.Xml.XmlWriter GetWriterForMessage (System.Web.Services.Protocols.SoapClientMessage message, int bufferSize)
	//	{
	//		//message.Headers.Add (new UsernameSoapHeader ("Username"));
	//		//message.Headers.Add (new PasswordSoapHeader ("Password"));
	//		//message.Headers.Add (new MessageIDSoapHeader (MessageID));
	//		message.Headers.Add (new WsseSecurityHeader (UsernameToken));

	//		return base.GetWriterForMessage (message, bufferSize);
	//	}
	//}
}

namespace TechCertain.Infrastructure.Authorization
{
	[XmlRoot ("Security")]
	[XmlType (Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", TypeName = "wsse")]
	public class WsseSecurityHeader : SoapHeader
	{
		[XmlElement("UsernameToken")]
		public WsseUsernameToken UsernameToken { get; set; }

		public WsseSecurityHeader () { }

		public WsseSecurityHeader (WsseUsernameToken usernameToken)
		{
			UsernameToken = usernameToken;
		}
	}

	[XmlType (Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", TypeName = "wsse")]
	[XmlRoot("UsernameToken")]
	public class WsseUsernameToken
	{
		[XmlAttribute (Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
		public string Id { get; set; }
		
		[XmlElement ("Username")]
		public string Username { get; set; }

		[XmlElement ("Password")]
		public WssePassword Password { get; set; }

		[XmlElement ("Nonce")]
		public WsseNonce Nonce { get; set; }

		[XmlElement ("Created")]
		public string Created { get; set; }

		public WsseUsernameToken () { }

		public WsseUsernameToken (UsernameToken token)
		{
			Username = token.Username;
			Password = new WssePassword () { Value = token.Password };
			Nonce = new WsseNonce () { Value = token.Nonce.ToString() };
			Created = token.Created.ToString ("O");
		}
	}

	public class WssePassword
	{
		[XmlAttribute]
		public string Type { get; set; }

		[XmlText]
		public string Value { get; set; }

		public WssePassword ()
		{
			Type = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText";
		}
	}

	[XmlType(TypeName = "wsu")]
	[XmlRoot]
	public class WsseNonce
	{
		[XmlAttribute]
		public string EncodingType { get; set; }

		[XmlText]
		public string Value { get; set; }

		public WsseNonce ()
		{
			EncodingType = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary";
		}
	}
}

