using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using SystemDocument = DealEngine.Domain.Entities.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DealEngine.WebUI.Models.Agreement;
using DealEngine.WebUI.Models;
using DealEngine.WebUI.Helpers;
using DealEngine.Infrastructure.Payment.PxpayAPI;
using Microsoft.AspNetCore.Http;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Infrastructure.Payment.EGlobalAPI;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using DealEngine.Infrastructure.Tasking;
using Microsoft.Extensions.Logging;

namespace DealEngine.WebUI.Controllers
{
    [Authorize]
    public class AgreementController : BaseController
    {
        #region Interfaces
        ISubsystemService _subsystemService;
        IActivityService _activityService;
        IInformationTemplateService _informationService;
        IClientInformationService _customerInformationService;
        IPaymentGatewayService _paymentGatewayService;
        IPaymentService _paymentService;
        IMerchantService _merchantService;
        IClientAgreementTermService _clientAgreementTermService;
        IMilestoneService _milestoneService;
        IAdvisoryService _advisoryService;
        ITaskingService _taskingService;
        IHttpClientService _httpClientService;
        IProductService _productService;                       
        IAppSettingService _appSettingService;
        IClientAgreementService _clientAgreementService;
        IClientAgreementRuleService _clientAgreementRuleService;
        IClientAgreementEndorsementService _clientAgreementEndorsementService;
        IFileService _fileService;
        IEmailService _emailService;              
        IOrganisationService _organisationService;
        IProgrammeService _programmeService;
        IUnitOfWork _unitOfWork;
        IInsuranceAttributeService _insuranceAttributeService;
        IEGlobalSubmissionService _eGlobalSubmissionService;
        IApplicationLoggingService _applicationLoggingService;
        ILogger<AgreementController> _logger;
        IClientAgreementTermCanService _clientAgreementTermCanService;
        IClientAgreementBVTermCanService _clientAgreementBVTermCanService;
        //convert to service?
        IMapperSession<Rule> _ruleRepository;
        IMapperSession<SystemDocument> _documentRepository;
        #endregion

        public AgreementController(
            ISubsystemService subsystemService,
            ILogger<AgreementController> logger,
            IApplicationLoggingService applicationLoggingService,
            IUserService userRepository, 
            IUnitOfWork unitOfWork, 
            IMilestoneService milestoneService, 
            IInformationTemplateService informationService, 
            IClientInformationService customerInformationService,
            IProductService productService,
            IClientAgreementService clientAgreementService, 
            IClientAgreementRuleService clientAgreementRuleService, 
            IAdvisoryService advisoryService,
            IClientAgreementEndorsementService clientAgreementEndorsementService, 
            IFileService fileService, 
            IHttpClientService httpClientService, 
            ITaskingService taskingService, 
            IActivityService activityService,
            IOrganisationService organisationService, 
            IMapperSession<Rule> ruleRepository, 
            IEmailService emailService, 
            IMapperSession<SystemDocument> documentRepository,
            IProgrammeService programmeService,
            IPaymentGatewayService paymentGatewayService, 
            IInsuranceAttributeService insuranceAttributeService, 
            IPaymentService paymentService, 
            IMerchantService merchantService, 
            IClientAgreementTermService clientAgreementTermService, 
            IAppSettingService appSettingService, 
            IEGlobalSubmissionService eGlobalSubmissionService,
            IClientAgreementTermCanService clientAgreementTermCanService,
            IClientAgreementBVTermCanService clientAgreementBVTermCanService
            )
            : base (userRepository)
        {
            _subsystemService = subsystemService;
            _logger = logger;
            _applicationLoggingService = applicationLoggingService;
            _activityService = activityService;
            _advisoryService = advisoryService;
            _taskingService = taskingService;
            _informationService = informationService;
            _customerInformationService = customerInformationService;
            _milestoneService = milestoneService;
            _organisationService = organisationService;
            _httpClientService = httpClientService;
            _productService = productService;
            _clientAgreementService = clientAgreementService;
            _clientAgreementRuleService = clientAgreementRuleService;
            _clientAgreementEndorsementService = clientAgreementEndorsementService;
            _fileService = fileService;
            _emailService = emailService;            
            _unitOfWork = unitOfWork;
            _ruleRepository = ruleRepository;
            _documentRepository = documentRepository;            
            _paymentGatewayService = paymentGatewayService;
            _paymentService = paymentService;
            _merchantService = merchantService;
            _clientAgreementTermService = clientAgreementTermService;
            _insuranceAttributeService = insuranceAttributeService;
            _programmeService = programmeService;
            _appSettingService = appSettingService;
            _eGlobalSubmissionService = eGlobalSubmissionService;
            _clientAgreementTermCanService = clientAgreementTermCanService;
            _clientAgreementBVTermCanService = clientAgreementBVTermCanService;

            ViewBag.Title = "Wellness and Health Associated Professionals Agreement";
        }

        [HttpGet]
        public async Task<IActionResult> MyAgreements()
        {
            MyAgreementsViewModel model = new MyAgreementsViewModel();
            model.MyAgreements = new List<AgreementViewModel>();

            // TODO - fix this to use ClientProgramme
            //foreach (var answerSheet in _customerInformationService.GetAllInformationFor (CurrentUser())) {
            //	if (answerSheet.Status == "Submitted") {
            //		var template = answerSheet.InformationTemplate;
            //		model.MyAgreements.Add (new AgreementViewModel () {
            //			InformationName = template.Name,
            //			InformationId = answerSheet.Id,
            //			Status = "Unaccepted"
            //		});
            //	}
            //}

            return View(model);
        }

        public async Task<IActionResult> AgreementTemplates()
        {
            return View();
        }

        public async Task<IActionResult> AgreementBuilder()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                var templates = await _informationService.GetAllTemplates();
                var products = await _productService.GetAllProducts();

                AgreementTemplateViewModel model = new AgreementTemplateViewModel();

                List<string> allLanguages = new List<string>();
                foreach (var product in products)
                    allLanguages.AddRange(product.Languages);

                model.Languages = new List<SelectListItem>();
                foreach (var language in allLanguages.Distinct())
                    model.Languages.Add(
                        new SelectListItem
                        {
                            Text = language,
                            Value = language
                        }
                    );

                model.InformationSheets = new List<SelectListItem>();
                foreach (var template in templates)
                    model.InformationSheets.Add(
                        new SelectListItem
                        {
                            Text = template.Name,
                            Value = template.Id.ToString()
                        }
                    );

                model.Products = new List<SelectListItem>();
                foreach (var product in products)
                    model.Products.Add(
                        new SelectListItem
                        {
                            Text = product.Name,
                            Value = product.Id.ToString()
                        }
                    );

