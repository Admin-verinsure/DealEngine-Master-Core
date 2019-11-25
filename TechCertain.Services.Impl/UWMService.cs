using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Services;
using TechCertain.Services.Impl.UnderwritingModuleServices;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class UWMService : IUWMService
    {

        IClientAgreementService _clientAgreementService;
        IClientAgreementRuleService _clientAgreementRuleService;
        IClientAgreementTermService _clientAgreementTermService;
        IClientAgreementMVTermService _clientAgreementMVTermService;
        IClientAgreementEndorsementService _clientAgreementEndorsementService;
        IUnderwritingModule _underwritingModule;        

        public UWMService(
            IUnderwritingModule underwritingModule,
            IClientAgreementService clientAgreementService, 
            IClientAgreementRuleService clientAgreementRuleService,
            IClientAgreementTermService clientAgreementTermService,
            IClientAgreementMVTermService clientAgreementMVTermService,
            IClientAgreementEndorsementService clientAgreementEndorsementService)
        {
            _clientAgreementService = clientAgreementService;
            _clientAgreementRuleService = clientAgreementRuleService;
            _clientAgreementTermService = clientAgreementTermService;
            _clientAgreementMVTermService = clientAgreementMVTermService;
            _clientAgreementEndorsementService = clientAgreementEndorsementService;
            _underwritingModule = underwritingModule;            
        }

        
        public bool UWM(User createdBy, ClientInformationSheet sheet, string reference)
        {
            var _modules = new Dictionary<string, IUnderwritingModule>();
            var modules = RegisterModules();
            bool result = false;
			foreach (Product product in sheet.Programme.BaseProgramme.Products) {
				if (!product.UnderwritingEnabled)
					continue;
				
				string uwmCode = product.UnderwritingModuleCode;
				if (string.IsNullOrWhiteSpace (uwmCode))
					throw new Exception ("No underwriting module specificed for product '" + product.Id + "'");
				var uwm = Load(uwmCode, _modules);
				result &= uwm.Underwrite (createdBy, sheet, product, reference);
			}
			return result;
        }

        public void Register(string key, IUnderwritingModule module, Dictionary<string, IUnderwritingModule> _modules)
        {
            _modules[key] = module;
        }

        public IUnderwritingModule Load(string key, Dictionary<string, IUnderwritingModule> _modules)
        {           
            var modules = RegisterModules();
            foreach (var module in modules)
                Register(module.Name, module, _modules);

            if (!_modules.ContainsKey(key))
                throw new Exception("No underwriting module for \"" + key + "\" registered");

            return _modules[key];
        }

        protected IUnderwritingModule[] RegisterModules()
        {
            var modules = new IUnderwritingModule[] {
                new ICIBHIANZUWModule(),
                new ICIBARCCOUWModule(),
                new MarshCoastGuardUWModule(),
                new EmptyUWModule(),
            };
            return modules;
        }

    }
    
}
