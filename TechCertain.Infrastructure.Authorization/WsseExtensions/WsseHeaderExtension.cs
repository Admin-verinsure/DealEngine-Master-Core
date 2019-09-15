using System;
using System.IO;


namespace TechCertain.Infrastructure.Authorization
{
	public class WsseHeaderExtension : SoapExtension
	{
		/*****************************************
		 * Code taken from
		 * https://forums.asp.net/t/1137408.aspx?Adding+information+to+the+SOAP+Header
		 * https://www.codeproject.com/Articles/19889/%2FArticles%2F19889%2FSOAP-Header-Extensions-How-To
		 *****************************************/

		Stream oldStream;
		Stream newStream;

		public WsseHeaderExtension ()
		{
		}

		public override Stream ChainStream (Stream stream)
		{
			oldStream = stream;
			newStream = new MemoryStream ();
			return newStream;
		}

		public override object GetInitializer (Type serviceType)
		{
			return null;
		}

		public override object GetInitializer (LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			return null;
		}

		public override void Initialize (object initializer)
		{
			
		}

		public override void ProcessMessage (SoapMessage message)
		{
			switch (message.Stage) {
			case SoapMessageStage.BeforeSerialize:
				break;
			case SoapMessageStage.AfterSerialize:
				WriteRequest (message);
				break;
			case SoapMessageStage.BeforeDeserialize:
				WriteResponse (message);
				break;
			case SoapMessageStage.AfterDeserialize:
				break;
			default:
				throw new Exception ("invalid stage");
			}
		}

		void WriteResponse (SoapMessage soapMessage)
		{
           /* System.Diagnostics.Debug.WriteLine(soapMessage);
            Copy (oldStream, newStream);
			newStream.Position = 0;
            */


		}

		void Copy (Stream fromStream, Stream toStream)
		{
			var reader = new StreamReader (fromStream);
			var writer = new StreamWriter (toStream);
			writer.WriteLine (reader.ReadToEnd ());
			writer.Flush ();
		}

		void WriteRequest (SoapMessage soapMessage)
		{
			// Get the SOAP body as a string, so we can manipulate...
			string soapBodyString = GetXmlFromCache ();

			// Strip off the old header stuff before the message body
			int bodyIndex = soapBodyString.IndexOf ("<soap:Body");  // yeah, it's pretty specific.  Tough.

			// Create the SOAP Message
			// It Comprises of a <soap:Element> that Enclosed a <soap:Body>.
			// Pack the XML  Document Inside the <soap:Body> Element 
			string rsaAuthUser = System.Web.Configuration.WebConfigurationManager.AppSettings ["RsaAuthUser"];
			string rsaAuthPw = System.Web.Configuration.WebConfigurationManager.AppSettings ["RsaAuthPw"];
            //soapBodyString = soapBodyString.Insert (bodyIndex, GetWsseHeader("MarshProdTestUser", "MarTst9tt$0Prd52xs"));//Production
            soapBodyString = soapBodyString.Insert (bodyIndex, GetWsseHeader("MarshNZSOAPUser", "MarNZ0sa$0Cap16us"));
            Console.WriteLine (soapBodyString);

			var writer = new StreamWriter (oldStream);
			writer.WriteLine (soapBodyString);
			writer.Flush ();
		}

		string GetXmlFromCache ()
		{
			newStream.Position = 0; // start at the beginning!
			string strSOAPresponse = ExtractFromStream (newStream);
			return strSOAPresponse;
		}

		// Transfer the text from the target
		// stream from the current position.
		string ExtractFromStream (Stream target)
		{
			if (target != null)
				return (new StreamReader (target)).ReadToEnd ();
			return "";
		}

		string GetWsseHeader (string username, string password)
		{
			UsernameToken usernameToken = new UsernameToken (username, password, PasswordOption.SendPlainText);

			var wsseHeader = @"<soap:Header>
				<wsse:Security soap:mustUnderstand=""1"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""> 
				<wsse:UsernameToken wsu:Id=""[TokenId]"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
			<wsse:Username>[Username]</wsse:Username><wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">[Password]</wsse:Password>
			<wsse:Nonce EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">[Nonce]</wsse:Nonce>
			<wsu:Created>[Created]</wsu:Created></wsse:UsernameToken></wsse:Security></soap:Header>";

            //testing changes to HEADER
            wsseHeader = wsseHeader.Replace ("[TokenId]", usernameToken.Id.Replace("SecurityToken", "UsernameToken"));
			wsseHeader = wsseHeader.Replace ("[Username]", usernameToken.Username);
			wsseHeader = wsseHeader.Replace ("[Password]", usernameToken.Password);
			wsseHeader = wsseHeader.Replace ("[Nonce]", Convert.ToBase64String(usernameToken.Nonce));
			wsseHeader = wsseHeader.Replace ("[Created]", usernameToken.Created.ToString("yyyy-MM-ddThh:mm:ss.fffZ"));

			return wsseHeader;

			//return "<soap:Header/>";
		}
	}
}

