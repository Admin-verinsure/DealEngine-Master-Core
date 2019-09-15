using TechCertain.Domain.Interfaces;

namespace TechCertain.Domain.Services.Factories
{
	public interface IUWMFactory
	{
		void Register (string key, IUnderwritingModule module);
		IUnderwritingModule Load (string key);
	}
}

