﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;
using SystemDocument = TechCertain.Domain.Entities.Document;
using Elmah;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechCertain.WebUI.Models.Agreement;
using TechCertain.WebUI.Models;
using TechCertain.WebUI.Helpers;
using TechCertain.Infrastructure.Payment.PxpayAPI;
using Microsoft.AspNetCore.Http;
using System.Net;
using TechCertain.WebUI.Areas.Identity.Data;

namespace TechCertain.WebUI.Controllers
{
    //[Authorize]
    public class AgreementController : BaseController
    {
        IInformationTemplateService _informationService;
        ICilentInformationService _customerInformationService;
        IPaymentGatewayService _paymentGatewayService;
        IPaymentService _paymentService;
        IMerchantService _merchantService;
        IClientAgreementTermService _clientAgreementTermService;

        IRepository<Product> _productRepository;
        IRepository<Rule> _ruleRepository;
        IRepository<User> _userRepository1;

        IClientAgreementService _clientAgreementService;
        IClientAgreementRuleService _clientAgreementRuleService;
        IClientAgreementEndorsementService _clientAgreementEndorsementService;
        IFileService _fileService;
        IEmailService _emailService;
        IRepository<Organisation> _OrganisationRepository;
        ILogger _logger;
        IRepository<SystemDocument> _documentRepository;
        IOrganisationService _organisationService;

        IRepository<ClientProgramme> _programmeRepository;
        IInsuranceAttributeService _insuranceAttributeService;


        private IUnitOfWorkFactory _unitOfWorkFactory;


        public AgreementController(IUserService userRepository, DealEngineDBContext dealEngineDBContext, IInformationTemplateService informationService, ICilentInformationService customerInformationService,
                                   IRepository<Product> productRepository, IClientAgreementService clientAgreementService, IClientAgreementRuleService clientAgreementRuleService,
                                   IClientAgreementEndorsementService clientAgreementEndorsementService, IFileService fileService, IUnitOfWorkFactory unitOfWorkFactory,
                                   IOrganisationService organisationService, IRepository<Organisation> OrganisationRepository, IRepository<Rule> ruleRepository, IEmailService emailService, ILogger logger, IRepository<SystemDocument> documentRepository, IRepository<User> userRepository1,
                                   IRepository<ClientProgramme> programmeRepository, IPaymentGatewayService paymentGatewayService, IInsuranceAttributeService insuranceAttributeService, IPaymentService paymentService, IMerchantService merchantService, IClientAgreementTermService clientAgreementTermService)
            : base(userRepository, dealEngineDBContext)
        {
            _informationService = informationService;
            _customerInformationService = customerInformationService;
            _organisationService = organisationService;

            _productRepository = productRepository;
            _clientAgreementService = clientAgreementService;
            _clientAgreementRuleService = clientAgreementRuleService;
            _clientAgreementEndorsementService = clientAgreementEndorsementService;
            _fileService = fileService;
            _emailService = emailService;
            _logger = logger;
            _unitOfWorkFactory = unitOfWorkFactory;
            // temp
            _ruleRepository = ruleRepository;
            _documentRepository = documentRepository;
            _userRepository1 = userRepository1;
            _paymentGatewayService = paymentGatewayService;
            _paymentService = paymentService;
            _merchantService = merchantService;
            _clientAgreementTermService = clientAgreementTermService;
            _insuranceAttributeService = insuranceAttributeService;
            _OrganisationRepository = OrganisationRepository;
            _programmeRepository = programmeRepository;

            ViewBag.Title = "Wellness and Health Associated Professionals Agreement";
        }

        [HttpGet]
        public ActionResult MyAgreements()
        {
            MyAgreementsViewModel model = new MyAgreementsViewModel();
            model.MyAgreements = new List<AgreementViewModel>();

            // TODO - fix this to use ClientProgramme
            //foreach (var answerSheet in _customerInformationService.GetAllInformationFor (CurrentUser)) {
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

        public ActionResult AgreementTemplates()
        {
            return View();
        }

        public ActionResult AgreementBuilder()
        {
            var templates = _informationService.GetAllTemplates();
            var products = _productRepository.FindAll();


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

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AuthoriseReferrals(Guid sheetId,Guid informationsheet, Guid agreementId)
        {
            ViewAgreementViewModel model = new ViewAgreementViewModel();
            ClientAgreement agreement = _clientAgreementService.GetAgreement(agreementId);
            //  ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV" && t.DateDeleted == null);
            //  model.ClientAgreementId = id;
            model.InformationSheetId = sheetId;
            model.ClientAgreementId = agreementId;

            model.Referrals = new List<ClientAgreementReferral>();
            foreach (var terms in agreement.ClientAgreementReferrals)
            {
                model.Referrals.Add(terms);
            }

            model.ReferralLoading = agreement.ClientAgreementTerms.FirstOrDefault().ReferralLoading;
            model.ReferralAmount = agreement.ClientAgreementTerms.FirstOrDefault().ReferralLoadingAmount;
            model.AuthorisationNotes = agreement.ClientAgreementTerms.FirstOrDefault().AuthorisationNotes;

            ViewBag.Title = "Agreement Refferals ";

            return View("AuthoriseReferrals", model);
        }

        [HttpPost]
        public ActionResult AuthorisedReferral(AgreementViewModel clientAgreementModel)
        {
            ClientAgreement agreement = _clientAgreementService.GetAgreement(clientAgreementModel.AgreementId);

            var premium = 0.0m;
            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                foreach (var terms in agreement.ClientAgreementReferrals.Where(r => r.Status == "Pending"))
                {
                    terms.Status = "Cleared";
                }

                foreach(var terms in agreement.ClientAgreementTerms)
                {
                    foreach (var bvterm in terms.BoatTerms)
                    {
                        premium = premium + bvterm.Premium;
                    }
                    foreach (var mvterm in terms.MotorTerms)
                    {
                        premium = premium + mvterm.Premium;
                    }
                }

                foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms)
                {
                    term.ReferralLoading = clientAgreementModel.RefferLodPrc;
                    term.ReferralLoadingAmount = clientAgreementModel.RefferLodAmt;
                    term.AuthorisationNotes = clientAgreementModel.AdditionalNotes;
                    term.Premium = premium * (1 + clientAgreementModel.RefferLodPrc/100) + clientAgreementModel.RefferLodAmt;
                }

                foreach (var terms in agreement.ClientAgreementTerms)
                {
                    foreach (var bvterm in terms.BoatTerms)
                    {
                        
                        if (bvterm.Boat.BoatWaterLocation != null)
                        {

                           InsuranceAttribute insuranceAttribute = _insuranceAttributeService.GetInsuranceAttributeByName("Other Marina");

                            var orgList = _organisationService.GetAllOrganisations().Where(o => o.IsApproved==false && o.InsuranceAttributes.Contains(insuranceAttribute) ).ToList();
                            foreach (var org in orgList)
                            {
                                InsuranceAttribute insuranceAttribute1 = _insuranceAttributeService.GetInsuranceAttributeByName(org.Name);
                                if(insuranceAttribute.InsuranceAttributeName == "Other Marina")
                                {
                                     
                                        org.IsApproved = true;
                                }
                            }
                            Organisation othermarine = _OrganisationRepository.GetById(bvterm.Boat.BoatWaterLocation.Id);
                        }
                        
                    }
                }

                if (agreement.Status != "Quoted")
                    agreement.Status = "Quoted";

                string auditLogDetail = "Agreement Referrals have been authorised by " + CurrentUser.FullName;
                AuditLog auditLog = new AuditLog(CurrentUser, agreement.ClientInformationSheet, agreement, auditLogDetail);
                agreement.ClientAgreementAuditLogs.Add(auditLog);

                uow.Commit();

            }

            var url = "/Agreement/ViewAcceptedAgreement/" + agreement.ClientInformationSheet.Programme.Id;
            // < a onclick = "location.href='@Url.Action(" / ViewAcceptedAgreement", "Agreement",new { id =  Model.ProgramId})'" style = "cursor:pointer" align = "center" >
            return Json(new { Url =url });
            // return RedirectToAction("ViewAcceptedAgreement", new { id = clientAgreementModel.InformationId });
        }

        [HttpGet]
        public ActionResult CancellAgreement(Guid id)
        {
            ViewAgreementViewModel model = new ViewAgreementViewModel();

            ClientAgreement agreement = _clientAgreementService.GetAgreement(id);
            ClientInformationSheet answerSheet = agreement.ClientInformationSheet;
            Organisation insured = answerSheet.Owner;
            ClientProgramme programme = answerSheet.Programme;

            model.InformationSheetId = answerSheet.Id;
            model.ClientAgreementId = agreement.Id;

            model.CancellNotes = agreement.CancelledNote;
            model.CancellEffectiveDate = agreement.CancelledEffectiveDate;

            ViewBag.Title = "Cancel Agreement ";

            return View("CancellAgreement", model);
        }

