﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using SystemDocument = TechCertain.Domain.Entities.Document;

namespace TechCertain.Services.Interfaces
{
	public interface IEmailService
	{
		string EmailEnabled { get; }
		string SmtpServer { get; }
		int SmtpPort { get; }
		string DefaultSender { get; }
		string SystemEmail { get; }
		string CatchAllEmail { get; }
        string BCCEmail { get; }
        string ReplyToEmail { get; }
        Task SendPasswordResetEmail(string recipent, Guid resetToken, string originDomain);
        Task SendEmailViaEmailTemplate(string recipent, EmailTemplate emailTemplate, List<SystemDocument> documents);
        Task ContactSupport (string sender, string subject, string body);
        Task SendSystemEmailLogin(string recipent);
        Task SendSystemPaymentSuccessConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        Task SendSystemPaymentFailConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        Task SendSystemFailedInvoiceConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        Task SendSystemSuccessInvoiceConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        Task SendSystemEmailUISIssueNotify(User UISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        Task SendSystemEmailUISSubmissionConfirmationNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        Task SendSystemEmailUISSubmissionNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        Task SendSystemEmailAgreementReferNotify(User uISIssued, Programme programme, ClientAgreement agreement, Organisation insuredOrg);
        Task SendSystemEmailAgreementIssueNotify(User issuer, Programme programme, ClientAgreement agreement, Organisation insuredOrg);
        Task SendSystemEmailAgreementBoundNotify(User binder, Programme programme, ClientAgreement agreement, Organisation insuredOrg);
        Task SendSystemEmailOtherMarinaTCNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        Task SendSystemEmailEGlobalTCNotify(string XMLBody);
    }
}

