using System;
using System.Collections.Generic;
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

		//bool SendEmail(string recipient, string subject, string body, List<SystemDocument> documents, string sender = "", bool saveCopy = true, bool isHtml = true);

		//bool SendEmail(string recipient, string subject, string body);

		//bool SendEmail(string recipient, string subject, string body, bool saveCopy);

		//bool SendEmail(string recipient, string sender, string subject, string body);

		//bool SendEmail(string recipient, string sender, string subject, string body, bool saveCopy);

		bool SendPasswordResetEmail(string recipent, Guid resetToken, string originDomain);

        bool SendEmailViaEmailTemplate(string recipent, EmailTemplate emailTemplate, List<SystemDocument> documents);

        bool ContactSupport (string sender, string subject, string body);

        bool SendSystemEmailLogin(string recipent);
        bool SendSystemPaymentSuccessConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        bool SendSystemPaymentFailConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        bool SendSystemFailedInvoiceConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        bool SendSystemSuccessInvoiceConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        bool SendSystemEmailUISIssueNotify(User UISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        bool SendSystemEmailUISSubmissionConfirmationNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        bool SendSystemEmailUISSubmissionNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        bool SendSystemEmailAgreementReferNotify(User uISIssued, Programme programme, ClientAgreement agreement, Organisation insuredOrg);
        bool SendSystemEmailAgreementIssueNotify(User issuer, Programme programme, ClientAgreement agreement, Organisation insuredOrg);
        bool SendSystemEmailAgreementBoundNotify(User binder, Programme programme, ClientAgreement agreement, Organisation insuredOrg);
        bool SendSystemEmailOtherMarinaTCNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        bool SendSystemEmailEGlobalTCNotify(string XMLBody);

    }
}