        [HttpPost]
        public ActionResult CancellAgreement(AgreementViewModel clientAgreementModel)
        {
            ClientAgreement agreement = _clientAgreementService.GetAgreement(clientAgreementModel.AgreementId);

            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                if ((agreement.Status != "Declined by Insurer" || agreement.Status != "Declined by Insured" || agreement.Status != "Cancelled") &&
                    (agreement.Status == "Bound" || agreement.Status == "Bound and invoice pending" || agreement.Status == "Bound and invoiced"))

                    agreement.Status = "Cancelled";
                agreement.CancelledNote = clientAgreementModel.CancellNotes;
                agreement.CancelledEffectiveDate = clientAgreementModel.CancellEffectiveDate;
                agreement.Cancelled = true;
                agreement.CancelledByUserID = CurrentUser;
                agreement.CancelledDate = DateTime.UtcNow;


                string auditLogDetail = "Agreement has been cancelled by " + CurrentUser.FullName;
                AuditLog auditLog = new AuditLog(CurrentUser, agreement.ClientInformationSheet, agreement, auditLogDetail);
                agreement.ClientAgreementAuditLogs.Add(auditLog);

                uow.Commit();

            }

            var url = "/Agreement/ViewAcceptedAgreement/" + agreement.ClientInformationSheet.Programme.Id;
            return Json(new { Url = url });
        }

        [HttpGet]
        public ActionResult DeclineAgreement(Guid id)
        {
            ViewAgreementViewModel model = new ViewAgreementViewModel();

            ClientAgreement agreement = _clientAgreementService.GetAgreement(id);
            ClientInformationSheet answerSheet = agreement.ClientInformationSheet;
            Organisation insured = answerSheet.Owner;
            ClientProgramme programme = answerSheet.Programme;

            model.InformationSheetId = answerSheet.Id;
            model.ClientAgreementId = agreement.Id;

            model.DeclineNotes = agreement.InsurerDeclinedComment;

            ViewBag.Title = "Decline Agreement ";

            return View("DeclineAgreement", model);
        }

        [HttpPost]
        public ActionResult DeclineAgreement(AgreementViewModel clientAgreementModel)
        {
            ClientAgreement agreement = _clientAgreementService.GetAgreement(clientAgreementModel.AgreementId);

            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                if (agreement.Status != "Declined by Insurer" || agreement.Status != "Declined by Insured" || agreement.Status != "Cancelled" ||
                    agreement.Status != "Bound" || agreement.Status != "Bound and invoice pending" || agreement.Status != "Bound and invoiced")

                    agreement.Status = "Declined by Insurer";
                agreement.InsurerDeclinedComment = clientAgreementModel.DeclineNotes;
                agreement.InsurerDeclined = true;
                agreement.InsurerDeclinedUserID = CurrentUser;
                agreement.InsurerDeclinedDate = DateTime.UtcNow;


                string auditLogDetail = "Agreement has been declined by " + CurrentUser.FullName;
                AuditLog auditLog = new AuditLog(CurrentUser, agreement.ClientInformationSheet, agreement, auditLogDetail);
                agreement.ClientAgreementAuditLogs.Add(auditLog);

                uow.Commit();

            }

