using System;
using System.Configuration;
using System.Net.Mail;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using SystemDocument = TechCertain.Domain.Entities.Document;
using System.Net.Mime;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TechCertain.Infrastructure.Email;
using HtmlToOpenXml;

namespace TechCertain.Services.Impl
{
	public class EmailService : IEmailService
	{
		IUserService _userService;		
        IFileService _fileService;
        ISystemEmailService _systemEmailRepository;
        IMapperSession<ClientInformationSheet> _clientInformationSheetMapper;

        public EmailService (IUserService userService, IFileService fileService, ISystemEmailService systemEmailService, IMapperSession<ClientInformationSheet> clientInformationSheetMapper)
		{
			_userService = userService;			
            _fileService = fileService;
            _systemEmailRepository = systemEmailService;
            _clientInformationSheetMapper = clientInformationSheetMapper;
        }

		#region IEmailService implementation

		public string EmailEnabled
		{
			get {
				return ConfigurationManager.AppSettings["EnableMail"];
			}
		}
		
		public string SmtpServer
		{
			get {
				return ConfigurationManager.AppSettings["SmtpServer"];
			}
		}

		public int SmtpPort
		{
			get {
				return Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
			}
		}

		public string DefaultSender
		{
			get {
				return ConfigurationManager.AppSettings["SenderEmail"];
			}
		}

		public string SystemEmail
		{
			get {
				return DefaultSender;
			}
		}

		public string CatchAllEmail
		{
			get {
				return ConfigurationManager.AppSettings["CatchAllEmail"];
			}
		}

		//public void SendEmail(string recipient, string subject, string body, List<SystemDocument> documents, string sender = "", void saveCopy = true, void isHtml = true)
		//{
		//	// if there is a catch email address, send all emails there, otherwise send to specified recipient
		//	recipient = !string.IsNullOrWhiteSpace (CatchAllEmail) ? CatchAllEmail : recipient;
		//	// create email
		//	if (string.IsNullOrWhiteSpace (sender))
		//		sender = DefaultSender;
		//	MailMessage mail = new MailMessage (sender, recipient);
		//	mail.Subject = subject;
		//	mail.Body = body;
		//	mail.IsBodyHtml = isHtml;
		//	if (saveCopy && string.IsNullOrWhiteSpace(CatchAllEmail))
		//		mail.Bcc.Add (SystemEmail);
                      
  //          if (documents != null)
  //          {
  //              foreach (SystemDocument doc in documents)
  //              {
  //                  if (doc.ContentType == MediaTypeNames.Text.Html)
  //                  {
		//				// Testing HtmlToOpenXml
		//				string html = _fileService.FromBytes(doc.Contents);
		//				using (MemoryStream virtualFile = new MemoryStream())
		//				{
		//				    using (WordprocessingDocument wordDocument = WordprocessingDocument.Create (virtualFile, WordprocessingDocumentType.Document))
		//				    {
		//				    // Add a main document part. 
		//				        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
		//				        new DocumentFormat.OpenXml.Wordprocessing.Document(new Body()).Save(mainPart);
		//				        HtmlConverter converter = new HtmlConverter(mainPart);
		//				        converter.ImageProcessing = ImageProcessing.AutomaticDownload;
		//				        converter.ParseHtml(html);
		//				    }
		//				    var attachement = new Attachment(new MemoryStream(virtualFile.ToArray()), doc.Name + ".docx");
		//				    mail.Attachments.Add(attachement);
		//				}
  //                  }
  //              }
  //          }

		//	using (SmtpClient client = new SmtpClient ()) {
		//		client.Port = SmtpPort;
		//		client.DeliveryMethod = SmtpDeliveryMethod.Network;
		//		client.UseDefaultCredentials = false;
		//		client.Host = SmtpServer;
		//		client.Send (mail);
		//	}

		//	return true;
		//}

		//public void SendEmail(string recipient, string subject, string body)
		//{
		//	return SendEmail (recipient, subject, body, true);
		//}

		//public void SendEmail(string recipient, string subject, string body, void saveCopy)
		//{
		//	return SendEmail (recipient, DefaultSender, subject, body, saveCopy);
		//}

		//public void SendEmail(string recipient, string sender, string subject, string body)
		//{
		//	return SendEmail (recipient, sender, subject, body, true);
		//}

		//public void SendEmail(string recipient, string sender, string subject, string body, void saveCopy)
		//{
		//	return SendEmail (recipient, subject, body, null, sender, saveCopy, true);
		//}

