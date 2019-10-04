using System;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
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
        IUnitOfWork _unitOfWork;
		Domain.Services.Factories.UWMFactory _uwmFactory;

        public UWMService(IClientAgreementService clientAgreementService, 
            IClientAgreementRuleService clientAgreementRuleService,
            IClientAgreementTermService clientAgreementTermService,
            IClientAgreementMVTermService clientAgreementMVTermService,
            IClientAgreementEndorsementService clientAgreementEndorsementService,
			Domain.Services.Factories.UWMFactory uwmFactory,
            IUnitOfWork unitOfWork)
        {
            _clientAgreementService = clientAgreementService;
            _clientAgreementRuleService = clientAgreementRuleService;
            _clientAgreementTermService = clientAgreementTermService;
            _clientAgreementMVTermService = clientAgreementMVTermService;
            _clientAgreementEndorsementService = clientAgreementEndorsementService;
            _unitOfWork = unitOfWork;
			_uwmFactory = uwmFactory;
        }

        
        public bool UWM_ICIBNZIMV(User createdBy, ClientInformationSheet sheet, string reference)
        {
			bool result = false;
			foreach (Product product in sheet.Programme.BaseProgramme.Products) {
				if (!product.UnderwritingEnabled)
					continue;
				
				string uwmCode = product.UnderwritingModuleCode;
				if (string.IsNullOrWhiteSpace (uwmCode))
					throw new Exception ("No underwriting module specificed for product '" + product.Id + "'");
				var uwm = _uwmFactory.Load (uwmCode);
				result &= uwm.Underwrite (createdBy, sheet, product, reference);
			}
			return result;
        }
    }
}
