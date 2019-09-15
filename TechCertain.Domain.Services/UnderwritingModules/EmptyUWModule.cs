using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Domain.Services
{
	/// <summary>
	/// Empty Underwiring Module.
	/// </summary>
	public class EmptyUWModule : IUnderwritingModule
	{
		public EmptyUWModule ()
		{
		}

		public string Name {
			get {
				return "NoUnderwrite";
			}
		}

		public bool Underwrite (User underwritingUser, ClientInformationSheet informationSheet)
		{
			return true;
		}

		public bool Underwrite (User underwritingUser, ClientInformationSheet informationSheet, Product product, string reference)
		{
			return true;
		}
	}
}

