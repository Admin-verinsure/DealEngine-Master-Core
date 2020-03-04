using System;
using System.IO;


namespace TechCertain.Infrastructure.Authorization
{
	public class TraceExtension : SoapExtension
	{
		/*
		 * Resources used
		 * https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/7w06t139(v=vs.100)
		 * https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/s25h0swd(v=vs.100)
		 */


		string filename;
		Stream oldStream;
		Stream newStream;

		public override Stream ChainStream (Stream stream)
		{
			oldStream = stream;
			newStream = new MemoryStream ();
			return newStream;
		}

		public override object GetInitializer (LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			return ((TraceExtensionAttribute)attribute).Filename;
		}

		public override object GetInitializer (Type serviceType)
		{
			return "/Users/james/" + serviceType.FullName + ".log";
		}

		public override void Initialize (object initializer)
		{
			filename = (string)initializer;
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

        void WriteResponse(SoapMessage soapMessage)
		{
			Copy (oldStream, newStream);
			var fs = new FileStream (filename, FileMode.Append, FileAccess.Write);
			var writer = new StreamWriter (fs);

			writer.WriteLine ("----- Response at " + DateTime.Now);
			writer.Flush ();
			newStream.Position = 0;

			Copy (newStream, fs);
			writer.Close ();
			newStream.Position = 0;
		}

		void WriteRequest(SoapMessage soapMessage)
		{
			newStream.Position = 0;
			var fs = new FileStream (filename, FileMode.Append, FileAccess.Write);
			var writer = new StreamWriter (fs);

			writer.WriteLine ("----- Request at " + DateTime.Now);
			writer.Flush ();
			Copy (newStream, fs);

			writer.Close ();
			newStream.Position = 0;
			Copy (newStream, oldStream);
		}

		void Copy (Stream fromStream, Stream toStream)
		{
			var reader = new StreamReader (fromStream);
			var writer = new StreamWriter (toStream);
			writer.WriteLine (reader.ReadToEnd ());
			writer.Flush ();
		}
	}
}

