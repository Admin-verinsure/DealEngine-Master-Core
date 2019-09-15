using System;
using System.Collections.Generic;
using TechCertain.Domain.Interfaces;
using TechCertain.Domain.Services.UnderwritingModules;

namespace TechCertain.Domain.Services.Factories
{
	public class UWMFactory : IUWMFactory
	{
		Dictionary<string, IUnderwritingModule> _modules;

        public UWMFactory ()
		{
            _modules = new Dictionary<string, IUnderwritingModule> ();
			// temp until parameter registration is working
			// and by temp we mean permanent until some intern does it a decade from now
			var modules = RegisterModules ();
			foreach (var module in modules)
				Register (module.Name, module);
		}

		public void Register (string key, IUnderwritingModule module)
		{
			_modules [key] = module;
		}

		public IUnderwritingModule Load (string key)
		{
			if (!_modules.ContainsKey (key))
				throw new Exception ("No underwriting module for \"" + key + "\" registered");

			return _modules [key];
		}

		protected IUnderwritingModule [] RegisterModules ()
		{
			var modules = new IUnderwritingModule [] {
				new ICIBHIANZUWModule(),
				new ICIBARCCOUWModule(),
                new MarshCoastGuardUWModule(),
                new EmptyUWModule(),
			};
			return modules;
		}
	}
}

