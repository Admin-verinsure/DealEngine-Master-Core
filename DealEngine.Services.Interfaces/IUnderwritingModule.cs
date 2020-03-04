using System;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
	public interface IUnderwritingModule
	{
		string Name { get; }

		[Obsolete("Doesn't support the new Programme implementation")]
		bool Underwrite (User underwritingUser, ClientInformationSheet informationSheet);

		bool Underwrite (User underwritingUser, ClientInformationSheet informationSheet, Product product, string reference);
	}
}

