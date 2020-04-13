﻿using System;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities;
using DealEngine.Domain.Services;
using DealEngine.Services.Impl.UnderwritingModuleServices;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
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
            string referenceId = reference;
            foreach (Product product in sheet.Programme.BaseProgramme.Products.OrderBy(t => t.OrderNumber)) {
				if (!product.UnderwritingEnabled)
					continue;

                //Mark the existing agreement for this product deleted
                ClientAgreement clientAgreement = sheet.Programme.Agreements.FirstOrDefault(a => a.Product != null && a.Product.Id == product.Id);
                if (clientAgreement != null)
                    clientAgreement.Delete(createdBy);

                //Check if the cover is required
                try
                {
                  if (product.IsOptionalProduct && sheet.Answers.Where(sa => sa.ItemName == product.OptionalProductRequiredAnswer).First().Value != "1")
                        continue;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                

                if (!product.IsMasterProduct)
                {
                    int.TryParse(referenceId, out int newReference);
                    referenceId = (newReference + 1).ToString();
                }

                string uwmCode = product.UnderwritingModuleCode;
				if (string.IsNullOrWhiteSpace (uwmCode))
					throw new Exception ("No underwriting module specificed for product '" + product.Id + "'");
				var uwm = Load(uwmCode, _modules);
				result &= uwm.Underwrite (createdBy, sheet, product, referenceId);

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
                new EmptyUWModule(),
                new ICIBHIANZUWModule(),
                new ICIBARCCOUWModule(),
                new MarshCoastGuardUWModule(),
                new NZACSPIUWModule(),
                new NZACSSLUWModule(),
                new NZACSELUWModule(),
                new NZACSEDUWModule(),
                new NZACSDOUWModule(),
                new NZACSPLUWModule(),
                new NZACSCLUWModule(),
                new CEASPIUWModule(),
                new CEASSLUWModule(),
                new CEASELUWModule(),
                new CEASEDUWModule(),
                new CEASDOUWModule(),
                new CEASPLUWModule(),
                new CEASCLUWModule(),
                new PMINZPIUWModule(),
                new PMINZPLUWModule(),
                new PMINZSLUWModule(),
                new PMINZDOUWModule(),
                new PMINZEDUWModule(),
                new PMINZELUWModule(),
                new PMINZCLUWModule(),

            };
            return modules;
        }

    }
    
}
