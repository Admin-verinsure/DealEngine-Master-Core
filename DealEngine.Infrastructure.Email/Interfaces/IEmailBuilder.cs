using System.Net.Mail;

namespace DealEngine.Infrastructure.Email.Interfaces
{
	public interface IEmailBuilder
	{
		IEmailBuilder From (string fromAddress);
		IEmailBuilder From (string fromAddress, string displayName);
		IEmailBuilder To (params string [] toAddresses);
		IEmailBuilder CC (params string [] ccAddresses);
		IEmailBuilder BCC (params string [] bccAddresses);
		IEmailBuilder WithSubject (string subject);
		IEmailBuilder WithBody (string body);
		IEmailBuilder UseHtmlBody (bool useHtml);
		IEmailBuilder Attachments (params Attachment [] attachments);
		void Send ();
	}
}

