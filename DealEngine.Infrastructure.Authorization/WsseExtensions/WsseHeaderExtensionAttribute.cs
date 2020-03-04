using System;

namespace TechCertain.Infrastructure.Authorization
{
	[AttributeUsage (AttributeTargets.Method)]
	public class WsseHeaderExtensionAttribute : SoapExtensionAttribute
	{
		public override Type ExtensionType {
			get {
				return typeof (WsseHeaderExtension);
			}
		}

		public override int Priority { get; set; }

		public WsseHeaderExtensionAttribute ()
		{
		}
	}
}

