using System;
using System.Web.Services.Protocols;

namespace TechCertain.Infrastructure.Authorization
{
	[AttributeUsage (AttributeTargets.Method)]
	public class TraceExtensionAttribute : SoapExtensionAttribute
	{
		public override Type ExtensionType {
			get {
				return typeof (TraceExtension);
			}
		}

		public override int Priority { get; set; }

		public string Filename { get; set; }

		public TraceExtensionAttribute ()
		{
			Filename = "/tmp/soapTrace.log";
            //Filename = "C:\\Users\\Public\\soapTrace.log";

        }
	}
}