		public void SendPasswordResetEmail (string recipent, Guid resetToken, string originDomain)
		{
			var user = _userService.GetUserByEmail (recipent);

			// hard code the body for now, should be stored in the db
			string body = "<p>Hi There,</p>";
			body += "<p>We've recently had someone request a password reset for your Proposalonline account. " +
				"If this was you, please click the following link to reset your password.<br/>";
			body += "<p>If you didn't request a password reset, please ignore and delete this email.</p>";
			body += string.Format("<a href='{0}/Account/ChangePassword/{1}'>Reset my password.</a>", originDomain, resetToken);
			body += "<p>If the above link doesn't work, copy and paste the following into your browser</p>";
			body += "<p>This link will expire in 24 hours for security reasons.</p>";
			body += string.Format("{0}/Account/ChangePassword/{1}", originDomain, resetToken);
			body += string.Format("<p>Your username is <em>{0}</em></p>", user.UserName);
			body += "<p>Thanks<br/>- The Proposalonline Team</p>";
			body += "<p>Proposalonline is technology of TechCertain Group Limited who provide technical support.</p>";
			body += string.Format("<p>Proposalonline login: {0}<br/>Email support: support@techcertain.com<br/>Telephone support: 09 377 6564 (9am to 5pm NZST)</p>", originDomain);

			EmailBuilder email = GetLocalizedEmailBuilder (DefaultSender, recipent);
			email.From (DefaultSender);
			email.WithSubject ("Proposalonline Password Reset");
			email.WithBody (body);
			email.UseHtmlBody (true);
			email.Send ();
		}

        public void SendEmailViaEmailTemplate(string recipent, EmailTemplate emailTemplate, List<SystemDocument> documents)
        {
            //string subject = emailTemplate.Subject;
            string body = System.Net.WebUtility.HtmlDecode(emailTemplate.Body);

			EmailBuilder email = GetLocalizedEmailBuilder (DefaultSender, recipent);
			email.From (DefaultSender);
			email.WithSubject (emailTemplate.Subject);
			email.WithBody (body);
			email.UseHtmlBody (true);
            if(documents != null)
            {
                email.Attachments(ToAttachments(documents).ToArray());
            }
			email.Send ();
        }

        public void ContactSupport (string sender, string subject, string body)
		{
			string subjectPrefix = "Proposalonline Support Request: ";

			EmailBuilder email = GetLocalizedEmailBuilder (sender, "support@techcertain.com");
			email.From (sender);
            email.WithSubject (subjectPrefix + subject);
            email.WithBody (body);
			email.UseHtmlBody (true);
			email.Send ();
		}

        public void SendSystemEmailLogin(string recipent)
        {
            var user = _userService.GetUserByEmail(recipent);

            List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
            mergeFields.Add(new KeyValuePair<string, string>("[[UserName]]", user.UserName));
            mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));
            
            SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("LoginEmail");
            string systememailsubject = systemEmailTemplate.Subject;
            string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
            foreach (KeyValuePair<string, string> field in mergeFields)
            {
                systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                systememailbody = systememailbody.Replace(field.Key, field.Value);
            }
            EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, recipent);
            systememail.From(DefaultSender);
            systememail.WithSubject (systememailsubject);
            systememail.WithBody (systememailbody);
            systememail.UseHtmlBody (true);
            systememail.Send();
        }

        public void SendSystemPaymentSuccessConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.PaymentConfigNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.PaymentConfigNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuer]]", uISIssuer.FullName));
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuerEmail]]", uISIssuer.Email));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("PaymentSuccessConfig");
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssuer, sheet, null, "Payment success config Notification Sent"));
                _clientInformationSheetMapper.UpdateAsync(sheet);
            }
        }

        public void SendSystemPaymentFailConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.PaymentConfigNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.PaymentConfigNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuer]]", uISIssuer.FullName));
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuerEmail]]", uISIssuer.Email));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("PaymentFailConfig");
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssuer, sheet, null, "Payment success config Notification Sent"));
                _clientInformationSheetMapper.UpdateAsync(sheet);
            }

        }

        public void SendSystemFailedInvoiceConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.InvoiceConfigNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.InvoiceConfigNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuer]]", uISIssuer.FullName));
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuerEmail]]", uISIssuer.Email));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("InvoiceFailConfig");
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssuer, sheet, null, "Payment success config Notification Sent"));
                _clientInformationSheetMapper.UpdateAsync(sheet);

            }
        }

        public void SendSystemSuccessInvoiceConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.InvoiceConfigNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.InvoiceConfigNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuer]]", uISIssuer.FullName));
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuerEmail]]", uISIssuer.Email));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("InvoiceSuccessConfig");
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssuer, sheet, null, "Payment success config Notification Sent"));
                _clientInformationSheetMapper.UpdateAsync(sheet);
            }

        }

        public void SendSystemEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.UISIssueNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.UISIssueNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);                    
                }

                List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuer]]", uISIssuer.FullName));
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuerEmail]]", uISIssuer.Email));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("UISIssueNotificationEmail");
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();


                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssuer, sheet, null, "Payment success config Notification Sent"));
                _clientInformationSheetMapper.UpdateAsync(sheet);
            }

        }

        public void SendSystemEmailUISSubmissionConfirmationNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (uISIssued != null)
            {
                recipent.Add(insuredOrg.Email);

                List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("UISSubmissionConfirmationEmail");
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From("postmaster@techcertain.com");
                //systememail.From(DefaultSender);
                //systememail.To(recipent.ToArray());
                systememail.To("postmaster@techcertain.com");
                if (programme.ProgrammeEmailCCToBroker)
                {
                    //systememail.CC(sheet.Programme.BrokerContactUser.Email);
                }
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssued, sheet, null, "Information Sheet Submission Confirmation Sent"));
                _clientInformationSheetMapper.UpdateAsync(sheet);
            }
        }

        public void SendSystemEmailUISSubmissionNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.UISSubmissionNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.UISSubmissionNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("UISSubmissionNotificationEmail");
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssued, sheet, null, "Information Sheet Submission Confirmation Sent"));
                _clientInformationSheetMapper.UpdateAsync(sheet);
            }
        }

        public void SendSystemEmailAgreementReferNotify(User uISIssued, Programme programme, ClientAgreement agreement, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.AgreementReferNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.AgreementReferNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[ContactBrokerName]]", programme.BrokerContactUser.FullName));
                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("AgreementReferralNotificationEmail");
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                ClientInformationSheet sheet = agreement.ClientInformationSheet;

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssued, sheet, null, "Information Sheet Submission Confirmation Sent"));
                _clientInformationSheetMapper.UpdateAsync(sheet);
            }
        }

        public void SendSystemEmailAgreementIssueNotify(User issuer, Programme programme, ClientAgreement agreement, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.AgreementIssueNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.AgreementIssueNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[ContactBrokerName]]", programme.BrokerContactUser.FullName));
                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("AgreementIssueNotificationEmail");
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                ClientInformationSheet sheet = agreement.ClientInformationSheet;

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(issuer, sheet, agreement, "Agreement Issue Notification Sent"));
                _clientInformationSheetMapper.UpdateAsync(sheet);
            }
        }


        public void SendSystemEmailAgreementBoundNotify(User binder, Programme programme, ClientAgreement agreement, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.AgreementBoundNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.AgreementBoundNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[ContactBrokerName]]", programme.BrokerContactUser.FullName));
                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("AgreementBoundNotificationEmail");
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                ClientInformationSheet sheet = agreement.ClientInformationSheet;

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(binder, sheet, agreement, "Agreement Bound Notification Sent"));
                _clientInformationSheetMapper.UpdateAsync(sheet);

            }
        }

        public void SendSystemEmailOtherMarinaTCNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (uISIssued != null)
            {
                recipent.Add("support@techcertain.com");

                List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[ReferenceID]]", sheet.ReferenceId));
                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = _systemEmailRepository.GetSystemEmailByType("OtherMarinaTCNotifyEmail");
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                if (programme.ProgrammeEmailCCToBroker)
                {
                    systememail.CC(sheet.Programme.BrokerContactUser.Email);
                }
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssued, sheet, null, "Create Other Marina Notification Email to TC Sent"));
                _clientInformationSheetMapper.UpdateAsync(sheet);

            }
        }

        public void SendSystemEmailEGlobalTCNotify(string XMLBody)
        {
                       
            EmailBuilder systememail = GetLocalizedEmailBuilder(DefaultSender, null);
            systememail.From(DefaultSender);
            systememail.WithSubject("ProposalOnline System Event");
            systememail.WithBody(XMLBody);
            systememail.UseHtmlBody(true);
            systememail.Send();

        }

        #endregion

        /// <summary>
        /// Returns an instance of the EmailBuilder configured with server settings
        /// </summary>
        /// <returns>The localized email builder.</returns>
        /// <param name="defaultSender">Default sender.</param>
        /// <param name="recipient">Recipient.</param>
        EmailBuilder GetLocalizedEmailBuilder (string defaultSender, string recipient)
		{
			EmailBuilder email = new EmailBuilder (DefaultSender);
			if (string.IsNullOrWhiteSpace(CatchAllEmail))
            {
                if (recipient != null)
                {
                    email.To(recipient).BCC(SystemEmail);
                }
            }
            else
            {
                email.To(CatchAllEmail);
            }
			return email;
		}

		Attachment ToAttachment (SystemDocument document)
		{
			if (document.ContentType == MediaTypeNames.Text.Html) {
				// Testing HtmlToOpenXml
				string html = _fileService.FromBytes (document.Contents);
				using (MemoryStream virtualFile = new MemoryStream ()) {
					using (WordprocessingDocument wordDocument = WordprocessingDocument.Create (virtualFile, WordprocessingDocumentType.Document)) {
						// Add a main document part. 
						MainDocumentPart mainPart = wordDocument.AddMainDocumentPart ();
						new DocumentFormat.OpenXml.Wordprocessing.Document (new Body ()).Save (mainPart);
						HtmlConverter converter = new HtmlConverter (mainPart);
						converter.ImageProcessing = ImageProcessing.AutomaticDownload;
						converter.ParseHtml (html);
					}
					//var attachment = new Attachment (new MemoryStream (virtualFile.ToArray ()), document.Name + ".docx");
					return new Attachment (new MemoryStream (virtualFile.ToArray ()), document.Name + ".docx");
				}
			}
			return null;
		}

		IEnumerable<Attachment> ToAttachments (IEnumerable<SystemDocument> documents)
		{
			IList<Attachment> attachments = new List<Attachment> ();
			foreach (SystemDocument document in documents)
				attachments.Add(ToAttachment (document));
			return attachments;
		}
	}
}

