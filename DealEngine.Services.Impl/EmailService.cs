using System;
using System.Net.Mail;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using DealEngine.Domain.Entities;
using SystemDocument = DealEngine.Domain.Entities.Document;
using File = System.IO.File;
using System.Net.Mime;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DealEngine.Infrastructure.Email;
using HtmlToOpenXml;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DealEngine.Services.Impl
{
	public class EmailService : IEmailService
	{
		IUserService _userService;		
        IFileService _fileService;
        ISystemEmailService _systemEmailRepository;
        IMapperSession<ClientInformationSheet> _clientInformationSheetmapperSession;
        IAppSettingService _appSettingService;
        private IConfiguration _configuration { get; set; }

        public EmailService (IUserService userService, IFileService fileService, ISystemEmailService systemEmailService, IConfiguration configuration, IMapperSession<ClientInformationSheet> clientInformationSheetmapperSession, IAppSettingService appSettingService)
		{
            _clientInformationSheetmapperSession = clientInformationSheetmapperSession;
            _userService = userService;			
            _fileService = fileService;
            _systemEmailRepository = systemEmailService;
            _configuration = configuration;
            _appSettingService = appSettingService;
        }

		#region IEmailService implementation

		public string EmailEnabled
		{
			get {
                return _configuration.GetValue<string>("EnableMail");
            }
		}

        public string SmtpServer
		{
			get {
                return _configuration.GetValue<string>("SmtpServer");
            }
		}

		public int SmtpPort
		{
			get {
                return _configuration.GetValue<int>("SmtpPort");
            }
		}

		public string DefaultSender
		{
			get {
                return _configuration.GetValue<string>("SenderEmail");
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
                return _configuration.GetValue<string>("CatchAllEmail");
            }
		}

        public string BCCEmail
        {
            get
            {
                return _configuration.GetValue<string>("BCCEmail");
            }
        }
        public string ReplyToEmail
        {
            get
            {
                return _configuration.GetValue<string>("ReplyToEmail");
            }
        }

        public async Task SendPasswordResetEmail (string recipent, Guid resetToken, string originDomain)
		{
			var user = await _userService.GetUserByEmail(recipent);

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
			body += "<p>Proposalonline is technology of DealEngine Group Limited who provide technical support.</p>";
			body += string.Format("<p>Proposalonline login: {0}<br/>Email support: support@techcertain.com<br/>Telephone support: 09 377 6564 (9am to 5pm NZST)</p>", originDomain);

			EmailBuilder email = await GetLocalizedEmailBuilder(DefaultSender, recipent);
			email.From (DefaultSender);
            email.WithSubject ("Deal Engine Password Reset");
			email.WithBody (body);
			email.UseHtmlBody (true);
            
            /*         
            Guid newGuid = Guid.Parse("85578a21-d383-45fb-8e71-aba300363f4b");
            SystemDocument test = await _fileService.GetDocumentByID(newGuid);
            var testPath = test.Path;
            //if (File.Exists(testPath))
            //{
            //    FileStream fileStream = File.OpenRead(testPath);
            //}
            //Attachment document = new Attachment(new MemoryStream(fileBytes), file.FileName);         
            You can try this multi-step approach.
            //First create FileContent
            FileContentResult fileContent = File(fileName, "application/pdf", "file.pdf");
            MemoryStream ms = new MemoryStream(fileContent.FileContents); 
            // Create an in-memory System.IO.Stream
            ContentType ct = new ContentType(fileContent.ContentType);
            Attachment a = new Attachment(ms, ct);
            sender.SendMail("email", "email", "subject", "Body", a);                     
            email.Attachments();
            */

            email.Send ();
		}

        public async Task SendEmailViaEmailTemplate(string recipent, EmailTemplate emailTemplate, List<SystemDocument> documents, ClientInformationSheet clientInformationSheet, ClientAgreement clientAgreement)
        {
            var user = await _userService.GetUserByEmail(recipent);
            List<KeyValuePair<string, string>> mergeFields;

            if (clientInformationSheet != null)
            {
                if (clientAgreement != null)
                {
                    mergeFields = MergeFieldLibrary(null, null, clientInformationSheet.Programme.BaseProgramme, clientInformationSheet, clientAgreement);
                }
                else
                {
                    mergeFields = MergeFieldLibrary(null, null, clientInformationSheet.Programme.BaseProgramme, clientInformationSheet, null);
                }

            } 
            else
            {
                mergeFields = MergeFieldLibrary(null, null, null, clientInformationSheet, null);
            }
            string systememailsubject = emailTemplate.Subject;
            string systememailbody = System.Net.WebUtility.HtmlDecode(emailTemplate.Body);
            foreach (KeyValuePair<string, string> field in mergeFields)
            {
                systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                systememailbody = systememailbody.Replace(field.Key, field.Value);
            }            

			EmailBuilder email = await GetLocalizedEmailBuilder(DefaultSender, recipent);
			email.From (DefaultSender);
            email.WithSubject (systememailsubject);
			email.WithBody (systememailbody);
			email.UseHtmlBody (true);
            if(documents != null)
            {
                var documentsList = await ToAttachments(documents);
                email.Attachments(documentsList.ToArray());
            }
			email.Send ();
        }


        public async Task IssueToBrokerSendEmail(string recipent, string EmailContent ,  ClientInformationSheet clientInformationSheet, ClientAgreement clientAgreement, User sender)
        {
            var user = await _userService.GetUserByEmail(recipent);
            List<KeyValuePair<string, string>> mergeFields;

            if (clientInformationSheet != null)
            {
                if (clientAgreement != null)
                {
                    mergeFields = MergeFieldLibrary(null, null, clientInformationSheet.Programme.BaseProgramme, clientInformationSheet, clientAgreement);
                }
                else
                {
                    mergeFields = MergeFieldLibrary(null, null, clientInformationSheet.Programme.BaseProgramme, clientInformationSheet, null);
                }

            }
            else
            {
                mergeFields = MergeFieldLibrary(null, null, null, clientInformationSheet, null);
            }
            string systememailsubject = "Issue to Broker Email (reference: " + clientInformationSheet.ReferenceId + ")";
            string systememailbody = System.Net.WebUtility.HtmlDecode(EmailContent);
            foreach (KeyValuePair<string, string> field in mergeFields)
            {
                systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                systememailbody = systememailbody.Replace(field.Key, field.Value);
            }

            EmailBuilder email = await GetLocalizedEmailBuilder(DefaultSender, recipent);
            email.From(DefaultSender);
            email.ReplyTo(sender.Email);
            email.WithSubject(systememailsubject);
            email.WithBody(systememailbody);
            email.UseHtmlBody(true);          
            email.Send();
        }


        public async Task ContactSupport (string sender, string subject, string body)
		{
			string subjectPrefix = "Support Request: ";

			EmailBuilder email = await GetLocalizedEmailBuilder(sender, "support@techcertain.com");
			email.From (sender);
            email.WithSubject (subjectPrefix + subject);
            email.WithBody (body);
			email.UseHtmlBody (true);
			email.Send ();
		}

        public async Task MarshPleaseCallMe(string recipient, string subject, string body)
        {
            string subjectPrefix = "Quick Quote Please Call Request: ";

            EmailBuilder email = await GetLocalizedEmailBuilder(DefaultSender, recipient);
            email.From(DefaultSender);
            email.WithSubject(subjectPrefix + subject);
            email.WithBody(body);
            email.UseHtmlBody(true);
            email.Send();
        }

        public async Task MarshRsaOneTimePassword(string recipient, string subject)
        {
            string subjectPrefix = "One Time Password: ";

            EmailBuilder email = await GetLocalizedEmailBuilder(DefaultSender, recipient);
            email.From(DefaultSender);
            email.WithSubject(subjectPrefix + subject);            
            email.UseHtmlBody(true);
            email.Send();
        }

        public async Task SendSystemEmailLogin(string recipent)
        {
            var user = await _userService.GetUserByEmail(recipent);

            List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();
            mergeFields.Add(new KeyValuePair<string, string>("[[UserName]]", user.UserName));
            mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));
            mergeFields.Add(new KeyValuePair<string, string>("[[SupportEmail]]", "support@techcertain.com"));

            SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("LoginEmail");
            if (systemEmailTemplate == null)
            {
                throw new Exception("LoginEmail is null");
            }
            string systememailsubject = systemEmailTemplate.Subject;
            string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
            foreach (KeyValuePair<string, string> field in mergeFields)
            {
                systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                systememailbody = systememailbody.Replace(field.Key, field.Value);
            }
            EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, recipent);
            systememail.From(DefaultSender);
            systememail.WithSubject (systememailsubject);
            systememail.WithBody (systememailbody);
            systememail.UseHtmlBody (true);
            systememail.Send();
        }

        public async Task SendSystemPaymentSuccessConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.PaymentConfigNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.PaymentConfigNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(uISIssuer, insuredOrg, programme, sheet, null);

                SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("PaymentSuccessConfig");
                if (systemEmailTemplate == null)
                {
                    throw new Exception("PaymentSuccessConfig is null");
                }
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssuer, sheet, null, "Payment success config Notification Sent"));
                await _clientInformationSheetmapperSession.UpdateAsync(sheet);
            }
        }

        public async Task SendSystemPaymentFailConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.PaymentConfigNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.PaymentConfigNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(uISIssuer, insuredOrg, programme, sheet, null);

                SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("PaymentFailConfig");
                if (systemEmailTemplate == null)
                {
                    throw new Exception("PaymentFailConfig is null");
                }
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssuer, sheet, null, "Payment success config Notification Sent"));
                await _clientInformationSheetmapperSession.UpdateAsync(sheet);
            }

        }

        public async Task SendSystemFailedInvoiceConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.InvoiceConfigNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.InvoiceConfigNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(uISIssuer, insuredOrg, programme, sheet, null);


                SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("InvoiceFailConfig");
                if (systemEmailTemplate == null)
                {
                    throw new Exception("InvoiceFailConfig is null");
                }
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssuer, sheet, null, "Payment success config Notification Sent"));
                await _clientInformationSheetmapperSession.UpdateAsync(sheet);

            }
        }

        public async Task SendSystemSuccessInvoiceConfigEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.InvoiceConfigNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.InvoiceConfigNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(uISIssuer, insuredOrg, programme, sheet, null);

                SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("InvoiceSuccessConfig");
                if (systemEmailTemplate == null)
                {
                    throw new Exception("InvoiceSuccessConfig is null");
                }
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssuer, sheet, null, "Payment success config Notification Sent"));
                await _clientInformationSheetmapperSession.UpdateAsync(sheet);
            }

        }

        public async Task SendSystemEmailAllSubUISInstruction(Organisation insuredOrg, Programme programme, ClientInformationSheet sheet)
        {
            var recipent = new List<string>();
            recipent.Add(insuredOrg.Email);

            List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(null, insuredOrg, programme, sheet, null);

            EmailTemplate systemEmailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == "SendSubInformationSheetInstruction");
            if (systemEmailTemplate == null)
            {
                throw new Exception("SendSubInformationSheetInstruction is null");
            }
            string systememailsubject = systemEmailTemplate.Subject;
            string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
            foreach (KeyValuePair<string, string> field in mergeFields)
            {
                systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                systememailbody = systememailbody.Replace(field.Key, field.Value);
            }
            EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
            systememail.From(DefaultSender);
            systememail.To(recipent.ToArray());
            systememail.WithSubject(systememailsubject);
            systememail.WithBody(systememailbody);
            systememail.UseHtmlBody(true);
            systememail.Send();
        }

        public async Task SendSystemEmailAllSubUISComplete(Organisation insuredOrg, Programme programme, ClientInformationSheet sheet)
        {
            var recipent = new List<string>();

            recipent.Add(insuredOrg.Email);

            List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(null, insuredOrg, programme, sheet, null);

            EmailTemplate systemEmailTemplate = programme.EmailTemplates.FirstOrDefault(et => et.Type == "SendSubInformationSheetCompletion");
            if (systemEmailTemplate == null)
            {
                throw new Exception("SendSubInformationSheetCompletion is null");
            }
            string systememailsubject = systemEmailTemplate.Subject;
            string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
            foreach (KeyValuePair<string, string> field in mergeFields)
            {
                systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                systememailbody = systememailbody.Replace(field.Key, field.Value);
            }
            EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
            systememail.From(DefaultSender);
            systememail.To(recipent.ToArray());
            systememail.WithSubject(systememailsubject);
            systememail.WithBody(systememailbody);
            systememail.UseHtmlBody(true);
            systememail.Send();
        }

        public async Task SendSystemEmailUISIssueNotify(User uISIssuer, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.UISIssueNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.UISIssueNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);                    
                }

                List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(uISIssuer, insuredOrg, programme, sheet, null);

                SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("UISIssueNotificationEmail");
                if (systemEmailTemplate == null)
                {
                    throw new Exception("UISIssueNotificationEmail is null");
                }
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();


                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssuer, sheet, null, "Payment success config Notification Sent"));
                await _clientInformationSheetmapperSession.UpdateAsync(sheet);
            }

        }

        public async Task SendSystemEmailUISSubmissionConfirmationNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (uISIssued != null)
            {
                recipent.Add(insuredOrg.Email);

                List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(null, insuredOrg, programme, sheet, null);

                SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("UISSubmissionConfirmationEmail");
                if (systemEmailTemplate == null)
                {
                    throw new Exception("UISSubmissionConfirmationEmail is null");
                }
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssued, sheet, null, "Information Sheet Submission Confirmation Sent"));
                await _clientInformationSheetmapperSession.UpdateAsync(sheet);
            }
        }

        public async Task SendSystemEmailUISSubmissionNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.UISSubmissionNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.UISSubmissionNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(uISIssued, insuredOrg, programme, sheet, null);

                SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("UISSubmissionNotificationEmail");
                if (systemEmailTemplate == null)
                {
                    throw new Exception("UISSubmissionNotificationEmail is null");
                }
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssued, sheet, null, "Information Sheet Submission Confirmation Sent"));
                await _clientInformationSheetmapperSession.UpdateAsync(sheet);
            }
        }

        public async Task SendSystemEmailAgreementReferNotify(User uISIssued, Programme programme, ClientAgreement agreement, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.AgreementReferNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.AgreementReferNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(uISIssued, insuredOrg, programme, agreement.ClientInformationSheet, null);

                SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("AgreementReferralNotificationEmail");
                if (systemEmailTemplate == null)
                {
                    throw new Exception("AgreementReferralNotificationEmail is null");
                }
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                ClientInformationSheet sheet = agreement.ClientInformationSheet;

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssued, sheet, null, "Information Sheet Submission Confirmation Sent"));
                await _clientInformationSheetmapperSession.UpdateAsync(sheet);
            }
        }

        public async Task SendSystemEmailAgreementIssueNotify(User issuer, Programme programme, ClientAgreement agreement, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.AgreementIssueNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.AgreementIssueNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(null, insuredOrg, programme, null, null);

                SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("AgreementIssueNotificationEmail");
                if (systemEmailTemplate == null)
                {
                    throw new Exception("AgreementIssueNotificationEmail is null");
                }
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                ClientInformationSheet sheet = agreement.ClientInformationSheet;

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(issuer, sheet, agreement, "Agreement Issue Notification Sent"));
                await _clientInformationSheetmapperSession.UpdateAsync(sheet);
            }
        }


        public async Task SendSystemEmailAgreementBoundNotify(User binder, Programme programme, ClientAgreement agreement, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (programme.AgreementBoundNotifyUsers.Count > 0)
            {
                foreach (User objNotifyUser in programme.AgreementBoundNotifyUsers)
                {
                    recipent.Add(objNotifyUser.Email);
                }

                List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(binder, insuredOrg, programme, agreement.ClientInformationSheet, null);

                mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

                SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("AgreementBoundNotificationEmail");
                if (systemEmailTemplate == null)
                {
                    throw new Exception("AgreementBoundNotificationEmail is null");
                }
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                ClientInformationSheet sheet = agreement.ClientInformationSheet;

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(binder, sheet, agreement, "Agreement Bound Notification Sent"));
                await _clientInformationSheetmapperSession.UpdateAsync(sheet);

            }
        }

        public async Task SendSystemEmailOtherMarinaTCNotify(User uISIssued, Programme programme, ClientInformationSheet sheet, Organisation insuredOrg)
        {
            var recipent = new List<string>();

            if (uISIssued != null)
            {
                recipent.Add("support@techcertain.com");

                List<KeyValuePair<string, string>> mergeFields = MergeFieldLibrary(null, insuredOrg, programme, sheet, null);

                SystemEmail systemEmailTemplate = await _systemEmailRepository.GetSystemEmailByType("OtherMarinaTCNotifyEmail");
                if (systemEmailTemplate == null)
                {
                    throw new Exception("OtherMarinaTCNotifyEmail is null");
                }
                string systememailsubject = systemEmailTemplate.Subject;
                string systememailbody = System.Net.WebUtility.HtmlDecode(systemEmailTemplate.Body);
                foreach (KeyValuePair<string, string> field in mergeFields)
                {
                    systememailsubject = systememailsubject.Replace(field.Key, field.Value);
                    systememailbody = systememailbody.Replace(field.Key, field.Value);
                }
                EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
                systememail.From(DefaultSender);
                systememail.To(recipent.ToArray());
                if (programme.ProgrammeEmailCCToBroker && programme.BrokerContactUser != null)
                {
                    //systememail.CC(sheet.Programme.BrokerContactUser.Email);
                }
                systememail.WithSubject(systememailsubject);
                systememail.WithBody(systememailbody);
                systememail.UseHtmlBody(true);
                systememail.Send();

                sheet.ClientInformationSheetAuditLogs.Add(new AuditLog(uISIssued, sheet, null, "Create Other Marina Notification Email to TC Sent"));
                await _clientInformationSheetmapperSession.UpdateAsync(sheet);

            }
        }

        public async Task SendSystemEmailEGlobalTCNotify(string XMLBody)
        {
                       
            EmailBuilder systememail = await GetLocalizedEmailBuilder(DefaultSender, null);
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
        public async Task<EmailBuilder> GetLocalizedEmailBuilder (string defaultSender, string recipient)
		{
            EmailBuilder email = new EmailBuilder(DefaultSender);
            //EmailBuilder email = new EmailBuilder (DefaultSender);
            if (string.IsNullOrWhiteSpace(CatchAllEmail))
            {
                if (!string.IsNullOrWhiteSpace(recipient))
                {                    
                    email.To(recipient);
                    if(!string.IsNullOrWhiteSpace(BCCEmail))
                    {
                        email.BCC(BCCEmail);
                    }
                    //email.To(recipient).BCC(SystemEmail);
                    if (!string.IsNullOrWhiteSpace(ReplyToEmail))
                    {
                        email.ReplyTo(ReplyToEmail);
                    }
                }
            }
            else
            {
                email.To(CatchAllEmail);
            }
			return email;
		}

        public async Task<Attachment> ToAttachment (SystemDocument document)
		{
			if (document.ContentType == MediaTypeNames.Text.Html) {
				// Testing HtmlToOpenXml
				string html = _fileService.FromBytes (document.Contents);
				using (MemoryStream virtualFile = new MemoryStream ()) {
					using (WordprocessingDocument wordDocument = WordprocessingDocument.Create (virtualFile, WordprocessingDocumentType.Document)) {
						// Add a main document part. 
						MainDocumentPart mainPart = wordDocument.AddMainDocumentPart ();
						new DocumentFormat.OpenXml.Wordprocessing.Document (new Body ()).Save (mainPart);

                        //================
                        //document override
                        string showBorder = "<figure class=\"table\"><table style=\"border-bottom:solid;border-left:solid;border-right:solid;border-top:solid;\"><tbody><tr>";
                        string noBorder = "<figure class=\"table\"><table><tbody><tr>";

                        // Create document with a "main part" to it. No data has been added yet.
                        if (html.Contains(showBorder))
                        {
                            html = html.Replace(showBorder, "<table e border=\"1\"><tbody><tr>");
                            // NEED TO DO CLOSING TAGS TOO      width=\"100%\" align=\"center\"     <tr style=\"font-weight:bold\">
                        }
                        if (html.Contains(noBorder))
                        {
                            html = html.Replace(noBorder, "<table border=\"0\"><tbody><tr>");
                            // NEED TO DO CLOSING TAGS TOO      width=\"100%\" align=\"center\"     <tr style=\"font-weight:bold\">
                        }
                        string pathurl = "https://" + _appSettingService.domainQueryString + "/images";
                        string oldpath = "<img src=\"../../../images";
                        string newpath = "<p style=\"margin-left:36.0pt; text-align:center;\"/><img  height ='100' width='100' src=\"" + pathurl;
                        if (html.Contains(oldpath))
                        {
                            html = html.Replace(oldpath, newpath);
                        }
                        //================

                        HtmlConverter converter = new HtmlConverter (mainPart);
                        converter.ImageProcessing = ImageProcessing.ManualProvisioning;
						converter.ParseHtml (html);
					}
					//var attachment = new Attachment (new MemoryStream (virtualFile.ToArray ()), document.Name + ".docx");
					return new Attachment (new MemoryStream (virtualFile.ToArray ()), document.Name + ".docx");
				}
			}
            else if (document.ContentType == MediaTypeNames.Application.Pdf)
            {
                var path = document.Path;
                if (File.Exists(path))
                {
                    var fileStream = new FileStream(path, FileMode.Open); // filestream not disposed of...
                    Attachment pdf = new Attachment(fileStream, path, MediaTypeNames.Application.Pdf);
                    return pdf;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;

            }
			return null;
		}

		public async Task<List<Attachment>> ToAttachments (IEnumerable<SystemDocument> documents)
		{
			List<Attachment> attachments = new List<Attachment> ();
			foreach (SystemDocument document in documents)
				attachments.Add(await ToAttachment(document));
			return attachments;
		}


        #region Merge Field Library
        public List<KeyValuePair<string, string>> MergeFieldLibrary(User uISIssuer, Organisation insuredOrg, Programme programme, ClientInformationSheet clientInformationSheet, ClientAgreement clientAgreement)
        {
            List<KeyValuePair<string, string>> mergeFields = new List<KeyValuePair<string, string>>();

            if (programme != null)
            {
                mergeFields.Add(new KeyValuePair<string, string>("[[ProgrammeName]]", programme.Name));
                if(programme.BrokerContactUser != null)
                {
                    mergeFields.Add(new KeyValuePair<string, string>("[[ContactBrokerName]]", programme.BrokerContactUser.FullName));
                }
                
            }
            if(insuredOrg != null)
            {
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredName]]", insuredOrg.Name));
                mergeFields.Add(new KeyValuePair<string, string>("[[InsuredEmail]]", insuredOrg.Email));
            }
            if(uISIssuer != null)
            {
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuer]]", uISIssuer.FullName));
                mergeFields.Add(new KeyValuePair<string, string>("[[UISIssuerEmail]]", uISIssuer.Email));
            }
            if(clientInformationSheet != null)
            {
                mergeFields.Add(new KeyValuePair<string, string>("[[ReferenceID]]", clientInformationSheet.ReferenceId));
            }
            if (clientAgreement != null)
            {
                mergeFields.Add(new KeyValuePair<string, string>("[[WordingDownloadURL]]", clientAgreement.Product.WordingDownloadURL));
                mergeFields.Add(new KeyValuePair<string, string>("[[ProductName]]", clientAgreement.Product.Name));
            }
                        
            mergeFields.Add(new KeyValuePair<string, string>("[[SupportPhone]]", "09 377 6564"));

            return mergeFields;
        }
        #endregion

    }
}