            var url = "/Agreement/ViewAcceptedAgreement/" + agreement.ClientInformationSheet.Programme.Id;
            return Json(new { Url = url });
        }


        [HttpGet]
        public ActionResult UndeclineAgreement(Guid id)
        {
            ClientAgreement agreement = _clientAgreementService.GetAgreement(id);

            if (agreement != null)
            {
                using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
                {
                    if (agreement.Status == "Declined by Insurer" || agreement.Status == "Declined by Insured")
                    {
                        agreement.Status = "Quoted";
                        agreement.InsurerDeclined = false;
                        agreement.InsuredDeclined = false;
                        agreement.UndeclinedUserID = CurrentUser;
                        agreement.UndeclinedDate = DateTime.UtcNow;
                    }

                    string auditLogDetail = "Agreement has been undeclined by " + CurrentUser.FullName;
                    AuditLog auditLog = new AuditLog(CurrentUser, agreement.ClientInformationSheet, agreement, auditLogDetail);
                    agreement.ClientAgreementAuditLogs.Add(auditLog);

                    uow.Commit();

                }
            }

            return Redirect("/Agreement/ViewAcceptedAgreement/" + agreement.ClientInformationSheet.Programme.Id);
        }



        //[HttpPost]
        //public ActionResult AuthorisedReferrals(Guid clientAgreementId, Guid sheetId, string type)
        //{
        //    ClientAgreement agreement = _clientAgreementService.GetAgreement(clientAgreementId);

        //    using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
        //    {
        //        foreach (var terms in agreement.ClientAgreementReferrals.Where(r => r.Status == "Pending"))
        //        {
        //            terms.Status = "Cleared";
        //        }

        //        uow.Commit();
        //    }
        //    //var url = "/Information/EditInformation/" + programmeId;

        //    return RedirectToAction("SendReferralsEmail", new { ClientAgreementId = clientAgreementId, SheetId= sheetId , Type = type });
        //}

        //[HttpGet]
        //public ActionResult SendReferralsEmail(Guid ClientAgreementId, Guid SheetId, string Type)
        //{
        //    ClientProgramme clientProgramme = _programmeRepository.GetById(SheetId);
        //    Organisation insured = clientProgramme.Owner;
        //    ClientInformationSheet answerSheet = clientProgramme.InformationSheet;

        //    // TODO - rewrite to save templates on a per programme basis

        //    ClientProgramme programme = answerSheet.Programme;
        //  ClientAgreement agreement = _clientAgreementService.GetAgreement(ClientAgreementId);

        //    //ClientAgreement agreement = _clientAgreementService.GetAgreement(sheetId);

        //    using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
        //    {
        //        foreach (var terms in agreement.ClientAgreementReferrals.Where(r => r.Status == "Pending"))
        //        {
        //            terms.Status = "Cleared";
        //        }

        //        uow.Commit();
        //    }
        //    //Organisation insured = programme.Owner;

        //    //EmailTemplate emailTemplate = agreement.Product.EmailTemplates.FirstOrDefault (et => et.Type == "SendPolicyDocuments");
        //    EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == Type);

        //    EmailTemplateViewModel model = new EmailTemplateViewModel();
        //    model.Type = Type;
        //    if (emailTemplate != null)
        //    {
        //        model.Name = emailTemplate.Name;
        //        model.Subject = emailTemplate.Subject;
        //        model.Body = System.Net.WebUtility.HtmlDecode(emailTemplate.Body);
        //    }
        //    else
        //    {
        //        model.Name = "";
        //        model.Subject = "";
        //        model.Body = "";
        //    }

        //    model.ClientProgrammeID = programme.Id;

        //    if (programme.Owner != null)
        //    {
        //        var recipents = new List<UserViewModel>();

        //        recipents.Add(new UserViewModel { ID = CurrentUser.Id, UserName = CurrentUser.UserName, FirstName = CurrentUser.FirstName, LastName = CurrentUser.LastName, FullName = CurrentUser.FullName, Email = CurrentUser.Email });

        //        foreach (User recipent in _userRepository1.FindAll().Where(ur1 => ur1.Organisations.Contains(programme.Owner)))
        //        {
        //            recipents.Add(new UserViewModel { ID = recipent.Id, UserName = recipent.UserName, FirstName = recipent.FirstName, LastName = recipent.LastName, FullName = recipent.FullName, Email = recipent.Email });
        //        }

        //        model.Recipents = recipents;
        //    }
        //    else
        //    {
        //        model.Recipents = null;
        //    }

        //    ViewBag.Title = programme.BaseProgramme.Name + " Referrals";

        //    return View("SendReferralsEmail", model);
        //}


        [HttpPost]
        public ActionResult SendReferredEmail(EmailTemplateViewModel model)
        {
            ClientProgramme programme = _programmeRepository.GetById(model.ClientProgrammeID);

            // TODO - rewrite to save templates on a per programme basis
            ClientAgreement agreement = programme.Agreements[0];
            //EmailTemplate emailTemplate = agreement.Product.EmailTemplates.FirstOrDefault (et => et.Type == "SendPolicyDocuments");
            EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == model.Type);

            if (emailTemplate != null)
            {
                using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
                {
                    emailTemplate.Subject = model.Subject;
                    emailTemplate.Body = model.Body;
                    emailTemplate.LastModifiedBy = CurrentUser;
                    emailTemplate.LastModifiedOn = DateTime.UtcNow;

                    uow.Commit();
                }
            }
            else
            {
                using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
                {
                    emailTemplate = new EmailTemplate(CurrentUser, "Agreement Documents Covering Text", "SendPolicyDocuments", model.Subject, model.Body, null, programme.BaseProgramme);
                    programme.BaseProgramme.EmailTemplates.Add(emailTemplate);

                    uow.Commit();
                }
            }

            var docs = agreement.Documents.Where(d => d.DateDeleted == null);
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
                var user = _userRepository1.GetById(model.Recipent);
                strrecipentemail = user.Email;
                _logger.Info(user.Email.ToString());
            }

            _emailService.SendEmailViaEmailTemplate(strrecipentemail, emailTemplate, documents);

            return Redirect("~/Home/Index");

        }



        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditTerms(Guid id)
        {
            ViewAgreementViewModel model = new ViewAgreementViewModel();
            ClientAgreement agreement = _clientAgreementService.GetAgreement(id);
            ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV" && t.DateDeleted == null);
            model.ClientAgreementId = id;
            foreach (var terms in agreement.ClientAgreementTerms)
            {
                if (terms.BoatTerms.Where(bvt => bvt.DateDeleted == null).Count() > 0)
                {
                    var boats = new List<EditTermsViewModel>();
                    foreach (var boat in terms.BoatTerms)
                    {
                        boats.Add(new EditTermsViewModel
                        {
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

            ViewBag.Title = "Edit Terms ";

            return View("EditTerms", model);
        }
        [HttpPost]
        public ActionResult EditTerm(Guid clientAgreementId, EditTermsViewModel clientAgreementBVTerm)
        {
            ClientAgreement agreement = _clientAgreementService.GetAgreement(clientAgreementId);

            ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV" && t.DateDeleted == null);

            ClientAgreementBVTerm bvTerm = null;
            if (term.BoatTerms != null)
            {
                bvTerm = term.BoatTerms.FirstOrDefault(bvt => bvt.Boat.BoatName == clientAgreementBVTerm.BoatName);
               
            }
            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                term.Premium -= bvTerm.Premium;
                term.Premium += clientAgreementBVTerm.Premium;
                bvTerm.TermLimit = clientAgreementBVTerm.TermLimit;
                bvTerm.Excess = clientAgreementBVTerm.Excess;
                bvTerm.Premium = clientAgreementBVTerm.Premium;
                bvTerm.FSL = clientAgreementBVTerm.FSL;
                NewMethod(uow);
            }

            return RedirectToAction("EditTerms", new { id = clientAgreementId });
        }

        [HttpPost]
        public ActionResult EditMotorTerm(Guid clientAgreementId, EditTermsViewModel clientAgreementMVTerm)
        {
            ClientAgreement agreement = _clientAgreementService.GetAgreement(clientAgreementId);
            //ClientAgreementTerm term = agreement.ClientAgreementTerms;

            ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV" && t.DateDeleted == null);

            ClientAgreementMVTerm mvTerm = null;
            if (term.MotorTerms != null)
            {
                mvTerm = term.MotorTerms.FirstOrDefault(bvt => bvt.Model == clientAgreementMVTerm.Model);
            }

            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(UserTimeZone);
                term.Premium -= mvTerm.Premium;
                term.Premium += clientAgreementMVTerm.Premium;
                mvTerm.TermLimit = clientAgreementMVTerm.TermLimit;
                mvTerm.Excess = clientAgreementMVTerm.Excess;
                mvTerm.Premium = clientAgreementMVTerm.Premium;
                mvTerm.FSL = clientAgreementMVTerm.FSL;
                NewMethod(uow);
            }

            return RedirectToAction("EditTerms", new { id = clientAgreementId });
        }


        [HttpPost]
        public ActionResult DeleteTerm(Guid clientAgreementId, EditTermsViewModel clientAgreementBVTerm)
        {
            ClientAgreement agreement = _clientAgreementService.GetAgreement(clientAgreementId);

            ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV" && t.DateDeleted == null);
            ClientAgreementBVTerm bvTerm = null;

            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {

                if (term.BoatTerms != null)
                {
                    bvTerm = term.BoatTerms.FirstOrDefault(bvt => bvt.Boat.BoatName == clientAgreementBVTerm.BoatName);
                    term.BoatTerms.Remove(bvTerm);
                }
                if (term.MotorTerms != null)
                {
                    bvTerm = term.BoatTerms.FirstOrDefault(bvt => bvt.Boat.BoatName == clientAgreementBVTerm.BoatName);
                    term.BoatTerms.Remove(bvTerm);
                }

                NewMethod(uow);
            }

            return RedirectToAction("EditTerms", new { id = clientAgreementId });
        }


        //[HttpPost]
        //public ActionResult AddTerm(Guid clientAgreementId, EditTermsViewModel clientAgreementBVTerm)
        //{
        //    ClientAgreement agreement = _clientAgreementService.GetAgreement(clientAgreementId);
        //    ClientAgreementTerm term = agreement.ClientAgreementTerms.FirstOrDefault(t => t.SubTermType == "BV" && t.DateDeleted == null);

        //    using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
        //    {

        //        if (term.BoatTerms != null)
        //        {
        //            term.BoatTerms.Add(clientAgreementBVTerm);
        //        }

        //        NewMethod(uow);
        //    }
        //    return RedirectToAction("EditTerms", new { id = clientAgreementId });
        //}



        private static void NewMethod(IUnitOfWork uow)
        {
            uow.Commit();
        }

        [HttpGet]
        public IActionResult ViewAgreement(Guid id)
        {
            var models = new BaseListViewModel<ViewAgreementViewModel>();


            //ClientInformationSheet answerSheet = _customerInformationService.GetInformation (id);
            //ClientAgreement agreement = answerSheet.ClientAgreement;
            //InformationTemplate template = answerSheet.InformationTemplate;
            ////User insured = _userRepository.GetUser (answerSheet.Owner.Id);
            //Organisation insured = answerSheet.Owner;

            ClientProgramme clientProgramme = _programmeRepository.GetById(id);
            Organisation insured = clientProgramme.Owner;
            ClientInformationSheet answerSheet = clientProgramme.InformationSheet;


            var insuranceRoles = new List<InsuranceRoleViewModel>();

            NumberFormatInfo currencyFormat = new CultureInfo(CultureInfo.CurrentCulture.ToString()).NumberFormat;
            currencyFormat.CurrencyNegativePattern = 2;

            // List Agreement Parties
            insuranceRoles.Add(new InsuranceRoleViewModel { RoleName = "Customer", Name = insured.Name, ManagedBy = "", Email = "" });
            //TODO - get from sheet or org?
            //foreach (Product product in products)
            //	foreach (KeyValuePair<string, Organisation> kvp in product.Parties) {
            //		insuranceRoles.Add (new InsuranceRoleViewModel () { RoleName = kvp.Key, Name = kvp.Value.Name, ManagedBy = kvp.Value.Name, Email = "" });
            //	}

            foreach (ClientAgreement agreement in clientProgramme.Agreements)
            {
                ViewAgreementViewModel model = new ViewAgreementViewModel
                {
                    EditEnabled = true,
                    ClientAgreementId = agreement.Id,
                    ClientProgrammeId = clientProgramme.Id
                };

                var insuranceInclusion = new List<InsuranceInclusion>();
                var insuranceExclusion = new List<InsuranceExclusion>();
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
                    insuranceInclusion.Add(new InsuranceInclusion { RiskName = riskname, Inclusion = "Limit: " + term.TermLimit.ToString("C") });
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

                // List Agreement Premiums
                foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms)
                {
                    if (answerSheet.PreviousInformationSheet == null)
                    {
                        riskPremiums.Add(new RiskPremiumsViewModel { RiskName = riskname, Premium = (term.Premium - term.FSL).ToString("C"), FSL = term.FSL.ToString("C"), TotalPremium = term.Premium.ToString("C") });
                    }
                    else
                    {
                        riskPremiums.Add(new RiskPremiumsViewModel { RiskName = riskname, Premium = string.Format(currencyFormat, "{0:c}", (term.PremiumDiffer - term.FSLDiffer)), FSL = string.Format(currencyFormat, "{0:c}", term.FSLDiffer), TotalPremium = string.Format(currencyFormat, "{0:c}", term.PremiumDiffer) });
                    }
                }

                // Populate the ViewModel
                model.InsuranceRoles = insuranceRoles;
                model.Inclusions = insuranceInclusion;
                model.Exclusions = insuranceExclusion;
                model.RiskPremiums = riskPremiums;

                // Status
                model.ProductName = agreement.Product.Name;
                model.Status = agreement.Status;
                model.InformationSheetStatus = agreement.ClientInformationSheet.Status;
                model.StartDate = LocalizeTimeDate(agreement.InceptionDate, "dd-mm-yyyy");
                model.EndDate = LocalizeTimeDate(agreement.ExpiryDate, "dd-mm-yyyy");
                model.AdministrationFee = agreement.BrokerFee.ToString("C");
                model.BrokerageRate = (agreement.Brokerage / 100).ToString("P2");
                model.CurrencySymbol = "fa fa-dollar";
                model.ClientNumber = agreement.ClientNumber;
                model.PolicyNumber = agreement.PolicyNumber;
                var userRoles = CurrentUser.GetRoles().ToArray();
                var hasViewAllRole = userRoles.FirstOrDefault(r => r.Name == "CanViewAllInformation") != null;
                if (hasViewAllRole)
                {
                    model.UserRoles.Add("CanViewAllInformation");
                }
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
                                boats.Add(new BoatViewModel { BoatName = b.BoatName, BoatMake = b.BoatMake, BoatModel = b.BoatModel, MaxSumInsured =  b.MaxSumInsured, BoatQuoteExcessOption = b.BoatQuoteExcessOption });
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

            return PartialView("_ViewAgreementList", models);
        }

        

        [HttpPost]
        public ActionResult SaveDate(Guid id , string Startdate)
        {
            ClientAgreement clientAgreement = _clientAgreementService.GetAgreement(id);

            var date = true;
            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                // TODO - Convert to UTC
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(UserTimeZone);
                clientAgreement.InceptionDate = DateTime.Parse(Startdate, UserCulture).ToUniversalTime(tzi);
                clientAgreement.ExpiryDate = clientAgreement.InceptionDate.AddYears(1);
                clientAgreement.CustomInceptionDate = true;

                //update boat effective date if the date is prior to the new start date or over 30 days after the new start date
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

                string auditLogDetail = "Agreement start date and end date have been modified by " + CurrentUser.FullName;
                AuditLog auditLog = new AuditLog(CurrentUser, clientAgreement.ClientInformationSheet, clientAgreement, auditLogDetail);
                clientAgreement.ClientAgreementAuditLogs.Add(auditLog);

                uow.Commit();
            }

           
            return Json(date);
        }



        [HttpGet]
        public ActionResult ViewAgreementDeclaration(Guid id)
        {
            var models = new BaseListViewModel<ViewAgreementViewModel>();
            ClientProgramme clientProgramme = _programmeRepository.GetById(id);
            Organisation insured = clientProgramme.Owner;
            ClientInformationSheet answerSheet = clientProgramme.InformationSheet;

            foreach (ClientAgreement agreement in clientProgramme.Agreements)
            {
                ViewAgreementViewModel model = new ViewAgreementViewModel
                {
                    EditEnabled = true,
                    ClientAgreementId = agreement.Id,
                    ClientProgrammeId = clientProgramme.Id
                };

                // Status
                model.Status = agreement.Status;
                model.InformationSheetId = answerSheet.Id;
                models.Add(model);
            }

            ViewBag.Title = clientProgramme.BaseProgramme.Name + " Agreement for " + insured.Name;

            return PartialView("_ViewAgreementDeclaration", models);
        }


        [HttpGet]
        public ActionResult ViewPayment(Guid id)
        {

            //need to review this code duplication
            var models = new BaseListViewModel<ViewAgreementViewModel>();

            ClientProgramme clientProgramme = _programmeRepository.GetById(id);
            ClientInformationSheet answerSheet = clientProgramme.InformationSheet;
            NumberFormatInfo currencyFormat = new CultureInfo(CultureInfo.CurrentCulture.ToString()).NumberFormat;
            currencyFormat.CurrencyNegativePattern = 2;

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
                }

                // List Agreement Premiums
                foreach (ClientAgreementTerm term in agreement.ClientAgreementTerms)
                {
                    if (answerSheet.PreviousInformationSheet == null)
                    {
                        riskPremiums.Add(new RiskPremiumsViewModel { RiskName = riskname, Premium = (term.Premium - term.FSL).ToString("C"), FSL = term.FSL.ToString("C"), TotalPremium = term.Premium.ToString("C") });
                    }
                    else
                    {
                        riskPremiums.Add(new RiskPremiumsViewModel { RiskName = riskname, Premium = string.Format(currencyFormat, "{0:c}", (term.PremiumDiffer - term.FSLDiffer)), FSL = string.Format(currencyFormat, "{0:c}", term.FSLDiffer), TotalPremium = string.Format(currencyFormat, "{0:c}", term.PremiumDiffer) });
                    }
                }

                bool isActive = true;
                
                //new implementation using httpclient
                //EGlobalSerializerAPI eGlobalSerializer = new EGlobalSerializerAPI();
                //try
                //{
                //    throw new Exception("method needs to use new httpclient");
                //    //if (eGlobalSerializer.SiteActive())
                //    //{
                //    //    _logger.Info("Active");
                //    //    isActive = true;
                //    //}
                //}
                //catch (Exception ex)
                //{
                //    _logger.Info("Inactive");
                //    isActive = false;
                //    ErrorSignal.FromCurrentContext().Raise(ex);
                //    _emailService.ContactSupport(_emailService.DefaultSender, "eglobal API inactive", ex.Message);
                //    //if (ex != null)
                //    //{
                //    //    _logger.Error(ex.InnerException.Message);
                //    //    _logger.Error(ex.Message);
                    //}
               // }
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
                models.Add(model);
            }


            return PartialView("_ViewPaymentList", models);
        }


        [HttpGet]
        public ActionResult EditAgreement(Guid id)
        {
            ViewAgreementViewModel model = new ViewAgreementViewModel();

            ClientAgreement agreement = _clientAgreementService.GetAgreement(id);
            ClientInformationSheet answerSheet = agreement.ClientInformationSheet;
            Organisation insured = answerSheet.Owner;
            ClientProgramme programme = answerSheet.Programme;

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

            ViewBag.Title = answerSheet.Programme.BaseProgramme.Name + " Edit Agreement for " + insured.Name;

            string auditLogDetail = "Agreement details have been modified by " + CurrentUser.FullName;
            AuditLog auditLog = new AuditLog(CurrentUser, answerSheet, agreement, auditLogDetail);
            agreement.ClientAgreementAuditLogs.Add(auditLog);

            return View("EditAgreement", model);
        }

        [HttpPost]
        public ActionResult EditAgreement(ViewAgreementViewModel model)
        {
            ClientAgreement agreement = _clientAgreementService.GetAgreement(model.ClientAgreementId);
            ClientInformationSheet answerSheet = agreement.ClientInformationSheet;

            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                // TODO - Convert to UTC
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(UserTimeZone);
                agreement.InceptionDate = DateTime.Parse(model.StartDate, UserCulture).ToUniversalTime();
                agreement.ExpiryDate = DateTime.Parse(model.EndDate, UserCulture).ToUniversalTime();
                agreement.Brokerage = Convert.ToDecimal(model.BrokerageRate.Replace("%", ""));
                agreement.BrokerFee = Convert.ToDecimal(model.AdministrationFee.Replace("$", ""));
                agreement.ClientNumber = model.ClientNumber;
                agreement.PolicyNumber = model.PolicyNumber;

                uow.Commit();
            }

            return Redirect("/Agreement/ViewAcceptedAgreement/" + answerSheet.Programme.Id);
        }


        [HttpGet]
        public ActionResult ViewAgreementRule(Guid id)
        {
            ViewAgreementRuleViewModel model = new ViewAgreementRuleViewModel();

            ClientAgreement agreement = _clientAgreementService.GetAgreement(id);
            ClientInformationSheet answerSheet = agreement.ClientInformationSheet;
            Organisation insured = answerSheet.Owner;

            //Client Agreement Rules
            model.HasRules = agreement.ClientAgreementRules.Count > 0;

            model.ClientAgreementID = id;
            model.ClientProgrammeID = answerSheet.Programme.Id;

            if (model.HasRules)
            {
                var clientAgreementRules = new AgreementRulesViewModel();
                foreach (ClientAgreementRule cr in agreement.ClientAgreementRules.OrderBy(cr => cr.OrderNumber))
                {
                    clientAgreementRules.Add(new ClientAgreementRuleViewModel { ClientAgreementRuleID = cr.Id, Description = cr.Description, Value = cr.Value });
                }
                model.ClientAgreementRules = clientAgreementRules;

            }

            ViewBag.Title = answerSheet.Programme.BaseProgramme.Name + " Agreement Rule for " + insured.Name;

            return View("ViewAgreementRule", model);
            //return View(model);
        }

        [HttpPost]
        public ActionResult ViewAgreementRule(ViewAgreementRuleViewModel model)
        {
            ClientAgreement agreement = _clientAgreementService.GetAgreement(model.ClientAgreementID);
            if (model.ClientAgreementRules.Any(mcr => mcr != null && mcr.Value != null))
            {
                using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
                {
                    foreach (ClientAgreementRuleViewModel crv in model.ClientAgreementRules.OrderBy(cr => cr.OrderNumber))
                    {
                        _clientAgreementRuleService.GetClientAgreementRuleBy(crv.ClientAgreementRuleID).Value = crv.Value;
                    }
                    uow.Commit();
                }
            }

            return Redirect("/Information/EditInformation/" + model.ClientProgrammeID);

        }

        [HttpGet]
        public ActionResult ViewAgreementEndorsement(Guid id)
        {
            ViewAgreementEndorsementViewModel model = new ViewAgreementEndorsementViewModel();

            ClientAgreement agreement = _clientAgreementService.GetAgreement(id);
            ClientInformationSheet answerSheet = agreement.ClientInformationSheet;
            Organisation insured = answerSheet.Owner;

            //Client Agreement Endorsements
            model.HasEndorsements = agreement.ClientAgreementEndorsements.Count > 0;

            model.ClientAgreementID = id;

            model.ClientProgrammeID = answerSheet.Programme.Id;

            if (model.HasEndorsements)
            {
                var clientAgreementEndorsements = new AgreementEndorsementsViewModel();
                foreach (ClientAgreementEndorsement ce in agreement.ClientAgreementEndorsements.OrderBy(ce => ce.OrderNumber))
                {
                    clientAgreementEndorsements.Add(new ClientAgreementEndorsementViewModel { ClientAgreementEndorsementID = ce.Id, Name = ce.Name, Value = ce.Value });
                }
                model.ClientAgreementEndorsements = clientAgreementEndorsements;

            }
            else
            {
                model.ClientAgreementEndorsements = null;
            }

            ViewBag.Title = answerSheet.Product.Name + " Agreement Endorsements for " + insured.Name;

            return View("ViewAgreementEndorsement", model);
            //return View(model);
        }

        [HttpPost]
        public ActionResult ViewAgreementEndorsement(ViewAgreementEndorsementViewModel model)
        {
            ClientAgreement agreement = _clientAgreementService.GetAgreement(model.ClientAgreementID);

            if (model.EndorsementNameToAdd != null && model.EndorsementTextToAdd != null)
            {
                _clientAgreementEndorsementService.AddClientAgreementEndorsement(CurrentUser, model.EndorsementNameToAdd, "Exclusion", agreement.Product, model.EndorsementTextToAdd, 100, agreement);

            }

            if (model.ClientAgreementEndorsements != null)
            {
                using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
                {
                    foreach (ClientAgreementEndorsementViewModel cev in model.ClientAgreementEndorsements.OrderBy(ce => ce.OrderNumber))
                    {
                        _clientAgreementEndorsementService.GetClientAgreementEndorsementBy(cev.ClientAgreementEndorsementID).Value = cev.Value;
                    }

                    uow.Commit();
                }

            }

            return Redirect("/Information/EditInformation/" + model.ClientProgrammeID);

        }


        [HttpGet]
        public ActionResult AcceptAgreement(Guid Id)
        {
            List<AgreementDocumentViewModel> models = new List<AgreementDocumentViewModel>();
            ClientProgramme programme = _programmeRepository.GetById(Id);

            foreach (ClientAgreement agreement in programme.Agreements)
            {
                if (agreement == null)
                    throw new Exception(string.Format("No Agreement found for {0}", agreement.Id));

                foreach (SystemDocument doc in agreement.Documents.Where(d => d.DateDeleted == null))
                {
                    doc.Delete(CurrentUser);
                }

                foreach (SystemDocument template in agreement.Product.Documents)
                {
                    SystemDocument renderedDoc = _fileService.RenderDocument(CurrentUser, template, agreement);
                    renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                    agreement.Documents.Add(renderedDoc);
                    _fileService.UploadFile(renderedDoc);
                }

                ClientAgreement reloadedAgreement = _clientAgreementService.GetAgreement(agreement.Id);
                foreach (SystemDocument doc in reloadedAgreement.Documents.Where(d => d.DateDeleted == null))
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
                    _clientAgreementService.AcceptAgreement(agreement, CurrentUser);
                }

            }

            return PartialView("_ViewAgreementDocs", models);
            //return View("_ViewAgreementDocs");
        }

        [HttpGet]
        public ActionResult SendPolicyDocuments(Guid id)
        {
            ClientInformationSheet sheet = _customerInformationService.GetInformation(id);

            // TODO - rewrite to save templates on a per programme basis

            ClientProgramme programme = sheet.Programme;
            ClientAgreement agreement = programme.Agreements[0];

            Organisation insured = programme.Owner;

            EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == "SendPolicyDocuments");

            EmailTemplateViewModel model = new EmailTemplateViewModel();

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

                recipents.Add(new UserViewModel { ID = CurrentUser.Id, UserName = CurrentUser.UserName, FirstName = CurrentUser.FirstName, LastName = CurrentUser.LastName, FullName = CurrentUser.FullName, Email = CurrentUser.Email });

                foreach (User recipent in _userRepository1.FindAll().Where(ur1 => ur1.Organisations.Contains(programme.Owner)))
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

        [HttpPost]
        public ActionResult SendPolicyDocuments(EmailTemplateViewModel model)
        {
            ClientProgramme programme = _programmeRepository.GetById(model.ClientProgrammeID);

            // TODO - rewrite to save templates on a per programme basis
            ClientAgreement agreement = programme.Agreements[0];
            //EmailTemplate emailTemplate = agreement.Product.EmailTemplates.FirstOrDefault (et => et.Type == "SendPolicyDocuments");
            EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == "SendPolicyDocuments");

            if (emailTemplate != null)
            {
                using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
                {
                    emailTemplate.Subject = model.Subject;
                    emailTemplate.Body = model.Body;
                    emailTemplate.LastModifiedBy = CurrentUser;
                    emailTemplate.LastModifiedOn = DateTime.UtcNow;

                    uow.Commit();
                }
            }
            else
            {
                using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
                {
                    emailTemplate = new EmailTemplate(CurrentUser, "Agreement Documents Covering Text", "SendPolicyDocuments", model.Subject, model.Body, null, programme.BaseProgramme);
                    programme.BaseProgramme.EmailTemplates.Add(emailTemplate);

                    uow.Commit();
                }
            }

            var docs = agreement.Documents.Where(d => d.DateDeleted == null);
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
                var user = _userRepository1.GetById(model.Recipent);
                strrecipentemail = user.Email;
                _logger.Info(user.Email.ToString());
            }

            _emailService.SendEmailViaEmailTemplate(strrecipentemail, emailTemplate, documents);

            return Redirect("~/Home/Index");

        }

        [HttpPost]
        public IActionResult GeneratePxPayment1(IFormCollection collection)
        {
            Guid sheetId = Guid.Empty;
            ClientInformationSheet sheet = null;
            if (Guid.TryParse(HttpContext.Request.Form["AnswerSheetId"], out sheetId))
            {

                sheet = _customerInformationService.GetInformation(sheetId);
            }

               ClientProgramme programme = sheet.Programme;
               programme.PaymentType = "Credit Card";


            //EGlobalSerializerAPI eGlobalSerializer = new EGlobalSerializerAPI();
            //eGlobalSerializer.SiteActive();
            //eGlobalSerializer.SerializePolicy(programme, CurrentUser);
            //Hardcoded variables
            decimal totalPremium = 0, totalPayment, brokerFee = 0, GST = 1.15m, creditCharge = 1.02m;
            Merchant merchant = _merchantService.GetMerchant(programme.BaseProgramme.Id);
            Payment payment = _paymentService.GetPayment(programme.Id);
            if (payment == null)
            {
                payment = _paymentService.AddNewPayment(sheet.CreatedBy, programme, merchant, merchant.MerchantPaymentGateway);
            }

            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                programme.PaymentType = "Credit Card";
                programme.Payment = payment;
                programme.InformationSheet.Status = "Bound";
                uow.Commit();
            }


            //add check to count how many failed payments
            var ProgrammeId = sheetId;
            foreach (ClientAgreement clientAgreement in programme.Agreements)
            {
                ProgrammeId = programme.Id;
                brokerFee += clientAgreement.BrokerFee;
                var terms = _clientAgreementTermService.GetAllAgreementTermFor(clientAgreement);
                foreach (ClientAgreementTerm clientAgreementTerm in terms)
                {
                    totalPremium += clientAgreementTerm.Premium;
                }
            }
            totalPayment = Math.Round(((totalPremium + brokerFee) * (GST) * (creditCharge)), 2);

            PxPay pxPay = new PxPay(merchant.MerchantPaymentGateway.PaymentGatewayWebServiceURL, merchant.MerchantPaymentGateway.PxpayUserId, merchant.MerchantPaymentGateway.PxpayKey);

            //string domainQueryString = WebConfigurationManager.AppSettings["DomainQueryString"].ToString();
            string domainQueryString = "localhost:44323";
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

        [HttpPost]
        public IActionResult GeneratePxPayment(IFormCollection collection)
        {
            Guid sheetId = Guid.Empty;

            ClientInformationSheet sheet = null;
            if (Guid.TryParse(HttpContext.Request.Form["AnswerSheetId"], out sheetId))
            {
                sheet = _customerInformationService.GetInformation(sheetId);
            }

            ClientProgramme programme = sheet.Programme;
            programme.PaymentType = "Credit Card";
            //EGlobalSerializerAPI eGlobalSerializer = new EGlobalSerializerAPI();
            //eGlobalSerializer.SiteActive();
            //eGlobalSerializer.SerializePolicy(programme, CurrentUser);
            //Hardcoded variables
            decimal totalPremium = 0, totalPayment, brokerFee = 0, GST = 1.15m, creditCharge = 1.02m;
            Merchant merchant = _merchantService.GetMerchant(programme.BaseProgramme.Id);
            Payment payment = _paymentService.GetPayment(programme.Id);
            if (payment == null)
            {
                payment = _paymentService.AddNewPayment(sheet.CreatedBy, programme, merchant, merchant.MerchantPaymentGateway);
            }

            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                programme.PaymentType = "Credit Card";
                programme.Payment = payment;
                programme.InformationSheet.Status = "Bound";
                uow.Commit();
            }

            //add check to count how many failed payments
            var ProgrammeId = sheetId;
            foreach (ClientAgreement clientAgreement in programme.Agreements)
            {
                ProgrammeId = programme.Id;
                brokerFee += clientAgreement.BrokerFee;
                var terms = _clientAgreementTermService.GetAllAgreementTermFor(clientAgreement);
                foreach (ClientAgreementTerm clientAgreementTerm in terms)
                {
                    totalPremium += clientAgreementTerm.Premium;
                }
            }
            totalPayment = Math.Round(((totalPremium + brokerFee) * (GST) * (creditCharge)), 2);

            PxPay pxPay = new PxPay(merchant.MerchantPaymentGateway.PaymentGatewayWebServiceURL, merchant.MerchantPaymentGateway.PxpayUserId, merchant.MerchantPaymentGateway.PxpayKey);

            //string domainQueryString = WebConfigurationManager.AppSettings["DomainQueryString"].ToString();
            string domainQueryString = "localhost:44323";
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


        //[HttpPost]
        //public ActionResult GeneratePxPayment(IFormCollection collection)
        //{

        //    ClientInformationSheet sheet = null;

        //    if (answerSheetId != null)
        //    {
        //        sheet = _customerInformationService.GetInformation(answerSheetId);
        //    }

        //    ClientProgramme programme = sheet.Programme;
        //    programme.PaymentType = "Credit Card";
        //    //EGlobalSerializerAPI eGlobalSerializer = new EGlobalSerializerAPI();
        //    //eGlobalSerializer.SiteActive();
        //    //eGlobalSerializer.SerializePolicy(programme, CurrentUser);
        //    //Hardcoded variables
        //    decimal totalPremium = 0, totalPayment, brokerFee = 0, GST = 1.15m, creditCharge = 1.02m;
        //    Merchant merchant = _merchantService.GetMerchant(programme.BaseProgramme.Id);
        //    Payment payment = _paymentService.GetPayment(programme.Id);
        //    if (payment == null)
        //    {
        //        payment = _paymentService.AddNewPayment(sheet.CreatedBy, programme, merchant, merchant.MerchantPaymentGateway);
        //    }

        //    using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
        //    {
        //        programme.PaymentType = "Credit Card";
        //        programme.Payment = payment;
        //        programme.InformationSheet.Status = "Bound";
        //        uow.Commit();
        //    }

        //    //add check to count how many failed payments
        //    var ProgrammeId = answerSheetId;
        //    foreach (ClientAgreement clientAgreement in programme.Agreements)
        //    {
        //        ProgrammeId = programme.Id;
        //        brokerFee += clientAgreement.BrokerFee;
        //        var terms = _clientAgreementTermService.GetAllAgreementTermFor(clientAgreement);
        //        foreach (ClientAgreementTerm clientAgreementTerm in terms)
        //        {
        //            totalPremium += clientAgreementTerm.Premium;
        //        }
        //    }
        //    totalPayment = Math.Round(((totalPremium + brokerFee) * (GST) * (creditCharge)), 2);

        //    PxPay pxPay = new PxPay(merchant.MerchantPaymentGateway.PaymentGatewayWebServiceURL, merchant.MerchantPaymentGateway.PxpayUserId, merchant.MerchantPaymentGateway.PxpayKey);

        //    //string domainQueryString = WebConfigurationManager.AppSettings["DomainQueryString"].ToString();
        //    string domainQueryString = "localhost:44323";
        //    RequestInput input = new RequestInput
        //    {
        //        AmountInput = totalPayment.ToString("0.00"),
        //        CurrencyInput = "NZD",
        //        TxnType = "Purchase",
        //        UrlFail = "http://" + domainQueryString + payment.PaymentPaymentGateway.PxpayUrlFail + ProgrammeId.ToString(),
        //        UrlSuccess = "http://" + domainQueryString + payment.PaymentPaymentGateway.PxpayUrlSuccess + ProgrammeId.ToString(),
        //        TxnId = payment.Id.ToString("N").Substring(0, 16),
        //    };

        //    RequestOutput requestOutput = pxPay.GenerateRequest(input);

        //    //opens on same page - hard to return back to current process
        //    return Json(new { url = requestOutput.Url });

        //}

        [HttpPost]
        public IActionResult GenerateEGlobal(IFormCollection collection)
        {
            ClientInformationSheet sheet = null;
            //throw new Exception("Method will need to be re-written");
            if (Guid.TryParse(HttpContext.Request.Form["AnswerSheetId"], out Guid sheetId))
            {
                sheet = _customerInformationService.GetInformation(sheetId);
            }

            //Hardcoded variables
            ClientProgramme programme = sheet.Programme;
            programme.PaymentType = "Invoice";
            //EGlobalSerializerAPI eGlobalSerializer = new EGlobalSerializerAPI();

            //check Eglobal parameters
            if (programme.EGlobalClientNumber == "")
            {
                throw new Exception(nameof(programme.EGlobalClientNumber) + " EGlobal client number");
            }

            //if (!eGlobalSerializer.SiteActive())
            //{
            //    var xmlBody = eGlobalSerializer.SerializePolicy(programme, CurrentUser);
            //    _emailService.SendSystemEmailEGlobalTCNotify(xmlBody);
            //}

            decimal totalPremium = 0, totalPayment, brokerFee = 0, GST = 1.15m;

            //add check to count how many failed payments
            var ProgrammeId = sheetId;
            foreach (ClientAgreement clientAgreement in programme.Agreements)
            {
                ProgrammeId = programme.Id;
                brokerFee += clientAgreement.BrokerFee;
                var terms = _clientAgreementTermService.GetAllAgreementTermFor(clientAgreement);
                foreach (ClientAgreementTerm clientAgreementTerm in terms)
                {
                    totalPremium += clientAgreementTerm.Premium;
                }
            }

            var status = "bound";
            using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                if (programme.InformationSheet.Status != status)
                {
                    programme.PaymentType = "Invoice";
                    programme.InformationSheet.Status = status;
                    uow.Commit();
                }
            }

            return Redirect("~/Agreement/ViewAcceptedAgreement/" + ProgrammeId.ToString());
        }


        [HttpGet]
        public ActionResult ProcessRequestConfiguration(Guid Id)
        {

            //string url = this.Url;
            QueryString queryString = HttpContext.Request.QueryString;
            var status = "Bound";

            ClientProgramme programme = _programmeRepository.GetById(Id);
            Payment payment = _paymentService.GetPayment(programme.Id);


            PxPay pxPay = new PxPay(payment.PaymentMerchant.MerchantPaymentGateway.PaymentGatewayWebServiceURL, payment.PaymentMerchant.MerchantPaymentGateway.PxpayUserId, payment.PaymentMerchant.MerchantPaymentGateway.PxpayKey);
            ResponseOutput responseOutput = pxPay.ProcessResponse(queryString.ToString());

            payment.PaymentAttempts += 1;
            payment.CreditCardType = responseOutput.CardName;
            payment.CreditCardNumber = responseOutput.CardNumber;
            payment.IsPaid = responseOutput.Success == "1" ? true : false;
            payment.PaymentAmount = Convert.ToDecimal(responseOutput.AmountSettlement);
            _paymentService.Update(payment);

            if (!payment.IsPaid)
            {
                //Payment failed
                status = "Bound and pending payment";
                _emailService.SendSystemPaymentFailConfigEmailUISIssueNotify(programme.BrokerContactUser, programme.BaseProgramme, programme.InformationSheet, programme.Owner);
                //return Redirect("~/Agreement/ProccessedAgreements/" + Id.ToString());
                return RedirectToAction("ProcessedAgreements", new { id = Id });

            }
            else
            {
                //Payment successed
                _emailService.SendSystemPaymentSuccessConfigEmailUISIssueNotify(programme.BrokerContactUser, programme.BaseProgramme, programme.InformationSheet, programme.Owner);

                EmailTemplate emailTemplate = programme.BaseProgramme.EmailTemplates.FirstOrDefault(et => et.Type == "SendPolicyDocuments");

                if (emailTemplate == null)
                {
                    //default email or send them somewhere??

                    using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
                    {
                        emailTemplate = new EmailTemplate(CurrentUser, "Agreement Documents Covering Text", "SendPolicyDocuments", "Policy Documents for ", WebUtility.HtmlDecode("Email Containing policy documents"), null, programme.BaseProgramme);
                        programme.BaseProgramme.EmailTemplates.Add(emailTemplate);
                        uow.Commit();
                    }
                }


                var hasEglobalNo = programme.EGlobalClientNumber != null ? true : false;

                if (hasEglobalNo)
                {
                    status = "Bound and invoiced";
                }
                else
                    status = "Bound and invoice pending";

                var documents = new List<SystemDocument>();
                foreach (ClientAgreement agreement in programme.Agreements)
                {
                    using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
                    {
                        if (agreement.Status != status)
                        {
                            agreement.Status = status;
                            uow.Commit();
                        }
                    }


                    agreement.Status = status;

                    foreach (SystemDocument doc in agreement.Documents.Where(d => d.DateDeleted == null))
                    {
                        doc.Delete(CurrentUser);
                    }
                    foreach (SystemDocument template in agreement.Product.Documents)
                    {
                        //render docs except invoice
                        if (template.DocumentType != 4)
                        {
                            SystemDocument renderedDoc = _fileService.RenderDocument(CurrentUser, template, agreement);
                            renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                            agreement.Documents.Add(renderedDoc);
                            documents.Add(renderedDoc);
                            _fileService.UploadFile(renderedDoc);
                        }

                    }
                    //else
                    //{
                    //    agreement.Documents.Add(renderedDoc);
                    //    documents.Add(renderedDoc);
                    //    _fileService.UploadFile(renderedDoc);
                    //}

                    //_emailService.SendSystemEmailAgreementBoundNotify(programme.BrokerContactUser, programme.BaseProgramme, agreement, programme.Owner);
                    _emailService.SendEmailViaEmailTemplate(programme.BrokerContactUser.Email, emailTemplate, documents);


                    _emailService.SendSystemEmailAgreementBoundNotify(programme.BrokerContactUser, programme.BaseProgramme, agreement, programme.Owner);
                    _emailService.SendEmailViaEmailTemplate(programme.BrokerContactUser.Email, emailTemplate, documents);
                }

                if (hasEglobalNo)
                {
                    _emailService.SendSystemSuccessInvoiceConfigEmailUISIssueNotify(programme.BrokerContactUser, programme.BaseProgramme, programme.InformationSheet, programme.Owner);
                }
                else
                {
                    _emailService.SendSystemFailedInvoiceConfigEmailUISIssueNotify(programme.BrokerContactUser, programme.BaseProgramme, programme.InformationSheet, programme.Owner);
                }

                using (var uow = _unitOfWorkFactory.BeginUnitOfWork())
                {
                    if (programme.InformationSheet.Status != status)
                    {
                        programme.InformationSheet.Status = status;
                        uow.Commit();
                    }
                }
            }
           return RedirectToAction("ProcessedAgreements", new { id = Id });
           // return Redirect("~/Agreement/ProcessedAgreements/" + Id);
        }

        [HttpGet]
        public IActionResult ProcessedAgreements(Guid id)
        {
            PartialViewResult result = (PartialViewResult)ViewAgreement(id);

            var models = (BaseListViewModel<ViewAgreementViewModel>)result.Model;
            foreach (ViewAgreementViewModel model in models)
            {
                model.EditEnabled = false;
                model.Documents = new List<AgreementDocumentViewModel>();

                ClientProgramme programme = _programmeRepository.GetById(id);
                model.InformationSheetId = programme.InformationSheet.Id;
                model.ClientProgrammeId = id;
                foreach (ClientAgreement agreement in programme.Agreements)
                {
                    model.ClientAgreementId = agreement.Id;
                    foreach (Document doc in agreement.Documents.Where(d => d.DateDeleted == null))
                    {
                        if(doc.DocumentType == 4)
                        {
                            if(programme.EGlobalClientNumber != null)
                            {
                                model.Documents.Add(new AgreementDocumentViewModel { DisplayName = doc.Name, Url = "/File/GetDocument/" + doc.Id });
                            }
                        } else
                            model.Documents.Add(new AgreementDocumentViewModel { DisplayName = doc.Name, Url = "/File/GetDocument/" + doc.Id });
                    }
                }
            }
            return View("ViewProccessedAgreementList", models);
        }

        [HttpGet]
        public ActionResult ViewAcceptedAgreement(Guid id)
        {
            PartialViewResult result = (PartialViewResult)ViewAgreement(id);

            var models = (BaseListViewModel<ViewAgreementViewModel>)result.Model;

            foreach (ViewAgreementViewModel model in models)
            {
                model.EditEnabled = false;
                model.Documents = new List<AgreementDocumentViewModel>();
                model.CurrentUser = CurrentUser;

                ClientProgramme programme = _programmeRepository.GetById(id);
                model.InformationSheetId = programme.InformationSheet.Id;
                model.ClientProgrammeId = id;
                foreach (ClientAgreement agreement in programme.Agreements)
                {
                    model.ClientAgreementId = agreement.Id;
                    foreach (Document doc in agreement.Documents.Where(d => d.DateDeleted == null))
                    {
                        model.Documents.Add(new AgreementDocumentViewModel { DisplayName = doc.Name, Url = "/File/GetDocument/" + doc.Id });
                    }
                }
            }
            return View("ViewAcceptedAgreementList", models);
        }


        /*		[HttpGet]
                public ActionResult ViewAgreement(Guid id)
                {
                    ViewAgreementViewModel model = new ViewAgreementViewModel();

                    ClientInformationSheet answerSheet = _customerInformationService.GetInformation (id);
                    InformationTemplate informationTemplate = answerSheet.InformationTemplate;
                    IList<Product> products = informationTemplate.Products;
                    User insured = _userRepository.GetUser (answerSheet.OwnerId);

                    // List Agreement Parties
                    var insuranceRoles = new List<InsuranceRoleViewModel>();

                    insuranceRoles.Add(new InsuranceRoleViewModel() { RoleName = "Customer", Name = insured.FullName, ManagedBy = insured.FullName, Email = insured.Email});
                    //foreach (Product product in products)
                    //	foreach (KeyValuePair<string, Organisation> kvp in product.Parties) {
                    //		insuranceRoles.Add (new InsuranceRoleViewModel () { RoleName = kvp.Key, Name = kvp.Value.Name, ManagedBy = kvp.Value.Name, Email = "" });
                    //	}

                    // List Agreement Inclusions
                    var insuranceInclusion = new List<InsuranceInclusion>();

                    insuranceInclusion.Add(new InsuranceInclusion() { RiskName = "Medical Malpractice", Inclusion = "Limit: $500,000" });
                    insuranceInclusion.Add(new InsuranceInclusion() { RiskName = "Statutory Liability", Inclusion = "Limit: $500,000" });
                    insuranceInclusion.Add(new InsuranceInclusion() { RiskName = "General Liability", Inclusion = "Limit: $1,000,000" });
                    insuranceInclusion.Add(new InsuranceInclusion() { RiskName = "Medical Malpractice", Inclusion = _MPCourAttendanceEndor });
                    insuranceInclusion.Add(new InsuranceInclusion() { RiskName = "Statutory Liability", Inclusion = _SLAmendmentstoDefinitionEndor });

                    // List Agreement Exclusions
                    var insuranceExclusion = new List<InsuranceExclusion>();

                    insuranceExclusion.Add(new InsuranceExclusion() { RiskName = "Medical Malpractice", Exclusion = "Excess: $2,000" });
                    insuranceExclusion.Add(new InsuranceExclusion() { RiskName = "Statutory Liability", Exclusion = "Excess: $500" });
                    insuranceExclusion.Add(new InsuranceExclusion() { RiskName = "General Liability", Exclusion = "Excess: $500" });
                    insuranceExclusion.Add(new InsuranceExclusion() { RiskName = "Medical Malpractice", Exclusion = _MPSplitLimitofIndemnityEndor });
                    insuranceExclusion.Add(new InsuranceExclusion() { RiskName = "General Liability", Exclusion = _GLBusinessAdviceorServiceExclusion2Endor });
                    insuranceExclusion.Add(new InsuranceExclusion() { RiskName = "Statutory Liability", Exclusion = _SLExclusionEndor });
                    insuranceExclusion.Add(new InsuranceExclusion() { RiskName = "Statutory Liability", Exclusion = _SLSplitLimitofIndemnityEndor });

                    // List Agreement Premiums
                    var riskPremiums = new List<RiskPremiumsViewModel>();

                    riskPremiums.Add(new RiskPremiumsViewModel() { RiskName = "Medical Malpractice", Premium = "$230.00" });
                    riskPremiums.Add(new RiskPremiumsViewModel() { RiskName = "Statutory Liability", Premium = "Included" });
                    riskPremiums.Add(new RiskPremiumsViewModel() { RiskName = "General Liability", Premium = "Included" });

                    // Populate the ViewModel
                    model.InsuranceRoles = insuranceRoles;
                    model.Inclusions = insuranceInclusion;
                    model.Exclusions = insuranceExclusion;
                    model.RiskPremiums = riskPremiums;

                    // Statuss
                    model.ProductName = informationTemplate.Name;;
                    model.Status = "Quoted";
                    model.StartDate = DateTime.UtcNow;
                    model.EndDate = DateTime.UtcNow.AddYears (1);  // TODO - load duration from product/underwriting configuration
                    model.AdministrationFee = 20;
                    model.CurrencySymbol = "fa fa-dollar";

                    // 
                    if (answerSheet.Status == "Submitted") {
                        model.InformationSheetId = answerSheet.Id;
                    }

                    //Title = ;
                    ViewBag.Title = "Proposalonline - " + informationTemplate.Name + " Agreement for " + insured.FullName;

                    return View(model);
                }*/

        public ActionResult CreateDefaultAgreementRules()
        {
            Product product = _productRepository.FindAll().FirstOrDefault(p => p.IsBaseProduct == false);

            using (IUnitOfWork uow = _unitOfWorkFactory.BeginUnitOfWork())
            {
                if (product != null)
                {
                    //
                    _ruleRepository.Add(new Rule(CurrentUser, "NICITY1CRate", "North Island City Rate for Category 1C", product, "2.75") { OrderNumber = 5 });
                    _ruleRepository.Add(new Rule(CurrentUser, "NITOWN1CRate", "North Island Town Rate for Category 1C", product, "2.5") { OrderNumber = 6 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SICITY1CRate", "South Island City Rate for Category 1C", product, "2.225") { OrderNumber = 7 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SITOWN1CRate", "South Island Town Rate for Category 1C", product, "2") { OrderNumber = 8 });
                    ///
                    _ruleRepository.Add(new Rule(CurrentUser, "NICITY1UAPRate", "North Island City Rate for Category 1UAP", product, "4") { OrderNumber = 9 });
                    _ruleRepository.Add(new Rule(CurrentUser, "NITOWN1UAPRate", "North Island Town Rate for Category 1UAP", product, "4") { OrderNumber = 10 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SICITY1UAPRate", "South Island City Rate for Category 1UAP", product, "4") { OrderNumber = 11 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SITOWN1UAPRate", "South Island Town Rate for Category 1UAP", product, "4") { OrderNumber = 12 });
                    ///
                    _ruleRepository.Add(new Rule(CurrentUser, "NICITY1PRate", "North Island City Rate for Category 1P", product, "2") { OrderNumber = 13 });
                    _ruleRepository.Add(new Rule(CurrentUser, "NITOWN1PRate", "North Island Town Rate for Category 1P", product, "1.5") { OrderNumber = 14 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SICITY1PRate", "South Island City Rate for Category 1P", product, "1.5") { OrderNumber = 15 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SITOWN1PRate", "South Island Town Rate for Category 1P", product, "1") { OrderNumber = 16 });
                    ///
                    _ruleRepository.Add(new Rule(CurrentUser, "NICITY1RRate", "North Island City Rate for Category 1R", product, "4.75") { OrderNumber = 17 });
                    _ruleRepository.Add(new Rule(CurrentUser, "NITOWN1RRate", "North Island Town Rate for Category 1R", product, "4.75") { OrderNumber = 18 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SICITY1RRate", "South Island City Rate for Category 1R", product, "4.75") { OrderNumber = 19 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SITOWN1RRate", "South Island Town Rate for Category 1R", product, "4.75") { OrderNumber = 20 });
                    ///
                    _ruleRepository.Add(new Rule(CurrentUser, "NICITY2Rate", "North Island City Rate for Category 2", product, "1.5") { OrderNumber = 21 });
                    _ruleRepository.Add(new Rule(CurrentUser, "NITOWN2Rate", "North Island Town Rate for Category 2", product, "1.25") { OrderNumber = 22 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SICITY2Rate", "South Island City Rate for Category 2", product, "1.25") { OrderNumber = 23 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SITOWN2Rate", "South Island Town Rate for Category 2", product, "1") { OrderNumber = 24 });
                    ///
                    _ruleRepository.Add(new Rule(CurrentUser, "NICITY3Rate", "North Island City Rate for Category 3", product, "1.75") { OrderNumber = 25 });
                    _ruleRepository.Add(new Rule(CurrentUser, "NITOWN3Rate", "North Island Town Rate for Category 3", product, "1.25") { OrderNumber = 26 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SICITY3Rate", "South Island City Rate for Category 3", product, "1.25") { OrderNumber = 27 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SITOWN3Rate", "South Island Town Rate for Category 3", product, "1") { OrderNumber = 28 });
                    ///
                    _ruleRepository.Add(new Rule(CurrentUser, "NICITYSVRate", "North Island City Rate for Category SV", product, "0.25") { OrderNumber = 29 });
                    _ruleRepository.Add(new Rule(CurrentUser, "NITOWNSVRate", "North Island Town Rate for Category SV", product, "0.25") { OrderNumber = 30 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SICITYSVRate", "South Island City Rate for Category SV", product, "0.25") { OrderNumber = 31 });
                    _ruleRepository.Add(new Rule(CurrentUser, "SITOWNSVRate", "South Island Town Rate for Category SV", product, "0.25") { OrderNumber = 32 });
                    ///
                    _ruleRepository.Add(new Rule(CurrentUser, "FSLUNDERFee", "FSL Fee for Vehicle under 3.5T", product, "6.08") { OrderNumber = 33 });
                    _ruleRepository.Add(new Rule(CurrentUser, "FSLOVER3Rate", "FSL Rate for Vehicle over 3.5T", product, "0.076") { OrderNumber = 34 });
                    _ruleRepository.Add(new Rule(CurrentUser, "FSLUNDERFeeAfter1July", "FSL Fee for Vehicle under 3.5T After 1 July", product, "6.08") { OrderNumber = 35 });
                    _ruleRepository.Add(new Rule(CurrentUser, "FSLOVER3RateAfter1July", "FSL Rate for Vehicle over 3.5T After 1 July", product, "0.076") { OrderNumber = 36 });
                    _ruleRepository.Add(new Rule(CurrentUser, "PaymentPremium", "Premium Payment", product, "Monthly") { OrderNumber = 40 });

                    uow.Commit();
                }
            }

            return Redirect("~/Home/Index");
        }

        public ActionResult RenderDocuments(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));
            ClientInformationSheet answerSheet = _customerInformationService.GetInformation(id);
            if (answerSheet == null)
                throw new Exception(string.Format("RenderDocuments: No Answer Sheet found for [{0}]", id));
            ClientProgramme clientProgramme = answerSheet.Programme;
            if (clientProgramme == null)
                throw new Exception(string.Format("RenderDocuments: No Client Programme found for information sheet [{0}]", id));
            //ClientAgreement agreement = answerSheet.ClientAgreement;
            //if (agreement == null)
            //	throw new Exception (string.Format ("No Information found for {0}", id));

            foreach (ClientAgreement agreement in clientProgramme.Agreements)
            {
                foreach (Document doc in agreement.Documents.Where(d => d.DateDeleted == null))
                {
                    doc.Delete(CurrentUser);
                }

                foreach (Document template in agreement.Product.Documents)
                {
                    Document renderedDoc = _fileService.RenderDocument(CurrentUser, template, agreement);
                    renderedDoc.OwnerOrganisation = agreement.ClientInformationSheet.Owner;
                    agreement.Documents.Add(renderedDoc);
                    _fileService.UploadFile(renderedDoc);
                }
            }

            return Redirect("/Agreement/ViewAcceptedAgreement/" + clientProgramme.Id);
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