                return View(model);
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> AuthoriseReferrals(Guid sheetId,Guid informationsheet, Guid agreementId)
        {
            ViewAgreementViewModel model = new ViewAgreementViewModel();
            model.Referrals = new List<ClientAgreementReferral>();
            User user = null;
            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(agreementId);
                model.InformationSheetId = sheetId;
                model.ClientAgreementId = agreementId;
                model.ClientProgrammeId = agreement.ClientInformationSheet.Programme.Id;

                foreach (var terms in agreement.ClientAgreementReferrals)
                {
                    model.Referrals.Add(terms);
                }

                model.ReferralLoading = agreement.ClientAgreementTerms.FirstOrDefault().ReferralLoading;
                model.ReferralAmount = agreement.ClientAgreementTerms.FirstOrDefault().ReferralLoadingAmount;
                model.AuthorisationNotes = agreement.ClientAgreementTerms.FirstOrDefault().AuthorisationNotes;

                ViewBag.Title = "Agreement Referals ";

                return View("AuthoriseReferrals", model);
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> AuthorisedReferral(AgreementViewModel clientAgreementModel)
        {
            User user = null;
            try
            {
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(clientAgreementModel.AgreementId);
                user = await CurrentUser();
                var premium = 0.0m;
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    foreach (var terms in agreement.ClientAgreementReferrals.Where(r => r.Status == "Pending"))
                    {
                        terms.Status = "Cleared";
                    }

                    foreach (var terms in agreement.ClientAgreementTerms)
                    {
                        if (terms.BoatTerms.Count() > 0)
                        {
                            foreach (var bvterm in terms.BoatTerms)
                            {
                                premium = premium + bvterm.Premium;
                            }
                        }
                        if (terms.MotorTerms.Count() > 0)
                        {
                            foreach (var mvterm in terms.MotorTerms)
                            {
                                premium = premium + mvterm.Premium;
                            }
                        }                        
                    }

                    foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms)
                    {
                        term.ReferralLoading = clientAgreementModel.RefferLodPrc;
                        term.ReferralLoadingAmount = clientAgreementModel.RefferLodAmt;
                        term.AuthorisationNotes = clientAgreementModel.AdditionalNotes;
                        if (term.MotorTerms.Count() == 0 && term.MotorTerms.Count() == 0)
                        {
                            term.Premium = term.Premium * (1 + clientAgreementModel.RefferLodPrc / 100) + clientAgreementModel.RefferLodAmt;
                        } else
                        {
                            term.Premium = premium * (1 + clientAgreementModel.RefferLodPrc / 100) + clientAgreementModel.RefferLodAmt;
                        }
                        
                    }

                    foreach (var terms in agreement.ClientAgreementTerms)
                    {
                        foreach (var bvterm in terms.BoatTerms)
                        {

                            if (bvterm.Boat.BoatWaterLocation != null)
                            {

                                InsuranceAttribute insuranceAttribute = await _insuranceAttributeService.GetInsuranceAttributeByName("Other Marina");
                                if (insuranceAttribute != null)
                                {
                                    var orgList = await _organisationService.GetAllOrganisations();
                                    orgList.Where(o => o.IsApproved == false && o.InsuranceAttributes.Contains(insuranceAttribute)).ToList();
                                    foreach (var org in orgList)
                                    {
                                        InsuranceAttribute insuranceAttribute1 = await _insuranceAttributeService.GetInsuranceAttributeByName(org.Name);
                                        if (insuranceAttribute.InsuranceAttributeName == "Other Marina")
                                        {

                                            org.IsApproved = true;
                                        }
                                    }
                                    //Organisation othermarine = await _OrganisationRepository.GetByIdAsync(bvterm.Boat.BoatWaterLocation.Id);
                                }

                            }

                        }
                    }

                    if (agreement.Status != "Quoted")
                        agreement.Status = "Quoted";

                    string auditLogDetail = "Agreement Referrals have been authorised by " + user.FullName;
                    AuditLog auditLog = new AuditLog(user, agreement.ClientInformationSheet, agreement, auditLogDetail);
                    agreement.ClientAgreementAuditLogs.Add(auditLog);

                    await uow.Commit();

                }

                var url = "/Agreement/ViewAcceptedAgreement/" + agreement.ClientInformationSheet.Programme.Id;
                return Json(new { url });
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> CancellAgreement(Guid id)
        {
            ViewAgreementViewModel model = new ViewAgreementViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(id);
                ClientInformationSheet answerSheet = agreement.ClientInformationSheet;
                Organisation insured = answerSheet.Owner;
                ClientProgramme programme = answerSheet.Programme;
                var insuranceRoles = new List<InsuranceRoleViewModel>();
                insuranceRoles.Add(new InsuranceRoleViewModel { RoleName = "Client", Name = insured.Name, ManagedBy = "", Email = "" });

                model.InformationSheetId = answerSheet.Id;
                model.ClientAgreementId = agreement.Id;
                model.ClientProgrammeId = programme.Id;
                model.InsuranceRoles = insuranceRoles;
                model.CancellNotes = agreement.CancelledNote;
                model.StartDate = LocalizeTimeDate(agreement.InceptionDate, "dd-mm-yyyy");
                model.EndDate = LocalizeTimeDate(agreement.ExpiryDate, "dd-mm-yyyy");
                model.CancellEffectiveDate = agreement.CancelledEffectiveDate;
                model.CancelAgreementReason = agreement.CancelAgreementReason;


                foreach (var terms in agreement.ClientAgreementTerms)
                {
                    if (terms.BoatTerms.Where(bvt => bvt.DateDeleted == null).Count() > 0)
                    {
                        var boats = new List<EditTermsViewModel>();
                        foreach (var boat in terms.BoatTerms)
                        {
                            boats.Add(new EditTermsViewModel
                            {
                                VesselId = boat.Id,
                                BoatName = boat.BoatName,
                                BoatMake = boat.BoatMake,
                                BoatModel = boat.BoatModel,
                                TermLimit = boat.TermLimit,
                                Excess = Convert.ToInt32(boat.Excess),
                                Premium = boat.Premium,
                                FSL = boat.FSL
                            });
                        }
                        model.BVTerms = boats;
                    }

                    if (terms.MotorTerms.Where(mvt => mvt.DateDeleted == null).Count() > 0)
                    {
                        var motors = new List<EditTermsViewModel>();
                        foreach (var motor in terms.MotorTerms)
                        {
                            motors.Add(new EditTermsViewModel
                            {
                                VesselId = motor.Id,
                                Registration = motor.Registration,
                                Make = motor.Make,
                                Model = motor.Model,
                                TermLimit = motor.TermLimit,
                                Excess = Convert.ToInt32(motor.Excess),
                                Premium = motor.Premium,
                                FSL = motor.FSL
                            });
                        }
                        model.MVTerms = motors;
                    }
                }

                if (agreement.ClientAgreementTermsCancel != null)
                {
                    foreach (var termsCan in agreement.ClientAgreementTermsCancel)
                    {
                        if (termsCan.BoatTermsCan.Where(bvtCan => bvtCan.DateDeleted == null).Count() > 0)
                        {
                            var boatsCan = new List<EditTermsCancelViewModel>();
                            foreach (var boatCan in termsCan.BoatTermsCan)
                            {
                                boatsCan.Add(new EditTermsCancelViewModel
                                {
                                    VesselCanId = boatCan.Id,
                                    BoatNameCan = boatCan.BoatNameCan,
                                    BoatMakeCan = boatCan.BoatMakeCan,
                                    BoatModelCan = boatCan.BoatModelCan,
                                    TermLimitCan = boatCan.TermLimitCan,
                                    ExcessCan = Convert.ToInt32(boatCan.ExcessCan),
                                    PremiumCan = boatCan.PremiumCan,
                                    FSLCan = boatCan.FSLCan
                                });
                            }
                            model.BVTermsCan = boatsCan;
                        }

                        if (termsCan.MotorTermsCan.Where(mvtCan => mvtCan.DateDeleted == null).Count() > 0)
                        {
                            var motorsCan = new List<EditTermsCancelViewModel>();
                            foreach (var motorCan in termsCan.MotorTermsCan)
                            {
                                motorsCan.Add(new EditTermsCancelViewModel
                                {
                                    VesselCanId = motorCan.Id,
                                    RegistrationCan = motorCan.RegistrationCan,
                                    MakeCan = motorCan.MakeCan,
                                    ModelCan = motorCan.ModelCan,
                                    TermLimitCan = motorCan.TermLimitCan,
                                    ExcessCan = Convert.ToInt32(motorCan.ExcessCan),
                                    PremiumCan = motorCan.PremiumCan,
                                    FSLCan = motorCan.FSLCan
                                });
                            }
                            model.MVTermsCan = motorsCan;
                        }
                    }
                }


                ViewBag.Title = "Cancel Agreement ";

                return View("CancellAgreement", model);
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> CancellAgreement(AgreementViewModel clientAgreementModel)
        {
            User user = null;
            var url = "";
            try
            {
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(clientAgreementModel.AgreementId);
                user = await CurrentUser();

                if (agreement.ClientInformationSheet.Programme.BaseProgramme.CalculateCancelTerm)
                {
                    ClientAgreementTerm excaterm = agreement.ClientAgreementTerms.FirstOrDefault(cat => cat.SubTermType == "BV" && cat.DateDeleted == null);
                    if (excaterm != null)
                    {
                        if (agreement.ClientAgreementTermsCancel.FirstOrDefault(acatcan => acatcan.DateDeleted == null && acatcan.exClientAgreementTerm == excaterm) == null)
                        {
                            await _clientAgreementTermCanService.AddAgreementTermCan(user, excaterm.TermLimit, excaterm.Excess, 0m, 0m, excaterm.BrokerageRate, 0m, agreement, "BV");
                        }
                        ClientAgreementTermCancel catermcan = agreement.ClientAgreementTermsCancel.FirstOrDefault(catCancel => catCancel.SubTermTypeCan == "BV" && catCancel.DateDeleted == null);
                        catermcan.exClientAgreementTerm = excaterm;
                        catermcan.LastModifiedBy = user;
                        catermcan.LastModifiedOn = DateTime.UtcNow;

                        var excaBVTerms = excaterm.BoatTerms;
                        var excaMVTerms = excaterm.MotorTerms;

                        int expolicyperiodindaysCan = 0;
                        decimal totalBoatFslCan = 0m;
                        decimal totalBoatPremiumCan = 0m;
                        decimal totalBoatBrokerageCan = 0m;
                        decimal totalVehicleFslCan = 0m;
                        decimal totalVehiclePremiumCan = 0m;
                        decimal totalVehicleBrokerageCan = 0m;
                        expolicyperiodindaysCan = (agreement.ExpiryDate - agreement.InceptionDate).Days;

                        if (excaBVTerms != null && catermcan != null)
                        {
                            foreach (ClientAgreementBVTerm excaBVTerm in excaBVTerms)
                            {
                                int boatperiodindaysCan = 0;
                                decimal boatproratedFslCan = 0m;
                                decimal boatproratedPremiumCan = 0m;
                                decimal boatproratedBrokerageCan = 0m;

                                //Calculate BV cancel term
                                if (clientAgreementModel.CancellEffectiveDate != null && excaBVTerm.Boat.BoatInceptionDate != null && 
                                    clientAgreementModel.CancellEffectiveDate >= excaBVTerm.Boat.BoatInceptionDate)
                                {
                                    boatperiodindaysCan = (excaBVTerm.Boat.BoatExpireDate - clientAgreementModel.CancellEffectiveDate).Days;
                                    boatproratedPremiumCan = excaBVTerm.AnnualPremium * boatperiodindaysCan / expolicyperiodindaysCan - excaBVTerm.Premium;
                                    boatproratedFslCan = excaBVTerm.AnnualFSL * boatperiodindaysCan / expolicyperiodindaysCan - excaBVTerm.FSL;
                                    boatproratedBrokerageCan = boatproratedPremiumCan * excaterm.BrokerageRate / 100;
                                }

                                totalBoatPremiumCan += boatproratedPremiumCan;
                                totalBoatFslCan += boatproratedFslCan;
                                totalBoatBrokerageCan += boatproratedBrokerageCan;

                                ClientAgreementBVTermCancel excabvtermcan = catermcan.BoatTermsCan.FirstOrDefault(acabvtcan => acabvtcan.DateDeleted == null && acabvtcan.exClientAgreementBVTerm == excaBVTerm);
                                if (excabvtermcan == null)
                                {
                                    using (var uow = _unitOfWork.BeginUnitOfWork())
                                    {
                                        ClientAgreementBVTermCancel cabvtermcan = new ClientAgreementBVTermCancel(user, excaBVTerm.BoatName, excaBVTerm.YearOfManufacture, excaBVTerm.BoatMake, excaBVTerm.BoatModel,
                                            excaBVTerm.TermLimit, excaBVTerm.Excess, boatproratedPremiumCan, boatproratedFslCan, excaBVTerm.BrokerageRate, boatproratedBrokerageCan, catermcan, excaBVTerm.Boat);
                                        cabvtermcan.TermCategoryCan = "active";
                                        cabvtermcan.AnnualPremiumCan = excaBVTerm.AnnualPremium;
                                        cabvtermcan.AnnualFSLCan = excaBVTerm.AnnualFSL;
                                        cabvtermcan.AnnualBrokerageCan = excaBVTerm.AnnualBrokerage;
                                        catermcan.BoatTermsCan.Add(cabvtermcan);
                                        cabvtermcan.exClientAgreementBVTerm = excaBVTerm;

                                        await uow.Commit().ConfigureAwait(false);
                                    }
                                } else
                                {
                                    excabvtermcan.AnnualPremiumCan = excaBVTerm.AnnualPremium;
                                    excabvtermcan.AnnualFSLCan = excaBVTerm.AnnualFSL;
                                    excabvtermcan.AnnualBrokerageCan = excaBVTerm.AnnualBrokerage;
                                    excabvtermcan.exClientAgreementBVTerm = excaBVTerm;
                                    excabvtermcan.PremiumCan = boatproratedPremiumCan;
                                    excabvtermcan.FSLCan = boatproratedFslCan;
                                    excabvtermcan.BrokerageCan = boatproratedBrokerageCan;
                                    excabvtermcan.LastModifiedBy = user;
                                    excabvtermcan.LastModifiedOn = DateTime.UtcNow;
                                }

                            }

                            catermcan.PremiumCan += totalBoatPremiumCan;
                            catermcan.FSLCan += totalBoatFslCan;
                            catermcan.BrokerageCan += totalBoatBrokerageCan;
                        }

                        if (excaMVTerms != null && catermcan != null)
                        {
                            foreach (ClientAgreementMVTerm excaMVTerm in excaMVTerms)
                            {
                                int vehicleperiodindaysCan = 0;
                                decimal vehicleproratedFslCan = 0m;
                                decimal vehicleproratedPremiumCan = 0m;
                                decimal vehicleproratedBrokerageCan = 0m;

                                //Calculate MV cancel term
                                if (clientAgreementModel.CancellEffectiveDate != null && excaMVTerm.Vehicle.VehicleInceptionDate != null &&
                                    clientAgreementModel.CancellEffectiveDate >= excaMVTerm.Vehicle.VehicleInceptionDate)
                                {
                                    vehicleperiodindaysCan = (excaMVTerm.Vehicle.VehicleExpireDate - clientAgreementModel.CancellEffectiveDate).Days;
                                    vehicleproratedPremiumCan = excaMVTerm.AnnualPremium * vehicleperiodindaysCan / expolicyperiodindaysCan - excaMVTerm.Premium;
                                    vehicleproratedFslCan = excaMVTerm.AnnualFSL * vehicleperiodindaysCan / expolicyperiodindaysCan - excaMVTerm.FSL;
                                    vehicleproratedBrokerageCan = vehicleproratedPremiumCan * excaterm.BrokerageRate / 100;
                                }

                                totalVehiclePremiumCan += vehicleproratedPremiumCan;
                                totalVehicleFslCan += vehicleproratedFslCan;
                                totalVehicleBrokerageCan += vehicleproratedBrokerageCan;

                                ClientAgreementMVTermCancel excamvtermcan = catermcan.MotorTermsCan.FirstOrDefault(acamvtcan => acamvtcan.DateDeleted == null && acamvtcan.exClientAgreementMVTerm == excaMVTerm);
                                if (excamvtermcan == null)
                                {
                                    using (var uow1 = _unitOfWork.BeginUnitOfWork())
                                    {
                                        ClientAgreementMVTermCancel camvtermcan = new ClientAgreementMVTermCancel(user, excaMVTerm.Registration, excaMVTerm.Year, excaMVTerm.Make, excaMVTerm.Model,
                                            excaMVTerm.TermLimit, excaMVTerm.Excess, vehicleproratedPremiumCan, vehicleproratedFslCan, excaMVTerm.BrokerageRate, vehicleproratedBrokerageCan,
                                            excaMVTerm.VehicleCategory, excaMVTerm.FleetNumber, catermcan, excaMVTerm.Vehicle, excaMVTerm.BurnerPremium);
                                        camvtermcan.TermCategoryCan = "active";
                                        camvtermcan.AnnualPremiumCan = excaMVTerm.AnnualPremium;
                                        camvtermcan.AnnualFSLCan = excaMVTerm.AnnualFSL;
                                        camvtermcan.AnnualBrokerageCan = excaMVTerm.AnnualBrokerage;
                                        catermcan.MotorTermsCan.Add(camvtermcan);
                                        camvtermcan.exClientAgreementMVTerm = excaMVTerm;

                                        await uow1.Commit().ConfigureAwait(false);
                                    }
                                }
                                else
                                {
                                    excamvtermcan.AnnualPremiumCan = excaMVTerm.AnnualPremium;
                                    excamvtermcan.AnnualFSLCan = excaMVTerm.AnnualFSL;
                                    excamvtermcan.AnnualBrokerageCan = excaMVTerm.AnnualBrokerage;
                                    excamvtermcan.exClientAgreementMVTerm = excaMVTerm;
                                    excamvtermcan.PremiumCan = vehicleproratedPremiumCan;
                                    excamvtermcan.FSLCan = vehicleproratedFslCan;
                                    excamvtermcan.BrokerageCan = vehicleproratedBrokerageCan;
                                    excamvtermcan.LastModifiedBy = user;
                                    excamvtermcan.LastModifiedOn = DateTime.UtcNow;
                                }

                            }

                            catermcan.PremiumCan += totalVehiclePremiumCan;
                            catermcan.FSLCan += totalVehicleFslCan;
                            catermcan.BrokerageCan += totalVehicleBrokerageCan;
                        }

                        if ((agreement.Status != "Declined by Insurer" || agreement.Status != "Declined by Insured" || agreement.Status != "Cancelled" || agreement.Status != "Cancel Pending") &&
                                (agreement.Status == "Bound" || agreement.Status == "Bound and invoice pending" || agreement.Status == "Bound and invoiced"))
                        {
                            using (var uow2 = _unitOfWork.BeginUnitOfWork())
                            {
                                agreement.Status = "Cancel Pending";
                                agreement.CancelledNote = clientAgreementModel.CancellNotes;
                                agreement.CancelledEffectiveDate = clientAgreementModel.CancellEffectiveDate;
                                agreement.CancelAgreementReason = clientAgreementModel.CancelAgreementReason;
                                agreement.CancelledByUserID = user;
                                agreement.CancelledDate = DateTime.UtcNow;


                                string auditLogDetail = "Agreement has been requested to cancel by " + user.FullName;
                                AuditLog auditLog = new AuditLog(user, agreement.ClientInformationSheet, agreement, auditLogDetail);
                                agreement.ClientAgreementAuditLogs.Add(auditLog);

                                await uow2.Commit().ConfigureAwait(false);
                            }
                        }
                        
                    }

                    url = "/Agreement/CancellAgreement/" + agreement.Id;


                } else
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if ((agreement.Status != "Declined by Insurer" || agreement.Status != "Declined by Insured" || agreement.Status != "Cancelled" || agreement.Status != "Cancel Pending") &&
                            (agreement.Status == "Bound" || agreement.Status == "Bound and invoice pending" || agreement.Status == "Bound and invoiced"))
                        {
                            agreement.Status = "Cancelled";
                            agreement.CancelledNote = clientAgreementModel.CancellNotes;
                            agreement.CancelledEffectiveDate = clientAgreementModel.CancellEffectiveDate;
                            agreement.CancelAgreementReason = clientAgreementModel.CancelAgreementReason;
                            agreement.Cancelled = true;
                            agreement.CancelledByUserID = user;
                            agreement.CancelledDate = DateTime.UtcNow;
                        }

                        string auditLogDetail = "Agreement has been cancelled by " + user.FullName;
                        AuditLog auditLog = new AuditLog(user, agreement.ClientInformationSheet, agreement, auditLogDetail);
                        agreement.ClientAgreementAuditLogs.Add(auditLog);

                        await uow.Commit().ConfigureAwait(false);

                    }

                    url = "/Agreement/CancellAgreement/" + agreement.Id;
                }

                return Json(new { url });
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCancellAgreement(AgreementViewModel clientAgreementModel)
        {
            User user = null;
            var url = "";
            try
            {
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(clientAgreementModel.AgreementId);
                user = await CurrentUser();

                ClientProgramme programme = agreement.ClientInformationSheet.Programme;
                var eGlobalSerializer = new EGlobalSerializerAPI();

                //check Eglobal parameters
                if (string.IsNullOrEmpty(programme.EGlobalClientNumber))
                {
                    throw new Exception(nameof(programme.EGlobalClientNumber) + " EGlobal client number");
                }

                string paymentType = "";
                Guid transactionreferenceid = Guid.NewGuid();

                var xmlPayload = eGlobalSerializer.SerializePolicy(programme, user, _unitOfWork, transactionreferenceid, paymentType, false, true, null);

                var byteResponse = await _httpClientService.CreateEGlobalInvoice(xmlPayload);

                EGlobalSubmission eglobalsubmission = await _eGlobalSubmissionService.GetEGlobalSubmissionByTransaction(transactionreferenceid);

                eGlobalSerializer.DeSerializeResponse(byteResponse, programme, user, _unitOfWork, eglobalsubmission);


                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    if ((agreement.Status != "Declined by Insurer" || agreement.Status != "Declined by Insured" || agreement.Status != "Cancelled") &&
                        (agreement.Status == "Bound" || agreement.Status == "Bound and invoice pending" || agreement.Status == "Bound and invoiced" || agreement.Status == "Cancel Pending"))
                    {
                        agreement.Status = "Cancelled";
                        agreement.Cancelled = true;
                        agreement.CancelledByUserID = user;
                        agreement.CancelledDate = DateTime.UtcNow;
                    }


                    string auditLogDetail = "Agreement has been confirmed cancel by " + user.FullName;
                    AuditLog auditLog = new AuditLog(user, agreement.ClientInformationSheet, agreement, auditLogDetail);
                    agreement.ClientAgreementAuditLogs.Add(auditLog);

                    await uow.Commit().ConfigureAwait(false);

                }

                url = "/Agreement/ViewAcceptedAgreement/" + agreement.ClientInformationSheet.Programme.Id;

                return Json(new { url });
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeclineAgreement(Guid id)
        {
            ViewAgreementViewModel model = new ViewAgreementViewModel();
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(id);
                ClientInformationSheet answerSheet = agreement.ClientInformationSheet;
                Organisation insured = answerSheet.Owner;
                ClientProgramme programme = answerSheet.Programme;

                model.InformationSheetId = answerSheet.Id;
                model.ClientAgreementId = agreement.Id;
                model.ClientProgrammeId = programme.Id;

                model.DeclineNotes = agreement.InsurerDeclinedComment;

                ViewBag.Title = "Decline Agreement ";

                return View("DeclineAgreement", model);
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> DeclineAgreement(AgreementViewModel clientAgreementModel)
        {
            User user = null; 
            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(clientAgreementModel.AgreementId);
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    if (agreement.Status != "Declined by Insurer" || agreement.Status != "Declined by Insured" || agreement.Status != "Cancelled" ||
                        agreement.Status != "Bound" || agreement.Status != "Bound and invoice pending" || agreement.Status != "Bound and invoiced")

                        agreement.Status = "Declined by Insurer";
                    agreement.InsurerDeclinedComment = clientAgreementModel.DeclineNotes;
                    agreement.InsurerDeclined = true;
                    agreement.InsurerDeclinedUserID = user;
                    agreement.InsurerDeclinedDate = DateTime.UtcNow;


                    string auditLogDetail = "Agreement has been declined by " + user.FullName;
                    AuditLog auditLog = new AuditLog(user, agreement.ClientInformationSheet, agreement, auditLogDetail);
                    agreement.ClientAgreementAuditLogs.Add(auditLog);

                    await uow.Commit();

                }

                var url = "/Agreement/ViewAcceptedAgreement/" + agreement.ClientInformationSheet.Programme.Id;
                return Json(new { url });
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> UndeclineAgreement(Guid id)
        {
            User user = null;
            try
            {
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(id);
                user = await CurrentUser();
                if (agreement != null)
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (agreement.Status == "Declined by Insurer" || agreement.Status == "Declined by Insured")
                        {
                            agreement.Status = "Quoted";
                            agreement.InsurerDeclined = false;
                            agreement.InsuredDeclined = false;
                            agreement.UndeclinedUserID = user;
                            agreement.UndeclinedDate = DateTime.UtcNow;
                        }

                        string auditLogDetail = "Agreement has been undeclined by " + user.FullName;
                        AuditLog auditLog = new AuditLog(user, agreement.ClientInformationSheet, agreement, auditLogDetail);
                        agreement.ClientAgreementAuditLogs.Add(auditLog);

                        await uow.Commit();

                    }
                }

                return Redirect("/Agreement/ViewAcceptedAgreement/" + agreement.ClientInformationSheet.Programme.Id);
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendReferredEmail(EmailTemplateViewModel model)
        {
            User user = null;
            try
            {
                ClientProgramme programme = await _programmeService.GetClientProgrammebyId(model.ClientProgrammeID);
                user = await CurrentUser();
                // TODO - rewrite to save templates on a per programme basis
                ClientAgreement agreement = programme.Agreements[0];
                //EmailTemplate emailTemplate = agreement.Product.EmailTemplates.FirstOrDefault (et => et.Type == "SendPolicyDocuments");
                EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == model.Type);

                if (emailTemplate != null)
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        emailTemplate.Subject = model.Subject;
                        emailTemplate.Body = model.Body;
                        emailTemplate.LastModifiedBy = user;
                        emailTemplate.LastModifiedOn = DateTime.UtcNow;

                        await uow.Commit();
                    }
                }
                else
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        emailTemplate = new EmailTemplate(user, "Agreement Documents Covering Text", "SendPolicyDocuments", model.Subject, model.Body, null, programme.BaseProgramme);
                        programme.BaseProgramme.EmailTemplates.Add(emailTemplate);

                        await uow.Commit();
                    }
                }

                var docs = agreement.GetDocuments();
                var documents = new List<SystemDocument>();

                if (docs != null)
                {
                    foreach (SystemDocument doc in docs)
                    {
                        if (doc.DateDeleted == null)
                        {
                            documents.Add(doc);
                        }
                    }
                }
                else
                {
                    documents = null;
                }

                string strrecipentemail = null;
                if (model.Recipent != null)
                {
                    var userdb = await _userService.GetUserById(model.Recipent);
                    strrecipentemail = userdb.Email;
                }

                //await _emailService.SendEmailViaEmailTemplate(strrecipentemail, emailTemplate, documents, null, null);

                return Redirect("~/Home/Index");
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }           
        }

