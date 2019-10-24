using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IFileService
	{
		string FileDirectory { get; }

		bool IsApplication (byte[] buffer, string contentType, string fileName);

		bool IsImageFile (byte[] buffer, string contentType, string fileName);

		bool IsTextFile (byte[] buffer, string contentType, string fileName);

		Task UploadFile (Document document);

		Task UploadFile (Image image);

        Task<Document> GetDocument (string documentName);

        Task<Image> GetImage (string imageName);

        Task<T> RenderDocument<T> (User renderedBy, T template, ClientAgreement agreement) where T : Document;

		byte [] ToBytes (string contents);
		string FromBytes (byte [] bytes);
        Task<Document> GetDocumentByType(Organisation primaryOrganisation, int documentType);
    }
}

