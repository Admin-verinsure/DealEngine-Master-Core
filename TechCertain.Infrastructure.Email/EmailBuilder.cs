using System;
using System.Configuration;
using System.Net.Mail;
using TechCertain.Infrastructure.Email.Interfaces;

namespace TechCertain.Infrastructure.Email
{
	public class EmailBuilder : IEmailBuilder
	{
		// Code taken from https://scottlilly.com/c-design-patterns-the-wrapperfacade-pattern/

		MailMessage _mailMessage;

        public EmailBuilder ()
		{
			_mailMessage = new MailMessage ();
        }

        public EmailBuilder (string senderAddress)
		{
			_mailMessage = new MailMessage ();
			_mailMessage.Sender = new MailAddress (senderAddress);
		}

		public IEmailBuilder From (string fromAddress)
		{
			_mailMessage.From = new MailAddress(fromAddress);
			return this;
		}

		public IEmailBuilder From (string fromAddress, string displayName)
		{
			_mailMessage.From = new MailAddress (fromAddress, displayName);
			return this;
		}

		public IEmailBuilder To (params string [] toAddresses)
		{
			foreach (string toAddress in toAddresses) {
				_mailMessage.To.Add (new MailAddress (toAddress));
			}
			return this;
		}

		public IEmailBuilder CC (params string [] ccAddresses)
		{
			foreach (string ccAddress in ccAddresses) {
				_mailMessage.CC.Add (new MailAddress (ccAddress));
			}
			return this;
		}

		public IEmailBuilder BCC (params string [] bccAddresses)
		{
			foreach (string bccAddress in bccAddresses) {
				_mailMessage.Bcc.Add (new MailAddress (bccAddress));
			}
			return this;
		}

        public IEmailBuilder ReplyTo(params string[] replyToAddresses)
        {
            foreach (string replyToAddress in replyToAddresses)
            {
                _mailMessage.ReplyToList.Add(new MailAddress(replyToAddress));
            }
            return this;
        }

        public IEmailBuilder WithSubject (string subject)
		{
			_mailMessage.Subject = subject;
			return this;
		}

		public IEmailBuilder WithBody (string body)
		{
			_mailMessage.Body = body;
			return this;
		}

		public IEmailBuilder UseHtmlBody (bool useHtml)
		{
			_mailMessage.IsBodyHtml = useHtml;
			return this;
		}

		public IEmailBuilder Attachments (params Attachment [] attachments)
		{
			foreach (Attachment attachment in attachments) {
				_mailMessage.Attachments.Add (attachment);
			}
			return this;
		}

        public void Send ()
		{
            //string smtpServer = ConfigurationManager.AppSettings ["SmtpServer"];
            //int smtpPort = Convert.ToInt32 (ConfigurationManager.AppSettings ["SmtpPort"]);

            string smtpServer = "localhost";
            int smtpPort = 25;

            using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient ()) {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect (smtpServer, smtpPort);                
                client.AuthenticationMechanisms.Remove ("XOAUTH2");
				client.Send (MimeKit.MimeMessage.CreateFromMailMessage(_mailMessage));

                client.Disconnect (true);
			}

		}
	}
}