        [HttpGet]
        public async Task<IActionResult> EditTerms(Guid id)
        {
            User user = null;
            ViewAgreementViewModel model = new ViewAgreementViewModel();
            try
            {
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(id);
                ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV" && t.DateDeleted == null);
                model.ClientAgreementId = id;
                model.ClientProgrammeId = agreement.ClientInformationSheet.Programme.Id;
                foreach (var terms in agreement.ClientAgreementTerms)
                {
                    if (terms.BoatTerms.Where(bvt => bvt.DateDeleted == null).Count() > 0)
                    {
                        var boats = new List<EditTermsViewModel>();
                        foreach (var boat in terms.BoatTerms)
                        {
                            boats.Add(new EditTermsViewModel
                            {
                                VesselId = boat.Id,
                                BoatName = boat.BoatName,
                                BoatMake = boat.BoatMake,
                                BoatModel = boat.BoatModel,
                                TermLimit = boat.TermLimit,
                                Excess = Convert.ToInt32(boat.Excess),
                                Premium = boat.Premium,
                                FSL = boat.FSL
                            });
                        }
                        model.BVTerms = boats;
                    }

                    if (terms.MotorTerms.Where(mvt => mvt.DateDeleted == null).Count() > 0)
                    {
                        var motors = new List<EditTermsViewModel>();
                        foreach (var motor in terms.MotorTerms)
                        {
                            motors.Add(new EditTermsViewModel
                            {
                                VesselId = motor.Id,
                                Registration = motor.Registration,
                                Make = motor.Make,
                                Model = motor.Model,
                                TermLimit = motor.TermLimit,
                                Excess = Convert.ToInt32(motor.Excess),
                                Premium = motor.Premium,
                                FSL = motor.FSL
                            });
                        }
                        model.MVTerms = motors;
                    }

                    if (terms.MotorTerms.Where(mvt => mvt.DateDeleted == null).Count() > 0)
                    {
                        var motors = new List<EditTermsViewModel>();
                        foreach (var motor in terms.MotorTerms)
                        {
                            motors.Add(new EditTermsViewModel
                            {
                                VesselId = motor.Id,
                                Registration = motor.Registration,
                                Make = motor.Make,
                                Model = motor.Model,
                                TermLimit = motor.TermLimit,
                                Excess = Convert.ToInt32(motor.Excess),
                                Premium = motor.Premium,
                                FSL = motor.FSL
                            });
                        }
                        model.MVTerms = motors;
                    }
                }
                var plterms = new List<EditTermsViewModel>();
                var edterms = new List<EditTermsViewModel>();
                var piterms = new List<EditTermsViewModel>();
                var elterms = new List<EditTermsViewModel>();
                var clterms = new List<EditTermsViewModel>();
                var slterms = new List<EditTermsViewModel>();
                var doterms = new List<EditTermsViewModel>();                

                foreach (var plterm in agreement.ClientAgreementTerms.Where(t => t.SubTermType == "PL" && t.DateDeleted == null))
                {
                    plterms.Add(new EditTermsViewModel
                    {
                        TermId = plterm.Id,
                        TermType = plterm.SubTermType,
                        TermLimit = plterm.TermLimit,
                        Excess = Convert.ToInt32(plterm.Excess),
                        Premium = plterm.Premium
                    });
                }
                foreach (var plterm in agreement.ClientAgreementTerms.Where(t => t.SubTermType == "ED" && t.DateDeleted == null))
                {
                    edterms.Add(new EditTermsViewModel
                    {
                        TermId = plterm.Id,
                        TermType = plterm.SubTermType,
                        TermLimit = plterm.TermLimit,
                        Excess = Convert.ToInt32(plterm.Excess),
                        Premium = plterm.Premium
                    });


                }
                foreach (var plterm in agreement.ClientAgreementTerms.Where(t => t.SubTermType == "PI" && t.DateDeleted == null))
                {
                    piterms.Add(new EditTermsViewModel
                    {
                        TermId = plterm.Id,
                        TermType = plterm.SubTermType,
                        TermLimit = plterm.TermLimit,
                        Excess = Convert.ToInt32(plterm.Excess),
                        Premium = plterm.Premium
                    });


                }
                foreach (var plterm in agreement.ClientAgreementTerms.Where(t => t.SubTermType == "EL" && t.DateDeleted == null))
                {
                    elterms.Add(new EditTermsViewModel
                    {
                        TermId = plterm.Id,
                        TermType = plterm.SubTermType,
                        TermLimit = plterm.TermLimit,
                        Excess = Convert.ToInt32(plterm.Excess),
                        Premium = plterm.Premium
                    });


                }
                foreach (var plterm in agreement.ClientAgreementTerms.Where(t => t.SubTermType == "CL" && t.DateDeleted == null))
                {
                    clterms.Add(new EditTermsViewModel
                    {
                        TermId = plterm.Id,
                        TermType = plterm.SubTermType,
                        TermLimit = plterm.TermLimit,
                        Excess = Convert.ToInt32(plterm.Excess),
                        Premium = plterm.Premium
                    });


                }
                foreach (var plterm in agreement.ClientAgreementTerms.Where(t => t.SubTermType == "SL" && t.DateDeleted == null))
                {
                    slterms.Add(new EditTermsViewModel
                    {
                        TermId = plterm.Id,
                        TermType = plterm.SubTermType,
                        TermLimit = plterm.TermLimit,
                        Excess = Convert.ToInt32(plterm.Excess),
                        Premium = plterm.Premium
                    });


                }
                foreach (var plterm in agreement.ClientAgreementTerms.Where(t => t.SubTermType == "DO" && t.DateDeleted == null))
                {
                    doterms.Add(new EditTermsViewModel
                    {
                        TermId = plterm.Id,
                        TermType = plterm.SubTermType,
                        TermLimit = plterm.TermLimit,
                        Excess = Convert.ToInt32(plterm.Excess),
                        Premium = plterm.Premium
                    });
                }
                model.PLTerms = plterms.OrderBy(acat => acat.TermLimit).ToList();
                model.EDTerms = edterms.OrderBy(acat => acat.TermLimit).ToList();
                model.PITerms = piterms.OrderBy(acat => acat.TermLimit).ToList();
                model.ELTerms = elterms.OrderBy(acat => acat.TermLimit).ToList();
                model.CLTerms = clterms.OrderBy(acat => acat.TermLimit).ToList();
                model.SLTerms = slterms.OrderBy(acat => acat.TermLimit).ToList();
                model.DOTerms = doterms.OrderBy(acat => acat.TermLimit).ToList();
                ViewBag.Title = "Edit Terms ";

                return View("EditTerms", model);
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }                       
        }

        [HttpPost]
        public async Task<IActionResult> EditTerm(EditTermsViewModel clientAgreementBVTerm)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(clientAgreementBVTerm.clientAgreementId);
                ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV");

