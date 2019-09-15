using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IFileService
	{
		/// <summary>
		/// Gets the absolute directiory where files are uploaded to.
		/// </summary>
		/// <value>The file directory.</value>
		string FileDirectory { get; }

		/// <summary>
		/// Determines if the uploaded file is a valid application file.
		/// </summary>
		/// <returns><c>true</c> if the uploaded file is a valid application; otherwise, <c>false</c>.</returns>
		/// <param name="buffer">Byte array containing the uploaded file.</param>
		/// <param name="contentType">Content type of the uploaded file.</param>
		/// <param name="fileName">File name of the uploaded file.</param>
		bool IsApplication (byte[] buffer, string contentType, string fileName);

		/// <summary>
		/// Determines if the uploaded file is a valid image file. Only JPEG, GIF, PNG and TIFF files are recognized.
		/// </summary>
		/// <returns><c>true</c> if the uploaded file is a valid image; otherwise, <c>false</c>.</returns>
		/// <param name="buffer">Byte array containing the uploaded file.</param>
		/// <param name="contentType">Content type of the uploaded file.</param>
		/// <param name="fileName">File name of the uploaded file.</param>
		bool IsImageFile (byte[] buffer, string contentType, string fileName);

		/// <summary>
		/// Determines if the uploaded file is a valid image file. Only HTML, TXT, RTF and XML files are recognized.
		/// </summary>
		/// <returns><c>true</c> if the uploaded file is a valid text file; otherwise, <c>false</c>.</returns>
		/// <param name="buffer">Byte array containing the uploaded file.</param>
		/// <param name="contentType">Content type of the uploaded file.</param>
		/// <param name="fileName">File name of the uploaded file.</param>
		bool IsTextFile (byte[] buffer, string contentType, string fileName);

		/// <summary>
		/// Uploads the given file.
		/// </summary>
		/// <returns><c>true</c>, if file was uploaded, <c>false</c> otherwise.</returns>
		/// <param name="uploadedBy">User who uploaded the file.</param>
		/// <param name="buffer">Byte array containing the uploaded file.</param>
		/// <param name="contentType">Content type of the uploaded file.</param>
		/// <param name="fileName">File name of the uploaded file.</param>
		//bool UploadFile (User uploadedBy, byte[] buffer, string contentType, string fileName);

		/// <summary>
		/// Uploads the given document.
		/// </summary>
		/// <returns><c>true</c>, if file was uploaded, <c>false</c> otherwise.</returns>
		/// <param name="document">The Document to be uploaded.</param>
		bool UploadFile (Document document);

		/// <summary>
		/// Uploads the given image.
		/// </summary>
		/// <returns><c>true</c>, if file was uploaded, <c>false</c> otherwise.</returns>
		/// <param name="image">The Image to be uploaded.</param>
		bool UploadFile (Image image);

		Document GetDocument (string documentName);

		Image GetImage (string imageName);

		T RenderDocument<T> (User renderedBy, T template, ClientAgreement agreement) where T : Document;

		/// <summary>
		/// Converts the specified string to an array of bytes.
		/// </summary>
		/// <returns>The converted string.</returns>
		/// <param name="contents">The string to be converted.</param>
		byte [] ToBytes (string contents);

		/// <summary>
		/// Converts an array of bytes to a string.
		/// </summary>
		/// <returns>The converted byte array.</returns>
		/// <param name="bytes">The array of bytes to be converted.</param>
		string FromBytes (byte [] bytes);
        Document GetDocumentByType(Organisation primaryOrganisation, int documentType);
    }
}

