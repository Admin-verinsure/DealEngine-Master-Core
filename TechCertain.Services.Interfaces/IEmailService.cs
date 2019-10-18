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
		void SendPasswordResetEmail(string recipent, Guid resetToken, string originDomain);

        void SendEmailViaEmailTemplate(string recipent, EmailTemplate emailTemplate, List<SystemDocument> documents);

        void ContactSupport (string sender, string subject, string body);

        void SendSystemEmailLogin(string recipent);
        void SendSystemPaymentSuccessConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        void SendSystemPaymentFailConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        void SendSystemFailedInvoiceConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        void SendSystemSuccessInvoiceConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        void SendSystemEmailUISIssueNotify(User UISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        void SendSystemEmailUISSubmissionConfirmationNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        void SendSystemEmailUISSubmissionNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        void SendSystemEmailAgreementReferNotify(User uISIssued, Programme programme, ClientAgreement agreement, Organisation insuredOrg);
        void SendSystemEmailAgreementIssueNotify(User issuer, Programme programme, ClientAgreement agreement, Organisation insuredOrg);
        void SendSystemEmailAgreementBoundNotify(User binder, Programme programme, ClientAgreement agreement, Organisation insuredOrg);
        void SendSystemEmailOtherMarinaTCNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg);
        void SendSystemEmailEGlobalTCNotify(string XMLBody);

    }
}