                ClientAgreementBVTerm bvTerm = null;
                if (term.BoatTerms != null)
                {
                    bvTerm = term.BoatTerms.FirstOrDefault(bvt => bvt.Boat.BoatName == clientAgreementBVTerm.BoatName);

                }
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    term.Premium -= bvTerm.Premium;
                    term.Premium += clientAgreementBVTerm.Premium;
                    bvTerm.TermLimit = clientAgreementBVTerm.TermLimit;
                    bvTerm.Excess = clientAgreementBVTerm.Excess;
                    bvTerm.Premium = clientAgreementBVTerm.Premium;
                    bvTerm.FSL = clientAgreementBVTerm.FSL;
                    await uow.Commit();
                }

                return RedirectToAction("EditTerms", new { id = clientAgreementBVTerm.clientAgreementId });
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditSubTerm(Guid clientAgreementId, EditTermsViewModel clientAgreementSubTerm)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(clientAgreementId);
                ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.Id == clientAgreementSubTerm.TermId && t.SubTermType == clientAgreementSubTerm.TermType && t.DateDeleted == null);

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    term.Premium = clientAgreementSubTerm.Premium;
                    term.TermLimit = clientAgreementSubTerm.TermLimit;
                    term.Excess = clientAgreementSubTerm.Excess;
                    await uow.Commit();
                }

                return RedirectToAction("EditTerms", new { id = clientAgreementId });
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditMotorTerm(Guid clientAgreementId, EditTermsViewModel clientAgreementMVTerm)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(clientAgreementId);
                ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV" && t.DateDeleted == null);

                ClientAgreementMVTerm mvTerm = null;
                if (term.MotorTerms != null)
                {
                    mvTerm = term.MotorTerms.FirstOrDefault(bvt => bvt.Model == clientAgreementMVTerm.Model);
                }

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    term.Premium -= mvTerm.Premium;
                    term.Premium += clientAgreementMVTerm.Premium;
                    mvTerm.TermLimit = clientAgreementMVTerm.TermLimit;
                    mvTerm.Excess = clientAgreementMVTerm.Excess;
                    mvTerm.Premium = clientAgreementMVTerm.Premium;
                    mvTerm.FSL = clientAgreementMVTerm.FSL;
                    await uow.Commit();
                }

                return RedirectToAction("EditTerms", new { id = clientAgreementId });
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteTerm( EditTermsViewModel clientAgreementBVTerm)
        {
            User user = null;
            try
            {
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(clientAgreementBVTerm.clientAgreementId);
                ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV" && t.DateDeleted == null);
                ClientAgreementBVTerm bvTerm = null;
                ClientAgreementMVTerm mvTerm = null;

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {

                    if (term.BoatTerms != null)
                    {

                        bvTerm = term.BoatTerms.FirstOrDefault(bvt => bvt.Id == clientAgreementBVTerm.VesselId);
                        term.BoatTerms.Remove(bvTerm);
                    }
                    if (term.MotorTerms != null)
                    {
                        mvTerm = term.MotorTerms.FirstOrDefault(bvt => bvt.Id == clientAgreementBVTerm.VesselId);
                        term.MotorTerms.Remove(mvTerm);
                    }
                    await uow.Commit();
                }

                return RedirectToAction("EditTerms", new { id = clientAgreementBVTerm.clientAgreementId });
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubTerm(EditTermsViewModel clientAgreementBVTerm)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(clientAgreementBVTerm.clientAgreementId);
                ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.Id == clientAgreementBVTerm.TermId && t.DateDeleted == null);

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {

                    term.DateDeleted = DateTime.UtcNow;
                    await uow.Commit();
                }

                return RedirectToAction("EditTerms", new { id = clientAgreementBVTerm.clientAgreementId });
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewAgreement(Guid id)
        {
            var models = new BaseListViewModel<ViewAgreementViewModel>();
            User user = null;
            try
            {
                user = await CurrentUser();
                ViewAgreementViewModel model;
                ClientProgramme clientProgramme = await _programmeService.GetClientProgrammebyId(id);
                Organisation insured = clientProgramme.Owner;
                ClientInformationSheet answerSheet = clientProgramme.InformationSheet;
                var insuranceRoles = new List<InsuranceRoleViewModel>();

                NumberFormatInfo currencyFormat = new CultureInfo(CultureInfo.CurrentCulture.ToString()).NumberFormat;
                currencyFormat.CurrencyNegativePattern = 2;

                // List Agreement Parties
                insuranceRoles.Add(new InsuranceRoleViewModel { RoleName = "Client", Name = insured.Name, ManagedBy = "", Email = "" });
                foreach (ClientAgreement agreement in clientProgramme.Agreements.Where(apa => apa.DateDeleted == null).OrderBy(apa => apa.Product.OrderNumber))
                {
                    model = new ViewAgreementViewModel
                    {
                        EditEnabled = true,
                        ClientAgreementId = agreement.Id,
                        ClientProgrammeId = clientProgramme.Id,
                        SentOnlineAcceptance = agreement.SentOnlineAcceptance
                    };

                    var insuranceInclusion = new List<InsuranceInclusion>();
                    var insuranceExclusion = new List<InsuranceExclusion>();
                    var riskPremiums = new List<RiskPremiumsViewModel>();
                    var multiCoverOptions = new List<MultiCoverOptions>();
                    string riskname = null;

                    // List Agreement Inclusions
                    if (agreement.Product.IsMultipleOption)
                    {
                        if (agreement.Product.Id == new Guid("79dc8bcd-01f2-4551-9caa-aa9200f1d659")) //NZACS DO
                        {
                            insuranceInclusion.Add(new InsuranceInclusion { RiskName = agreement.Product.Name, Inclusion = "Limit: Options displayed below / Extension Covers" });
                        }
                        else
                        {
                            insuranceInclusion.Add(new InsuranceInclusion { RiskName = agreement.Product.Name, Inclusion = "Limit: Options displayed below" });
                        }
                        insuranceExclusion.Add(new InsuranceExclusion { RiskName = agreement.Product.Name, Exclusion = "Excess: Options displayed below" });

                    }
                    else
                    {
                        foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms)
                        {
                            if (term.SubTermType == "MV")
                            {
                                riskname = "Motor Vehicle";
                            }
                            else if (term.SubTermType == "BV")
                            {
                                riskname = "Vessel";
                            }
                            else
                            {
                                riskname = agreement.Product.Name;
                            }
                            insuranceInclusion.Add(new InsuranceInclusion { RiskName = riskname, Inclusion = term.TermLimit.ToString("C0", UserCulture) });
                        }

                        // List Agreement Exclusions
                        if (agreement.Product.Id == new Guid("107c38d6-0d46-4ec1-b3bd-a73b0021f2e3")) //HIANZ
                        {
                            foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms)
                            {
                                insuranceExclusion.Add(new InsuranceExclusion
                                {
                                    RiskName = "Motor Vehicle",
                                    Exclusion = "Excess: <br /> - 1% of Sum Insured subject to a minimum of $500 " +
                                                                    "<br /> - theft excess 1 % of the sum insured with a minimum of $1,000 including whilst on hire, non return from hire and from the clients yard " +
                                                                    "<br /> - theft excess nil for any vehicle or item insured fitted with a GPS tracking device " +
                                                                    "<br /> PLUS " +
                                                                    "<br /> - Whilst being driven by any person under 25 years of age $500 " +
                                                                    "<br /> - Breach of Warranty / Invalidation Clause $1, 000"
                                });
                            }
                        }
                        else if (agreement.Product.Id == new Guid("bc62172c-1e15-4e5a-8547-a7bd002121eb"))
                        { //Arcco
                            foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms)
                            {
                                insuranceExclusion.Add(new InsuranceExclusion
                                {
                                    RiskName = "Motor Vehicle",
                                    Exclusion = "Excess: <br /> $2,000 each and every claim. " +
                                                                    "<br /> An additional $1,000 excess applies when the vehicle is on hire and is being driven by an under 21 year old driver or has held a full licence less than 12 months. " +
                                                                    "<br /> $500 excess on trailers. "
                                });
                            }
                        }
                        else if (agreement.Product.Id == new Guid("e2eae6d8-d68e-4a40-b50a-f200f393777a"))
                        { //CoastGuard
                            foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms)
                            {
                                insuranceExclusion.Add(new InsuranceExclusion
                                {
                                    RiskName = "Vessel",
                                    Exclusion = "Excess: refer to vessel excess "
                                });
                            }
                        }
                        else
                        {
                            foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms)
                            {
                                insuranceExclusion.Add(new InsuranceExclusion
                                {
                                    RiskName = agreement.Product.Name,
                                    Exclusion = "Excess: " + term.Excess.ToString("C", UserCulture)
                                });
                            }
                        }
                    }

                    foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms.Where(t => t.DateDeleted == null).OrderBy(acat => acat.TermLimit))
                    {

                        multiCoverOptions.Add(new MultiCoverOptions { TermId = term.Id, isSelected = (term.Bound == true) ? "checked" : "", ProductId = agreement.Product.Id, RiskName = agreement.Product.Name, Inclusion = "Limit: " + term.TermLimit.ToString("C", UserCulture), Exclusion = "Excess: " + term.Excess.ToString("C", UserCulture), TotalPremium = term.Premium.ToString("C", UserCulture) });
                    }

                    // List Agreement Premiums
                    foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms.Where(t => t.DateDeleted == null))
                    {
                        if (answerSheet.IsChange && answerSheet.PreviousInformationSheet != null)
                        {
                            riskPremiums.Add(new RiskPremiumsViewModel
                            {
                                RiskName = riskname,
                                Premium = (term.PremiumDiffer - term.FSLDiffer).ToString("C", UserCulture),
                                FSL = term.FSLDiffer.ToString("C", UserCulture),
                                TotalPremium = term.PremiumDiffer.ToString("C", UserCulture),
                                TotalPremiumIncFeeGST = ((term.PremiumDiffer + agreement.BrokerFee) * agreement.ClientInformationSheet.Programme.BaseProgramme.TaxRate).ToString("C", UserCulture),
                                TotalPremiumIncFeeIncGST = ((term.PremiumDiffer + agreement.BrokerFee) * (1 + agreement.ClientInformationSheet.Programme.BaseProgramme.TaxRate)).ToString("C", UserCulture)
                            });
                        }
                        else
                        {
                            riskPremiums.Add(new RiskPremiumsViewModel
                            {
                                RiskName = riskname,
                                Premium = (term.Premium - term.FSL).ToString("C", UserCulture),
                                FSL = term.FSL.ToString("C", UserCulture),
                                TotalPremium = term.Premium.ToString("C", UserCulture),
                                TotalPremiumIncFeeGST = ((term.Premium + agreement.BrokerFee) * agreement.ClientInformationSheet.Programme.BaseProgramme.TaxRate).ToString("C", UserCulture),
                                TotalPremiumIncFeeIncGST = ((term.Premium + agreement.BrokerFee) * (1 + agreement.ClientInformationSheet.Programme.BaseProgramme.TaxRate)).ToString("C", UserCulture)
                            });
                        }
                    }

                    // Populate the ViewModel
                    model.Sheetstatus = answerSheet.Status;
                    model.InsuranceRoles = insuranceRoles;
                    model.Inclusions = insuranceInclusion;
                    model.Exclusions = insuranceExclusion;
                    model.RiskPremiums = riskPremiums;
                    model.MultiCoverOptions = multiCoverOptions;
                    model.Status = answerSheet.Status;

                    // Status
                    model.ProductName = agreement.Product.Name;
                    model.IsMultipleOption = agreement.Product.IsMultipleOption;
                    model.IsOptionalProduct = agreement.Product.IsOptionalProduct;
                    model.Status = agreement.Status;
                    if (agreement.Status == "Referred")
                    {
                        var milestone = await _milestoneService.GetMilestoneByBaseProgramme(answerSheet.Programme.BaseProgramme.Id);
                        if (milestone != null)
                        {
                            var advisoryList = await _advisoryService.GetAdvisorysByMilestone(milestone);
                            var advisory = advisoryList.LastOrDefault(a => a.Activity.Name == "Agreement Status – Referred" && a.DateDeleted == null);
                            var userTaskList = await _taskingService.GetUserTasksByMilestone(milestone);
                            var userTask = userTaskList.LastOrDefault(t => t.Activity.Name == "Agreement Status – Referred" && t.DateDeleted == null);
                            if (userTask != null)
                            {
                                userTask.IsActive = true;
                                await _taskingService.UpdateUserTask(userTask);
                            }
                            if (advisory != null)
                            {
                                model.Advisory = advisory.Description;
                            }

                        }
                        //_emailService.SendSystemEmailAgreementReferNotify(user, answerSheet.Programme.BaseProgramme, agreement, answerSheet.Owner);
                    }
                    model.InformationSheetStatus = agreement.ClientInformationSheet.Status;
                    Boolean nextInfoSheet = false;
                    Boolean IsChange = false;

                    if (null != agreement.ClientInformationSheet.NextInformationSheet)
                    {
                        model.NextInfoSheet = true;
                    }
                    else
                    {
                        model.NextInfoSheet = false;
                    }

                    if (null != agreement.ClientInformationSheet.IsChange)
                    {
                        model.IsChange = agreement.ClientInformationSheet.IsChange;
                    }

                    model.StartDate = LocalizeTimeDate(agreement.InceptionDate, "dd-mm-yyyy");
                    model.EndDate = LocalizeTimeDate(agreement.ExpiryDate, "dd-mm-yyyy");
                    model.AdministrationFee = agreement.BrokerFee.ToString("C", UserCulture);
                    model.BrokerageRate = (agreement.Brokerage / 100).ToString("P2", UserCulture);
                    model.CurrencySymbol = "fa fa-dollar";
                    if (agreement.ClientInformationSheet.Programme.BaseProgramme.UsesEGlobal &&
                        agreement.ClientInformationSheet.Programme.EGlobalBranchCode != null && agreement.ClientInformationSheet.Programme.EGlobalClientNumber != null)
                    {
                        model.ClientNumber = agreement.ClientInformationSheet.Programme.EGlobalBranchCode + "-" + agreement.ClientInformationSheet.Programme.EGlobalClientNumber;
                    }
                    else
                    {
                        model.ClientNumber = agreement.ClientNumber;
                    }
                    model.PolicyNumber = agreement.PolicyNumber;

                    // MV
                    model.HasVehicles = answerSheet.Vehicles.Count > 0;
                    if (model.HasVehicles)
                    {
                        var vehicles = new List<VehicleViewModel>();
                        foreach (Vehicle v in answerSheet.Vehicles)
                        {
                            if (!v.Removed && v.DateDeleted == null && v.VehicleCeaseDate == DateTime.MinValue)
                            {
                                vehicles.Add(new VehicleViewModel { VehicleCategory = v.VehicleCategory, Make = v.Make, Year = v.Year, Registration = v.Registration, FleetNumber = v.FleetNumber, VehicleModel = v.Model, SumInsured = v.GroupSumInsured });
                            }
                        }
                        model.Vehicles = vehicles;
                    }

                    // BV
                    model.HasBoats = answerSheet.Boats.Count > 0;
                    try
                    {
                        if (model.HasBoats)
                        {
                            var boats = new List<BoatViewModel>();
                            foreach (Boat b in answerSheet.Boats)
                            {
                                if (!b.Removed && b.DateDeleted == null && b.BoatCeaseDate == DateTime.MinValue)
                                {
                                    boats.Add(new BoatViewModel { BoatName = b.BoatName, BoatMake = b.BoatMake, BoatModel = b.BoatModel, MaxSumInsured = b.MaxSumInsured, BoatQuoteExcessOption = b.BoatQuoteExcessOption });
                                }
                            }
                            model.Boats = boats;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.InnerException);
                    }

                    try
                    {
                        if (model.HasBoats)
                        {
                            var bvterms = new List<EditTermsViewModel>();
                            foreach (ClientAgreementBVTerm bt in
                                agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV" && t.DateDeleted == null).BoatTerms)
                            {
                                if (bt.DateDeleted == null)
                                {
                                    bvterms.Add(new EditTermsViewModel
                                    {
                                        BoatName = bt.BoatName,
                                        BoatMake = bt.BoatMake,
                                        BoatModel = bt.BoatModel,
                                        TermLimit = bt.TermLimit,
                                        Excess = bt.Excess
                                    });
                                }
                            }
                            model.BVTerms = bvterms;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.InnerException);
                    }

                    model.InformationSheetId = answerSheet.Id;
                    models.Add(model);
                }

                ViewBag.Title = clientProgramme.BaseProgramme.Name + " Agreement for " + insured.Name;
                ViewBag.Status = answerSheet.Status;
                ClientAgreement masterclientagreement = clientProgramme.Agreements.Where(cpam => cpam.MasterAgreement).FirstOrDefault();
                if (clientProgramme.BaseProgramme.StopAgreement && masterclientagreement.DateCreated >= clientProgramme.BaseProgramme.StopAgreementDateTime)
                {
                    model = new ViewAgreementViewModel();
                    model.ProgrammeStopAgreement = clientProgramme.BaseProgramme.StopAgreement;
                    model.AgreementMessage = clientProgramme.BaseProgramme.StopAgreementMessage;

                    if (clientProgramme.InformationSheet.Status != "Submitted" && clientProgramme.InformationSheet.Status != "Bound")
                    {
                        using (var uow = _unitOfWork.BeginUnitOfWork())
                        {
                            clientProgramme.InformationSheet.Status = "Submitted";
                            clientProgramme.InformationSheet.SubmitDate = DateTime.UtcNow;
                            clientProgramme.InformationSheet.SubmittedBy = user;
                            await uow.Commit();
                        }

                    }

                    return PartialView("_ViewStopAgreementMessage", model);
                }
                else if (clientProgramme.BaseProgramme.HasSubsystemEnabled)
                {
                    model = new ViewAgreementViewModel();
                    model.ProgrammeStopAgreement = true;
                    var message = clientProgramme.BaseProgramme.SubsystemMessage;
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        message = "subsystem invoked";
                    }
                    model.AgreementMessage = message;

                    var isBaseClientProgramme = await _programmeService.IsBaseClass(clientProgramme);
                    if (isBaseClientProgramme)
                    {
                        bool isComplete;
                        if (clientProgramme.SubClientProgrammes.Count != 0)
                        {
                            isComplete = await _programmeService.SubsystemCompleted(clientProgramme);
                            if (isComplete)
                            {
                                return PartialView("_ViewAgreementList", models);
                            }
                            else
                            {
                                model.AgreementMessage = "not all subsystem client information sheets have been completed";
                                return PartialView("_ViewStopAgreementMessage", model);
                            }
                        }
                        else
                        {
                            try
                            {
                                await _subsystemService.CreateSubObjects(clientProgramme.Id, answerSheet);
                                return PartialView("_ViewStopAgreementMessage", model);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Create Sub Objects failed", ex);
                            }
                        }
                    }
                    else
                    {
                        message = "subsystem client information sheet completed and have notified the broker";
                        //Notify broker 
                        return PartialView("_ViewStopAgreementMessage", model);
                    }

                }
                else
                {
                    return PartialView("_ViewAgreementList", models);
                }
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }                       
        }

        

        [HttpPost]
        public async Task<IActionResult> SaveDate(Guid id , string Startdate)
        {
            User user = await CurrentUser();
            try
            {
                ClientAgreement clientAgreement = await _clientAgreementService.GetAgreement(id);
                user = await CurrentUser();
                var date = true;
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    // TODO - Convert to UTC
                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(UserTimeZone);
                    clientAgreement.InceptionDate = DateTime.Parse(Startdate, UserCulture).ToUniversalTime(tzi);
                    clientAgreement.ExpiryDate = clientAgreement.InceptionDate.AddYears(1);
                    clientAgreement.CustomInceptionDate = true;

                    //boat effective date if the date is prior to the new start date or over 30 days after the new start date
                    foreach (var boat in clientAgreement.ClientInformationSheet.Boats.Where(b => !b.Removed && b.DateDeleted == null))
                    {
                        if (boat.BoatEffectiveDate < clientAgreement.InceptionDate || boat.BoatEffectiveDate > clientAgreement.InceptionDate.AddDays(30))
                        {
                            boat.BoatEffectiveDate = clientAgreement.InceptionDate;
                        }
                    }

                    //update vehicle effective date if the date is prior to the new start date or over 30 days after the new start date
                    foreach (var vehicle in clientAgreement.ClientInformationSheet.Vehicles.Where(v => !v.Removed && v.DateDeleted == null))
                    {
                        if (vehicle.VehicleEffectiveDate < clientAgreement.InceptionDate || vehicle.VehicleEffectiveDate > clientAgreement.InceptionDate.AddDays(30))
                        {
                            vehicle.VehicleEffectiveDate = clientAgreement.InceptionDate;
                        }
                    }

                    string auditLogDetail = "Agreement start date and end date have been modified by " + user.FullName;
                    AuditLog auditLog = new AuditLog(user, clientAgreement.ClientInformationSheet, clientAgreement, auditLogDetail);
                    clientAgreement.ClientAgreementAuditLogs.Add(auditLog);

                    await uow.Commit();
                }

                var url = "/Information/EditInformation/" + clientAgreement.ClientInformationSheet.Programme.Id;
                return Json(new { url });
                //return Json(date);
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> ViewAgreementDeclaration(Guid id)
        {
            var models = new BaseListViewModel<ViewAgreementViewModel>();
            User user = null;

            try
            {
                var clientProgramme = await _programmeService.GetClientProgrammebyId(id);
                Organisation insured = clientProgramme.Owner;
                ClientInformationSheet answerSheet = clientProgramme.InformationSheet;

                models.BaseProgramme = clientProgramme.BaseProgramme;
                var advisoryDesc = "";
                var milestone = await _milestoneService.GetMilestoneByBaseProgramme(clientProgramme.BaseProgramme.Id);
                if (milestone != null)
                {
                    var advisoryList = await _advisoryService.GetAdvisorysByMilestone(milestone);
                    var advisory = advisoryList.LastOrDefault(a => a.Activity.Name == "Agreement Status - Declined" && a.DateDeleted == null);
                    if (advisory != null)
                    {
                        advisoryDesc = advisory.Description;
                    }

                }

                //subsystem check
                if(clientProgramme.Agreements.Count == 0)
                {
                    ViewAgreementViewModel model = new ViewAgreementViewModel
                    {
                        EditEnabled = true,
                        ClientProgrammeId = clientProgramme.Id,
                        Declaration = "Declaration for subsystem"
                    };

                    model.Advisory = advisoryDesc;
                    model.Status = answerSheet.Status;
                    model.InformationSheetId = answerSheet.Id;
                    models.Add(model);
                }
                else
                {
                    foreach (ClientAgreement agreement in clientProgramme.Agreements)
                    {
                        ViewAgreementViewModel model = new ViewAgreementViewModel
                        {
                            EditEnabled = true,
                            ClientAgreementId = agreement.Id,
                            ClientProgrammeId = clientProgramme.Id,
                            Declaration = clientProgramme.BaseProgramme.Declaration
                        };

                        model.Advisory = advisoryDesc;
                        model.Status = agreement.Status;
                        model.InformationSheetId = answerSheet.Id;
                        models.Add(model);
                    }
                    
                }
                
                ViewBag.Title = clientProgramme.BaseProgramme.Name + " Agreement for " + insured.Name;

                return PartialView("_ViewAgreementDeclaration", models);
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> ViewPayment(Guid id)
        {
            User user = null;

            try
            {
                user = await CurrentUser();
                //need to review this code duplication
                var models = new BaseListViewModel<ViewAgreementViewModel>();

                ClientProgramme clientProgramme = await _programmeService.GetClientProgrammebyId(id);
                ClientInformationSheet answerSheet = clientProgramme.InformationSheet;
                NumberFormatInfo currencyFormat = new CultureInfo(CultureInfo.CurrentCulture.ToString()).NumberFormat;
                currencyFormat.CurrencyNegativePattern = 2;

                decimal totalPayable = 0M;
                foreach (ClientAgreement agreement in clientProgramme.Agreements)
                {
                    ViewAgreementViewModel model = new ViewAgreementViewModel
                    {
                        EditEnabled = true,
                        ClientAgreementId = agreement.Id,
                        ClientProgrammeId = clientProgramme.Id
                    };

                    var riskPremiums = new List<RiskPremiumsViewModel>();
                    string riskname = null;

                    // List Agreement Inclusions
                    foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms)
                    {
                        if (term.SubTermType == "MV")
                        {
                            riskname = "Motor Vehicle";
                        }
                        else if (term.SubTermType == "BV")
                        {
                            riskname = "Vessel";
                        }
                        else
                        {
                            riskname = agreement.Product.Name;
                        }
                    }

                    // List Agreement Premiums
                    foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms)
                    {
                        if (answerSheet.PreviousInformationSheet == null)
                        {
                            riskPremiums.Add(new RiskPremiumsViewModel { RiskName = riskname, Premium = (term.Premium - term.FSL).ToString("C"), FSL = term.FSL.ToString("C"), TotalPremium = term.Premium.ToString("C") });
                            totalPayable += term.Premium;
                        }
                        else
                        {
                            riskPremiums.Add(new RiskPremiumsViewModel { RiskName = riskname, Premium = string.Format(currencyFormat, "{0:c}", (term.PremiumDiffer - term.FSLDiffer)), FSL = string.Format(currencyFormat, "{0:c}", term.FSLDiffer), TotalPremium = string.Format(currencyFormat, "{0:c}", term.PremiumDiffer) });
                            totalPayable += term.PremiumDiffer;
                        }
                    }

                    bool isActive = true;

                    model.EGlobalIsActive = isActive;

                    // Populate the ViewModel
                    model.RiskPremiums = riskPremiums;
                    //model.EGlobalIsActive = isActive;

                    // Status
                    model.ProductName = agreement.Product.Name;
                    model.Status = agreement.Status;
                    model.StartDate = LocalizeTime(agreement.InceptionDate, "d");
                    model.EndDate = LocalizeTime(agreement.ExpiryDate, "d");
                    model.AdministrationFee = agreement.BrokerFee.ToString("C");
                    model.BrokerageRate = (agreement.Brokerage / 100).ToString("P2");
                    model.CurrencySymbol = "fa fa-dollar";
                    model.ClientNumber = agreement.ClientNumber;
                    model.PolicyNumber = agreement.PolicyNumber;

                    model.NoPaymentRequiredMessage = clientProgramme.BaseProgramme.NoPaymentRequiredMessage;

                    models.Add(model);
                }

                ViewBag.Title = clientProgramme.BaseProgramme.Name + " Payment for " + clientProgramme.Owner.Name;

                bool requirePayment = false;
                if (clientProgramme.BaseProgramme.HasCCPayment && totalPayable > 0)
                {
                    requirePayment = true;
                }

                if (requirePayment)
                {
                    return PartialView("_ViewPaymentList", models);
                }
                else
                {
                    return PartialView("_ViewNoPaymentRequiredMsg", models);
                }
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditAgreement(Guid id)
        {
            ViewAgreementViewModel model = new ViewAgreementViewModel();
            User user = null;
            try
            {
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(id);
                ClientInformationSheet answerSheet = agreement.ClientInformationSheet;
                Organisation insured = answerSheet.Owner;
                ClientProgramme programme = answerSheet.Programme;
                user = await CurrentUser();

                model.InformationSheetId = answerSheet.Id;
                model.ClientAgreementId = agreement.Id;
                model.ClientProgrammeId = programme.Id;
                model.StartDate = LocalizeTimeDate(agreement.InceptionDate, "dd-mm-yyyy");
                model.EndDate = LocalizeTimeDate(agreement.ExpiryDate, "dd-mm-yyyy");
                model.AdministrationFee = agreement.BrokerFee.ToString("C");
                model.BrokerageRate = (agreement.Brokerage / 100).ToString("P2");
                model.CurrencySymbol = "fa fa-dollar";
                model.ClientNumber = agreement.ClientNumber;
                model.PolicyNumber = agreement.PolicyNumber;
                model.RetroactiveDate = agreement.RetroactiveDate;

                ViewBag.Title = answerSheet.Programme.BaseProgramme.Name + " Edit Agreement for " + insured.Name;

                return View("EditAgreement", model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAgreement(ViewAgreementViewModel model)
        {
            User user = null;
            try
            {
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(model.ClientAgreementId);
                ClientInformationSheet answerSheet = agreement.ClientInformationSheet;
                user = await CurrentUser();
                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    // TODO - Convert to UTC
                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(UserTimeZone);
                    agreement.InceptionDate = DateTime.Parse(model.StartDate, UserCulture).ToUniversalTime();
                    agreement.ExpiryDate = DateTime.Parse(model.EndDate, UserCulture).ToUniversalTime();
                    agreement.Brokerage = Convert.ToDecimal(model.BrokerageRate.Replace("%", ""));
                    agreement.BrokerFee = Convert.ToDecimal(model.AdministrationFee.Replace("$", ""));
                    agreement.ClientNumber = model.ClientNumber;
                    agreement.PolicyNumber = model.PolicyNumber;
                    agreement.RetroactiveDate = model.RetroactiveDate;

                    string auditLogDetail = "Agreement details have been modified by " + user.FullName;
                    AuditLog auditLog = new AuditLog(user, answerSheet, agreement, auditLogDetail);
                    agreement.ClientAgreementAuditLogs.Add(auditLog);

                    await uow.Commit();
                }

                return Redirect("/Agreement/ViewAcceptedAgreement/" + answerSheet.Programme.Id);
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewAgreementRule(Guid id)
        {
            ViewAgreementRuleViewModel model = new ViewAgreementRuleViewModel();
            User user = null;
            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(id);
                ClientInformationSheet answerSheet = agreement.ClientInformationSheet;
                Organisation insured = answerSheet.Owner;

                //Client Agreement Rules
                model.HasRules = agreement.ClientAgreementRules.Count > 0;

                model.ClientAgreementID = id;
                model.ClientProgrammeID = answerSheet.Programme.Id;

                if (model.HasRules)
                {
                    var clientAgreementRules = new AgreementRulesViewModel();
                    var clientAgreementRulesTypeRate = new AgreementRulesViewModel();
                    foreach (ClientAgreementRule cr in agreement.ClientAgreementRules.OrderBy(cr => cr.OrderNumber))
                    {
                        clientAgreementRules.Add(new ClientAgreementRuleViewModel { ClientAgreementRuleID = cr.Id, Description = cr.Description, Value = cr.Value });
                        if (cr.RuleCategory == "uwrate")
                        {
                            clientAgreementRulesTypeRate.Add(new ClientAgreementRuleViewModel { ClientAgreementRuleID = cr.Id, Description = cr.Description, Value = cr.Value });
                        }
                    }
                    model.ClientAgreementRules = clientAgreementRules;
                    model.ClientAgreementRulesTypeRate = clientAgreementRulesTypeRate;
                }

                ViewBag.Title = answerSheet.Programme.BaseProgramme.Name + " Agreement Rule for " + insured.Name;

                return View("ViewAgreementRule", model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ViewAgreementRule(ViewAgreementRuleViewModel model)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(model.ClientAgreementID);
                if (model.ClientAgreementRules.Any(mcr => mcr != null && mcr.Value != null))
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        foreach (ClientAgreementRuleViewModel crv in model.ClientAgreementRules.OrderBy(cr => cr.OrderNumber))
                        {
                            var clientAgreementRule = await _clientAgreementRuleService.GetClientAgreementRuleBy(crv.ClientAgreementRuleID);
                            clientAgreementRule.Value = crv.Value;
                        }
                        await uow.Commit();
                    }
                }

                return Redirect("/Information/EditInformation/" + model.ClientProgrammeID);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> ViewAgreementEndorsement(Guid id)
        {
            ViewAgreementEndorsementViewModel model = new ViewAgreementEndorsementViewModel();
            User user = null;
            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(id);
                ClientInformationSheet answerSheet = agreement.ClientInformationSheet;
                Organisation insured = answerSheet.Owner;

                //Client Agreement Endorsements
                model.HasEndorsements = agreement.ClientAgreementEndorsements.Count > 0;

                model.ClientAgreementID = id;

                model.ClientProgrammeID = answerSheet.Programme.Id;

                if (model.HasEndorsements)
                {
                    var clientAgreementEndorsements = new AgreementEndorsementsViewModel();
                    foreach (ClientAgreementEndorsement ce in agreement.ClientAgreementEndorsements.Where(ce => ce.DateDeleted == null).OrderBy(ce => ce.OrderNumber))
                    {
                        clientAgreementEndorsements.Add(new ClientAgreementEndorsementViewModel { ClientAgreementEndorsementID = ce.Id, Name = ce.Name, Value = ce.Value });
                    }
                    model.ClientAgreementEndorsements = clientAgreementEndorsements;

                }
                else
                {
                    model.ClientAgreementEndorsements = null;
                }

                ViewBag.Title = answerSheet.Programme.BaseProgramme.Name + " Agreement Endorsements for " + insured.Name;

                return View("ViewAgreementEndorsement", model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ViewAgreementEndorsement(ViewAgreementEndorsementViewModel model)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                ClientAgreement agreement = await _clientAgreementService.GetAgreement(model.ClientAgreementID);
                if (model.EndorsementNameToAdd != null && model.EndorsementTextToAdd != null)
                {
                    await _clientAgreementEndorsementService.AddClientAgreementEndorsement(user, model.EndorsementNameToAdd, "Exclusion", agreement.Product, model.EndorsementTextToAdd, 100, agreement);
                }

                if (model.ClientAgreementEndorsements != null)
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        foreach (ClientAgreementEndorsementViewModel cev in model.ClientAgreementEndorsements.OrderBy(ce => ce.OrderNumber))
                        {
                            var clientAgreement = await _clientAgreementEndorsementService.GetClientAgreementEndorsementBy(cev.ClientAgreementEndorsementID);
                            cev.Value = clientAgreement.Value;
                        }

                        await uow.Commit();
                    }

                }

                return Redirect(model.ClientAgreementID.ToString());
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> AcceptAgreement(Guid Id)
        {
            User user = null;
            List<AgreementDocumentViewModel> models = new List<AgreementDocumentViewModel>();
            try
            {
                ClientProgramme programme = await _programmeService.GetClientProgrammebyId(Id);
                user = await CurrentUser();
                foreach (ClientAgreement agreement in programme.Agreements)
                {
                    if (agreement == null)
                        throw new Exception(string.Format("No Agreement found for {0}", agreement.Id));

                    var agreeDocList = agreement.GetDocuments();
                    foreach (SystemDocument doc in agreeDocList)
                    {
                        doc.Delete(user);
                    }

                    foreach (SystemDocument template in agreement.Product.Documents)
                    {
                        SystemDocument renderedDoc = await _fileService.RenderDocument(user, template, agreement, null);
                        renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                        agreement.Documents.Add(renderedDoc);
                        await _fileService.UploadFile(renderedDoc);
                    }

                    ClientAgreement reloadedAgreement = await _clientAgreementService.GetAgreement(agreement.Id);
                    agreeDocList = reloadedAgreement.GetDocuments();
                    foreach (SystemDocument doc in agreeDocList)
                    {
                        if (doc.DocumentType == 4)
                        {
                            if (programme.EGlobalClientNumber != null)
                            {
                                models.Add(new AgreementDocumentViewModel { DisplayName = doc.Name, Url = "/File/GetDocument/" + doc.Id });
                            }
                        }
                        else
                            models.Add(new AgreementDocumentViewModel { DisplayName = doc.Name, Url = "/File/GetDocument/" + doc.Id });
                    }

                    if (agreement.Product.Id == new Guid("bc62172c-1e15-4e5a-8547-a7bd002121eb"))
                    { //Arcco
                        await _clientAgreementService.AcceptAgreement(agreement, user);
                    }

                }

                return PartialView("_ViewAgreementDocs", models);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> ByPassPayment (IFormCollection collection)
        {
            Guid sheetId = Guid.Empty;
            ClientInformationSheet sheet = null;
            User user = null;
            try
            {
                if (Guid.TryParse(HttpContext.Request.Form["ClientInformationSheet.Id"], out sheetId))
                {
                    sheet = await _customerInformationService.GetInformation(sheetId);
                }

                ClientProgramme programme = sheet.Programme;
                user = await CurrentUser();                
                var status = "Bound";
                if (sheet.Programme.BaseProgramme.UsesEGlobal)
                {
                    status = "Bound and invoice pending";
                }
                
                foreach (ClientAgreement agreement in programme.Agreements)
                {
                    var allDocs = await _fileService.GetDocumentByOwner(programme.Owner);
                    var documents = new List<SystemDocument>();
                    var agreeTemplateList = agreement.Product.Documents;
                    var agreeDocList = agreement.GetDocuments();
                    
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (agreement.Status != status)
                        {
                            agreement.Status = status;
                            agreement.BoundDate = DateTime.Now;
                            if (programme.BaseProgramme.PolicyNumberPrefixString != null)
                            {
                                agreement.PolicyNumber = programme.BaseProgramme.PolicyNumberPrefixString + "-0" + agreement.ReferenceId;                                
                            }
                            await uow.Commit();
                        }
                    }

                    agreement.Status = status;
                    
                    foreach (SystemDocument doc in agreeDocList)
                    {
                        doc.Delete(user);
                    }
                                       
                    foreach (SystemDocument template in agreeTemplateList)
                    {
                        //render docs except invoice
                        if (template.DocumentType != 4 && template.DocumentType != 6)
                        {
                            SystemDocument renderedDoc = await _fileService.RenderDocument(user, template, agreement, null);
                            renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                            agreement.Documents.Add(renderedDoc);
                            documents.Add(renderedDoc);
                            await _fileService.UploadFile(renderedDoc);
                        }
                        //render all subsystem
                        if (template.DocumentType == 6)
                        {
                            foreach(var subSystemClient in sheet.SubClientInformationSheets)
                            {
                                SystemDocument renderedDoc = await _fileService.RenderDocument(user, template, agreement, subSystemClient);
                                renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                                agreement.Documents.Add(renderedDoc);
                                documents.Add(renderedDoc);
                                await _fileService.UploadFile(renderedDoc);
                            }                            
                        }
                    }

                    if (programme.BaseProgramme.ProgEnableEmail)
                    {
                        //send out policy document email
                        EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == "SendPolicyDocuments");
                        if (emailTemplate != null)
                        {
                            await _emailService.SendEmailViaEmailTemplate(programme.Owner.Email, emailTemplate, documents, null, null);
                        }
                        //send out agreement bound notification email
                        await _emailService.SendSystemEmailAgreementBoundNotify(programme.BrokerContactUser, programme.BaseProgramme, agreement, programme.Owner);
                    }

                }

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    if (programme.InformationSheet.Status != status)
                    {
                        programme.InformationSheet.Status = status;
                        await uow.Commit();
                    }
                }
                
                var url = "/Agreement/ViewAcceptedAgreement/" + programme.Id;
                return Json(new { url });

            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SendPolicyDocuments(Guid id)
        {
            User user = null;
            try
            {
                ClientInformationSheet sheet = await _customerInformationService.GetInformation(id);

                // TODO - rewrite to save templates on a per programme basis

                ClientProgramme programme = sheet.Programme;
                ClientAgreement agreement = programme.Agreements[0];

                Organisation insured = programme.Owner;

                EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == "SendPolicyDocuments");

                EmailTemplateViewModel model = new EmailTemplateViewModel();

                user = await CurrentUser();

                if (emailTemplate != null)
                {
                    model.Name = emailTemplate.Name;
                    model.Subject = emailTemplate.Subject;
                    model.Body = System.Net.WebUtility.HtmlDecode(emailTemplate.Body);
                }
                else
                {
                    model.Name = "";
                    model.Subject = "";
                    model.Body = "";
                }

                model.ClientProgrammeID = programme.Id;

                if (programme.Owner != null)
                {
                    var recipents = new List<UserViewModel>();

                    recipents.Add(new UserViewModel { ID = user.Id, UserName = user.UserName, FirstName = user.FirstName, LastName = user.LastName, FullName = user.FullName, Email = user.Email });

                    var recipentList = await _userService.GetAllUsers();
                    foreach (User recipent in recipentList.Where(ur1 => ur1.Organisations.Contains(programme.Owner)))
                    {
                        recipents.Add(new UserViewModel { ID = recipent.Id, UserName = recipent.UserName, FirstName = recipent.FirstName, LastName = recipent.LastName, FullName = recipent.FullName, Email = recipent.Email });
                    }

                    model.Recipents = recipents;
                }
                else
                {
                    model.Recipents = null;
                }

                ViewBag.Title = programme.BaseProgramme.Name + " Agreement Documents Covering Text";

                return View("SendPolicyDocuments", model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendPolicyDocuments(EmailTemplateViewModel model)
        {
            User user = null;
            try
            {
                ClientProgramme programme = await _programmeService.GetClientProgrammebyId(model.ClientProgrammeID);
                user = await CurrentUser();
                // TODO - rewrite to save templates on a per programme basis
                ClientAgreement agreement = programme.Agreements[0];
                //EmailTemplate emailTemplate = agreement.Product.EmailTemplates.FirstOrDefault (et => et.Type == "SendPolicyDocuments");
                EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == "SendPolicyDocuments");

                if (emailTemplate != null)
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        emailTemplate.Subject = model.Subject;
                        emailTemplate.Body = model.Body;
                        emailTemplate.LastModifiedBy = user;
                        emailTemplate.LastModifiedOn = DateTime.UtcNow;

                        await uow.Commit();
                    }
                }
                else
                {
                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        emailTemplate = new EmailTemplate(user, "Agreement Documents Covering Text", "SendPolicyDocuments", model.Subject, model.Body, null, programme.BaseProgramme);
                        programme.BaseProgramme.EmailTemplates.Add(emailTemplate);

                        await uow.Commit();
                    }
                }

                var docs = agreement.GetDocuments();
                var documents = new List<SystemDocument>();

                if (docs != null)
                {
                    foreach (SystemDocument doc in docs)
                    {
                        if (doc.DateDeleted == null)
                        {
                            documents.Add(doc);
                        }
                    }
                }
                else
                {

                    documents = null;
                }

                string strrecipentemail = null;
                if (model.Recipent != null)
                {
                    var userDb = await _userService.GetUserById(model.Recipent);
                    strrecipentemail = user.Email;
                }

                //await _emailService.SendEmailViaEmailTemplate(strrecipentemail, emailTemplate, documents, null, null);

                return Redirect("~/Home/Index");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePxPayment(IFormCollection collection)
        {
            Guid sheetId = Guid.Empty;
            ClientInformationSheet sheet = null;
            User user = null;
            try
            {
                if (Guid.TryParse(HttpContext.Request.Form["AnswerSheetId"], out sheetId))
                {
                    sheet = await _customerInformationService.GetInformation(sheetId);
                }

                ClientProgramme programme = sheet.Programme;
                programme.PaymentType = "Credit Card";

                //var active = _httpClientService.GetEglobalStatus().Result;

                //Hardcoded variables
                decimal totalPremium = 0, totalPayment, brokerFee = 0, GST = 1.15m, creditCharge = 1.02m;
                Merchant merchant = await _merchantService.GetMerchant(programme.BaseProgramme.Id);
                Payment payment = await _paymentService.GetPayment(programme.Id);
                if (payment == null)
                {
                    payment = await _paymentService.AddNewPayment(sheet.CreatedBy, programme, merchant, merchant.MerchantPaymentGateway);
                }

                using (var uow = _unitOfWork.BeginUnitOfWork())
                {
                    programme.PaymentType = "Credit Card";
                    programme.Payment = payment;
                    programme.InformationSheet.Status = "Bound";
                    await uow.Commit();
                }

                //add check to count how many failed payments
                var ProgrammeId = sheetId;
                foreach (ClientAgreement clientAgreement in programme.Agreements)
                {
                    ProgrammeId = programme.Id;
                    brokerFee += clientAgreement.BrokerFee;
                    var terms = await _clientAgreementTermService.GetAllAgreementTermFor(clientAgreement);
                    foreach (ClientAgreementTerm clientAgreementTerm in terms)
                    {
                        if (programme.InformationSheet.IsChange && programme.InformationSheet.PreviousInformationSheet != null)
                        {
                            totalPremium += clientAgreementTerm.PremiumDiffer;
                        }
                        else
                        {
                            totalPremium += clientAgreementTerm.Premium;
                        }

                    }
                }
                totalPayment = Math.Round(((totalPremium + brokerFee) * (GST) * (creditCharge)), 2);

                PxPay pxPay = new PxPay(merchant.MerchantPaymentGateway.PaymentGatewayWebServiceURL, merchant.MerchantPaymentGateway.PxpayUserId, merchant.MerchantPaymentGateway.PxpayKey);

                string domainQueryString = _appSettingService.domainQueryString;

                RequestInput input = new RequestInput
                {
                    AmountInput = totalPayment.ToString("0.00"),
                    CurrencyInput = "NZD",
                    TxnType = "Purchase",
                    UrlFail = "https://" + domainQueryString + payment.PaymentPaymentGateway.PxpayUrlFail + ProgrammeId.ToString(),
                    UrlSuccess = "https://" + domainQueryString + payment.PaymentPaymentGateway.PxpayUrlSuccess + ProgrammeId.ToString(),
                    TxnId = payment.Id.ToString("N").Substring(0, 16),
                };

                RequestOutput requestOutput = pxPay.GenerateRequest(input);
                //opens on same page - hard to return back to current process
                return Json(new { url = requestOutput.Url });
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }       

        [HttpPost]
        public async Task<IActionResult> GenerateEGlobal(IFormCollection collection)
        {
            ClientInformationSheet sheet = null;
            User user = null;
            try
            {
                user = await CurrentUser();
                //throw new Exception("Method will need to be re-written");
                if (Guid.TryParse(HttpContext.Request.Form["AnswerSheetId"], out Guid sheetId))
                {
                    sheet = await _customerInformationService.GetInformation(sheetId);
                }

                //Hardcoded variables
                ClientProgramme programme = sheet.Programme;
                var eGlobalSerializer = new EGlobalSerializerAPI();

                //check Eglobal parameters
                if (string.IsNullOrEmpty(programme.EGlobalClientNumber))
                {
                    throw new Exception(nameof(programme.EGlobalClientNumber) + " EGlobal client number");
                }
                string paymentType = "Credit";
                Guid transactionreferenceid = Guid.NewGuid();

                var xmlPayload = eGlobalSerializer.SerializePolicy(programme, user, _unitOfWork, transactionreferenceid, paymentType, false, false, null);

                var byteResponse = await _httpClientService.CreateEGlobalInvoice(xmlPayload);

                EGlobalSubmission eglobalsubmission = await _eGlobalSubmissionService.GetEGlobalSubmissionByTransaction(transactionreferenceid);

                eGlobalSerializer.DeSerializeResponse(byteResponse, programme, user, _unitOfWork, eglobalsubmission);

                if (programme.ClientAgreementEGlobalResponses.Count > 0)
                {
                    EGlobalResponse eGlobalResponse = programme.ClientAgreementEGlobalResponses.Where(er => er.DateDeleted == null && er.ResponseType == "update").OrderByDescending(er => er.VersionNumber).FirstOrDefault();
                    if (eGlobalResponse != null)
                    {
                        var documents = new List<SystemDocument>();
                        foreach (ClientAgreement agreement in programme.Agreements)
                        {
                            if (agreement.MasterAgreement && (agreement.ReferenceId == eGlobalResponse.MasterAgreementReferenceID))
                            {
                                foreach (SystemDocument doc in agreement.Documents.Where(d => d.DateDeleted == null && d.DocumentType == 4))
                                {
                                    doc.Delete(user);
                                }
                                foreach (SystemDocument template in agreement.Product.Documents)
                                {
                                    //render docs invoice
                                    if (template.DocumentType == 4)
                                    {
                                        SystemDocument renderedDoc = await _fileService.RenderDocument(user, template, agreement, null);
                                        renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                                        agreement.Documents.Add(renderedDoc);
                                        documents.Add(renderedDoc);
                                        await _fileService.UploadFile(renderedDoc);
                                    }
                                }
                            }
                        }
                    }

                }

                return Redirect("~/Agreement/ViewAcceptedAgreement/" + programme.Id.ToString());
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> ProcessRequestConfiguration(Guid Id)
        {
            User user = null;
            try
            {
                string queryString = HttpContext.Request.Query["result"].ToString();
                var status = "Bound";
                user = await CurrentUser();

                ClientProgramme programme = await _programmeService.GetClientProgrammebyId(Id);
                Payment payment = await _paymentService.GetPayment(programme.Id);


                PxPay pxPay = new PxPay(payment.PaymentMerchant.MerchantPaymentGateway.PaymentGatewayWebServiceURL, payment.PaymentMerchant.MerchantPaymentGateway.PxpayUserId, payment.PaymentMerchant.MerchantPaymentGateway.PxpayKey);
                ResponseOutput responseOutput = pxPay.ProcessResponse(queryString.ToString());

                payment.PaymentAttempts += 1;
                payment.CreditCardType = responseOutput.CardName;
                payment.CreditCardNumber = responseOutput.CardNumber;
                payment.IsPaid = responseOutput.Success == "1" ? true : false;
                payment.PaymentAmount = Convert.ToDecimal(responseOutput.AmountSettlement);
                payment.PaymentCurrency = "NZD";
                await _paymentService.Update(payment);

                if (!payment.IsPaid)
                {
                    //Payment failed
                    status = "Bound and pending payment";
                    //_emailService.SendSystemPaymentFailConfigEmailUISIssueNotify(programme.BrokerContactUser, programme.BaseProgramme, programme.InformationSheet, programme.Owner);
                    foreach (ClientAgreement agreement in programme.Agreements)
                    {
                        using (var uow = _unitOfWork.BeginUnitOfWork())
                        {
                            if (agreement.Status != status)
                            {
                                agreement.Status = status;
                                await uow.Commit();
                            }
                        }

                        agreement.Status = status;

                    }

                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (programme.InformationSheet.Status != status)

                        {
                            programme.InformationSheet.Status = status;
                            await uow.Commit();
                        }
                    }
                    
                    return RedirectToAction("ProcessedAgreements", new { id = Id });

                }
                else
                {
                    //Payment successed
                    //await _emailService.SendSystemPaymentSuccessConfigEmailUISIssueNotify(programme.BrokerContactUser, programme.BaseProgramme, programme.InformationSheet, programme.Owner);

                    //bool hasEglobalNo = programme.EGlobalClientNumber != null ? true : false;
                    status = "Bound and invoice pending";
                    bool hasEglobalNo = false;
                    if (programme.EGlobalClientNumber != null)
                    {
                        hasEglobalNo = true;
                    }

                    bool eglobalsuccess = false;

                    var documents = new List<SystemDocument>();
                    foreach (ClientAgreement agreement in programme.Agreements)
                    {
                        using (var uow = _unitOfWork.BeginUnitOfWork())
                        {
                            if (agreement.Status != status)
                            {
                                agreement.Status = status;
                                if (programme.BaseProgramme.PolicyNumberPrefixString != null)
                                {
                                    agreement.PolicyNumber = programme.BaseProgramme.PolicyNumberPrefixString + "-0" + agreement.ReferenceId;
                                }
                                await uow.Commit();
                            }
                        }

                        agreement.Status = status;

                        if (hasEglobalNo)
                        {

                            var eGlobalSerializer = new EGlobalSerializerAPI();

                            string paymentType = "Credit";
                            Guid transactionreferenceid = Guid.NewGuid();

                            var xmlPayload = eGlobalSerializer.SerializePolicy(programme, user, _unitOfWork, transactionreferenceid, paymentType, false, false, null);

                            var byteResponse = await _httpClientService.CreateEGlobalInvoice(xmlPayload);

                            EGlobalSubmission eglobalsubmission = await _eGlobalSubmissionService.GetEGlobalSubmissionByTransaction(transactionreferenceid);

                            eGlobalSerializer.DeSerializeResponse(byteResponse, programme, user, _unitOfWork, eglobalsubmission);


                            if (programme.ClientAgreementEGlobalResponses.Count > 0)
                            {
                                EGlobalResponse eGlobalResponse = programme.ClientAgreementEGlobalResponses.Where(er => er.DateDeleted == null && er.ResponseType == "update").OrderByDescending(er => er.VersionNumber).FirstOrDefault();
                                if (eGlobalResponse != null)
                                {
                                    status = "Bound and invoiced";
                                    eglobalsuccess = true;
                                    agreement.Status = status;
                                }
                            }

                            //await _emailService.SendSystemSuccessInvoiceConfigEmailUISIssueNotify(programme.BrokerContactUser, programme.BaseProgramme, programme.InformationSheet, programme.Owner);
                        }
                        else
                        {
                            //_emailService.SendSystemFailedInvoiceConfigEmailUISIssueNotify(programme.BrokerContactUser, programme.BaseProgramme, programme.InformationSheet, programme.Owner);
                        }

                        var agreeDocList = agreement.GetDocuments();
                        foreach (SystemDocument doc in agreeDocList)
                        {
                            doc.Delete(user);
                        }
                        foreach (SystemDocument template in agreement.Product.Documents)
                        {
                            if (!eglobalsuccess)
                            {
                                //render docs except invoice
                                if (template.DocumentType != 4)
                                {
                                    SystemDocument renderedDoc = await _fileService.RenderDocument(user, template, agreement, null);
                                    renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                                    agreement.Documents.Add(renderedDoc);
                                    documents.Add(renderedDoc);
                                    await _fileService.UploadFile(renderedDoc);
                                }
                            }
                            else
                            {
                                SystemDocument renderedDoc = await _fileService.RenderDocument(user, template, agreement, null);
                                renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                                agreement.Documents.Add(renderedDoc);
                                documents.Add(renderedDoc);
                                await _fileService.UploadFile(renderedDoc);
                            }

                        }

                        EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == "SendPolicyDocuments");

                        if (emailTemplate == null)
                        {
                            //default email or send them somewhere??

                            using (var uow = _unitOfWork.BeginUnitOfWork())
                            {
                                emailTemplate = new EmailTemplate(user, "Agreement Documents Covering Text", "SendPolicyDocuments", "Policy Documents for ", WebUtility.HtmlDecode("Email Containing policy documents"), null, programme.BaseProgramme);
                                programme.BaseProgramme.EmailTemplates.Add(emailTemplate);
                                await uow.Commit();
                            }
                        }

                        if (programme.BaseProgramme.ProgEnableEmail)
                        {
                            await _emailService.SendEmailViaEmailTemplate(programme.BrokerContactUser.Email, emailTemplate, documents, null, null);
                            await _emailService.SendSystemEmailAgreementBoundNotify(programme.BrokerContactUser, programme.BaseProgramme, agreement, programme.Owner);
                        }
                    }

                    

                    using (var uow = _unitOfWork.BeginUnitOfWork())
                    {
                        if (programme.InformationSheet.Status != status)
                        {
                            programme.InformationSheet.Status = status;
                            await uow.Commit();
                        }
                    }
                }
                return RedirectToAction("ProcessedAgreements", new { id = Id });
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProcessedAgreements(Guid id)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                PartialViewResult result = (PartialViewResult)await ViewAgreement(id);
                var models = (BaseListViewModel<ViewAgreementViewModel>)result.Model;
                var agreeDocList = new List<Document>();
                foreach (ViewAgreementViewModel model in models)
                {
                    model.EditEnabled = false;
                    model.Documents = new List<AgreementDocumentViewModel>();

                    ClientProgramme programme = await _programmeService.GetClientProgrammebyId(id);
                    model.InformationSheetId = programme.InformationSheet.Id;
                    model.ClientProgrammeId = id;
                    foreach (ClientAgreement agreement in programme.Agreements)
                    {
                        model.ClientAgreementId = agreement.Id;
                        agreeDocList = agreement.GetDocuments();
                        foreach (Document doc in agreeDocList)
                        {
                            model.Documents.Add(new AgreementDocumentViewModel { DisplayName = doc.Name, Url = "/File/GetDocument/" + doc.Id });
                        }
                    }
                }
                return View("ViewProccessedAgreementList", models);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewAcceptedAgreement(Guid id)
        {
            User user = null;

            try
            {
                PartialViewResult result = (PartialViewResult)await ViewAgreement(id);
                var models = (BaseListViewModel<ViewAgreementViewModel>)result.Model;
                user = await CurrentUser();
                var agreeDocList = new List<Document>();

                foreach (ViewAgreementViewModel model in models)
                {
                    model.EditEnabled = false;
                    model.Documents = new List<AgreementDocumentViewModel>();
                    model.CurrentUser = user;

                    ClientProgramme programme = await _programmeService.GetClientProgrammebyId(id);
                    model.ClientInformationSheet = programme.InformationSheet;
                    model.InformationSheetId = programme.InformationSheet.Id;
                    model.ClientProgrammeId = id;                    
                    foreach (ClientAgreement agreement in programme.Agreements.Where(a=>a.DateDeleted == null))
                    {
                        agreeDocList = agreement.GetDocuments();
                        foreach (Document doc in agreeDocList)
                        {
                            model.Documents.Add(new AgreementDocumentViewModel { DisplayName = doc.Name, Url = "/File/GetDocument/" + doc.Id , ClientAgreementId = agreement.Id });
                        }
                    }
                }
                ViewBag.Id = id;

                return View("ViewAcceptedAgreementList", models);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        public async Task<IActionResult> CreateDefaultAgreementRules()
        {
            User user = null;
            try
            {
                var productList = await _productService.GetAllProducts();
                Product product = productList.FirstOrDefault(p => p.IsBaseProduct == false);
                user = await CurrentUser();

                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    if (product != null)
                    {
                        //
                        await _ruleRepository.AddAsync(new Rule(user, "NICITY1CRate", "North Island City Rate for Category 1C", product, "2.75") { OrderNumber = 5 });
                        await _ruleRepository.AddAsync(new Rule(user, "NITOWN1CRate", "North Island Town Rate for Category 1C", product, "2.5") { OrderNumber = 6 });
                        await _ruleRepository.AddAsync(new Rule(user, "SICITY1CRate", "South Island City Rate for Category 1C", product, "2.225") { OrderNumber = 7 });
                        await _ruleRepository.AddAsync(new Rule(user, "SITOWN1CRate", "South Island Town Rate for Category 1C", product, "2") { OrderNumber = 8 });
                        ///
                        await _ruleRepository.AddAsync(new Rule(user, "NICITY1UAPRate", "North Island City Rate for Category 1UAP", product, "4") { OrderNumber = 9 });
                        await _ruleRepository.AddAsync(new Rule(user, "NITOWN1UAPRate", "North Island Town Rate for Category 1UAP", product, "4") { OrderNumber = 10 });
                        await _ruleRepository.AddAsync(new Rule(user, "SICITY1UAPRate", "South Island City Rate for Category 1UAP", product, "4") { OrderNumber = 11 });
                        await _ruleRepository.AddAsync(new Rule(user, "SITOWN1UAPRate", "South Island Town Rate for Category 1UAP", product, "4") { OrderNumber = 12 });
                        ///
                        await _ruleRepository.AddAsync(new Rule(user, "NICITY1PRate", "North Island City Rate for Category 1P", product, "2") { OrderNumber = 13 });
                        await _ruleRepository.AddAsync(new Rule(user, "NITOWN1PRate", "North Island Town Rate for Category 1P", product, "1.5") { OrderNumber = 14 });
                        await _ruleRepository.AddAsync(new Rule(user, "SICITY1PRate", "South Island City Rate for Category 1P", product, "1.5") { OrderNumber = 15 });
                        await _ruleRepository.AddAsync(new Rule(user, "SITOWN1PRate", "South Island Town Rate for Category 1P", product, "1") { OrderNumber = 16 });
                        ///
                        await _ruleRepository.AddAsync(new Rule(user, "NICITY1RRate", "North Island City Rate for Category 1R", product, "4.75") { OrderNumber = 17 });
                        await _ruleRepository.AddAsync(new Rule(user, "NITOWN1RRate", "North Island Town Rate for Category 1R", product, "4.75") { OrderNumber = 18 });
                        await _ruleRepository.AddAsync(new Rule(user, "SICITY1RRate", "South Island City Rate for Category 1R", product, "4.75") { OrderNumber = 19 });
                        await _ruleRepository.AddAsync(new Rule(user, "SITOWN1RRate", "South Island Town Rate for Category 1R", product, "4.75") { OrderNumber = 20 });
                        ///
                        await _ruleRepository.AddAsync(new Rule(user, "NICITY2Rate", "North Island City Rate for Category 2", product, "1.5") { OrderNumber = 21 });
                        await _ruleRepository.AddAsync(new Rule(user, "NITOWN2Rate", "North Island Town Rate for Category 2", product, "1.25") { OrderNumber = 22 });
                        await _ruleRepository.AddAsync(new Rule(user, "SICITY2Rate", "South Island City Rate for Category 2", product, "1.25") { OrderNumber = 23 });
                        await _ruleRepository.AddAsync(new Rule(user, "SITOWN2Rate", "South Island Town Rate for Category 2", product, "1") { OrderNumber = 24 });
                        ///
                        await _ruleRepository.AddAsync(new Rule(user, "NICITY3Rate", "North Island City Rate for Category 3", product, "1.75") { OrderNumber = 25 });
                        await _ruleRepository.AddAsync(new Rule(user, "NITOWN3Rate", "North Island Town Rate for Category 3", product, "1.25") { OrderNumber = 26 });
                        await _ruleRepository.AddAsync(new Rule(user, "SICITY3Rate", "South Island City Rate for Category 3", product, "1.25") { OrderNumber = 27 });
                        await _ruleRepository.AddAsync(new Rule(user, "SITOWN3Rate", "South Island Town Rate for Category 3", product, "1") { OrderNumber = 28 });
                        ///
                        await _ruleRepository.AddAsync(new Rule(user, "NICITYSVRate", "North Island City Rate for Category SV", product, "0.25") { OrderNumber = 29 });
                        await _ruleRepository.AddAsync(new Rule(user, "NITOWNSVRate", "North Island Town Rate for Category SV", product, "0.25") { OrderNumber = 30 });
                        await _ruleRepository.AddAsync(new Rule(user, "SICITYSVRate", "South Island City Rate for Category SV", product, "0.25") { OrderNumber = 31 });
                        await _ruleRepository.AddAsync(new Rule(user, "SITOWNSVRate", "South Island Town Rate for Category SV", product, "0.25") { OrderNumber = 32 });
                        ///
                        await _ruleRepository.AddAsync(new Rule(user, "FSLUNDERFee", "FSL Fee for Vehicle under 3.5T", product, "6.08") { OrderNumber = 33 });
                        await _ruleRepository.AddAsync(new Rule(user, "FSLOVER3Rate", "FSL Rate for Vehicle over 3.5T", product, "0.076") { OrderNumber = 34 });
                        await _ruleRepository.AddAsync(new Rule(user, "FSLUNDERFeeAfter1July", "FSL Fee for Vehicle under 3.5T After 1 July", product, "6.08") { OrderNumber = 35 });
                        await _ruleRepository.AddAsync(new Rule(user, "FSLOVER3RateAfter1July", "FSL Rate for Vehicle over 3.5T After 1 July", product, "0.076") { OrderNumber = 36 });
                        await _ruleRepository.AddAsync(new Rule(user, "PaymentPremium", "Premium Payment", product, "Monthly") { OrderNumber = 40 });

                        await uow.Commit();
                    }
                }

                return Redirect("~/Home/Index");
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        public async Task<IActionResult> RenderDocuments(Guid id)
        {
            User user = null;
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentNullException(nameof(id));
                ClientInformationSheet answerSheet = await _customerInformationService.GetInformation(id);
                if (answerSheet == null)
                    throw new Exception(string.Format("RenderDocuments: No Answer Sheet found for [{0}]", id));
                ClientProgramme clientProgramme = answerSheet.Programme;
                if (clientProgramme == null)
                    throw new Exception(string.Format("RenderDocuments: No Client Programme found for information sheet [{0}]", id));
                //ClientAgreement agreement = answerSheet.ClientAgreement;
                //if (agreement == null)
                //	throw new Exception (string.Format ("No Information found for {0}", id));
                user = await CurrentUser();
                var agreeDocList = new List<Document>();
                Document renderedDoc;
                foreach (ClientAgreement agreement in clientProgramme.Agreements)
                {
                    agreeDocList = agreement.GetDocuments();
                    foreach (Document doc in agreeDocList)
                    {
                        doc.Delete(user);
                    }

                    foreach (Document template in agreement.Product.Documents)
                    {
                        if (template.DocumentType == 6)
                        {
                            foreach (var subsheet in agreement.ClientInformationSheet.SubClientInformationSheets)
                            {
                                renderedDoc = await _fileService.RenderDocument(user, template, agreement, subsheet);
                                renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                                agreement.Documents.Add(renderedDoc);
                                await _fileService.UploadFile(renderedDoc);
                            }
                        }
                        else
                        {
                            renderedDoc = await _fileService.RenderDocument(user, template, agreement, null);
                            renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                            agreement.Documents.Add(renderedDoc);
                            await _fileService.UploadFile(renderedDoc);
                        }
                    }
                }

                return Redirect("/Agreement/ViewAcceptedAgreement/" + clientProgramme.Id);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        private string _MPCourAttendanceEndor = @"<p><strong>Court Attendance</strong></p>
<p></p>
<p>[[InsurerCompanyShort]] will indemnify the Insured where the Insured attends Court as a witness in connection with a Valid Claim Covered by this Policy.</p>
<p>include the following rates per day for each and every day on which Your attendance in court has been required:</p>
<p>(a) for any of Your principals, partners, or directors $250 per day</p>
<p>(b) for any of Your employees or contractors $100 per day</p>
<p>Provided that the maximum [[InsurerCompanyShort]] will pay is $100,000 in the aggregate in respect of all Claims by all Insured members of the Wellness and Health Associated Professionals Insurance facility.</p>";

        private string _MPSplitLimitofIndemnityEndor = @"<p><strong>Split Limit of Indemnity</strong></p>
<p></p>
<p>Notwithstanding anything contained in the policy Schedule and wording to the contrary it is agreed that;</p>
<p>1.  The Limit of Indemnity is divided according to the following definitions:</p>
<p>Defence Costs Limit of Indemnity:         $250,000</p>
<p>Loss Limit of Indemnity:                           $250,000</p>
<p>Policy Limit of Indemnity:                       $500,000</p>
<p>2.  The following Definitions are included within the Policy wording:</p>
<p>Defence Costs Limit of Indemnity</p>
<p>'Defence Costs Limit of Indemnity' means [[InsurerCompanyShort]]’s maximum limit of liability for payment of all Defence Costs incurred by or on behalf on an Insured as specified in this Endorsement.</p>
<p>Loss Limit of Indemnity</p>
<p>'Loss Limit of Indemnity' means [[InsurerCompanyShort]]'s maximum limit of liability for payment of all amounts (excluding Defence Costs) incurred by or on behalf of an Insured as specified in this Endorsement.</p>
<p>Policy Limit of Indemnity</p>
<p>'Policy Limit of Indemnity' means the combined total of the Defence Costs Limit of Indemnity and the Loss Limit of Indemnity as specified in the Schedule as the Policy Limit of Indemnity.</p>
<p>3.  If any Claim is subject to a sub-limit which is less than the Policy Limit of Indemnity, then the Defence Costs Limit of Indemnity and the Loss Limit of Indemnity shall be divided to the extent of the sub-limit in the same proportions as they bear to the Policy Limit of Indemnity.</p>
<p>Nothing in this Endorsement shall be held to vary, alter, waive or extend any of the terms, conditions, provisions, agreements or limitations of the Policy other than as stated in this Endorsement.</p>";

        private string _GLBusinessAdviceorServiceExclusion2Endor = @"<p><strong>Business Advice or Service Exclusion 2</strong></p>
<p></p>
<p>It is agreed that the Automatic Coverage Clause 3.1,  Business Advice or Service is deleted.</p>
<p>Nothing herein contained shall be held to vary, alter, waive or extend any of the terms, conditions, provisions, agreements or limitations of the above mentioned Policy other than as above stated.</p>";

        private string _SLAmendmentstoDefinitionEndor = @"<p><strong>Amendments to Definition 1.1</strong></p>
<p></p>
<p>Notwithstanding anything in the Policy to the contrary it is agreed that the Policy is amended as follows.</p>
<p>Definition 1.1 Act of Parliament is deleted and replaced by the following.</p>
<p>1.1  Act of Parliament</p>
<p>'Act of Parliament' means any Act of the New Zealand Parliament, including any amendment, enactment, re-enactment or replacement legislation or any Code, Rules or Regulations under such Act;</p>
<p>In all other respects this Policy remains unaltered other than as stated above.</p>";

        private string _SLExclusionEndor = @"<p><strong>Exclusion 4.7</strong></p>
<p></p>
<p>The Land Transport Act 1998 is added to Exclusion 4.7 as an Excluded Act.</p>
<p>In all other respects this Policy remains unaltered other than as stated above.</p>";

        private string _SLSplitLimitofIndemnityEndor = @"<p><strong>Split Limit of Indemnity</strong></p>
<p></p>
<p>Notwithstanding anything contained in the policy Schedule and wording to the contrary it is agreed that;</p>
<p>1.  The Limit of Indemnity is divided according to the following definitions:</p>
<p>Defence Costs Limit of Indemnity:         $250,000</p>
<p>Loss Limit of Indemnity:                           $250,000</p>
<p>Policy Limit of Indemnity:                       $500,000</p>
<p>2.  The following Definitions are included within the Policy wording:</p>
<p>Defence Costs Limit of Indemnity</p>
<p>'Defence Costs Limit of Indemnity' means [[InsurerCompanyShort]]’s maximum limit of liability for payment of all Defence Costs incurred by or on behalf on an Insured as specified in this Endorsement.</p>
<p>Loss Limit of Indemnity</p>
<p>'Loss Limit of Indemnity' means [[InsurerCompanyShort]]'s maximum limit of liability for payment of all amounts (excluding Defence Costs) incurred by or on behalf of an Insured as specified in this Endorsement.</p>
<p>Policy Limit of Indemnity</p>
<p>'Policy Limit of Indemnity' means the combined total of the Defence Costs Limit of Indemnity and the Loss Limit of Indemnity as specified in the Schedule as the Policy Limit of Indemnity.</p>
<p>3.  If any Claim is subject to a sub-limit which is less than the Policy Limit of Indemnity, then the Defence Costs Limit of Indemnity and the Loss Limit of Indemnity shall be divided to the extent of the sub-limit in the same proportions as they bear to the Policy Limit of Indemnity.</p>
<p>Nothing in this Endorsement shall be held to vary, alter, waive or extend any of the terms, conditions, provisions, agreements or limitations of the Policy other than as stated in this Endorsement.</p>";

    }
}
