using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Xml.Linq;

using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;

using NReco.PdfGenerator;

namespace NReco.PdfGenerator.Examples.WordToPdf {

	class Program {

		static void Main(string[] args) {
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InstalledUICulture;

			Console.WriteLine("Converting sample.docx to sample/index.html ...");
			var htmlFile = ConvetWordToHtml("sample.docx", Environment.CurrentDirectory);
			Console.WriteLine("Converting sample/index.html to sample.pdf");

			var htmlToPdf = new HtmlToPdfConverter();
			htmlToPdf.GeneratePdfFromFile(htmlFile, null, "sample.pdf");

			Console.WriteLine("Done! sample.docx was converted to sample.pdf");
		}

		// adopted code snippet from OpenXmlPowerTools Examples
		// more details: https://github.com/OfficeDev/Open-Xml-PowerTools
		static string ConvetWordToHtml(string wordFile, string htmlTmpDir) {
			using (var wordFs = new FileStream(wordFile, FileMode.Open, FileAccess.ReadWrite)) {

				using (WordprocessingDocument wDoc = WordprocessingDocument.Open(wordFs, true))
				{
					var destName = Path.GetFileNameWithoutExtension(wordFile);
					var destDir = Path.Combine(htmlTmpDir, destName);
					if (!Directory.Exists(destDir))
						Directory.CreateDirectory(destDir);
						int imageCounter = 0;

					var pageTitle = destName;
					var part = wDoc.CoreFilePropertiesPart;
					if (part != null)
					{
						pageTitle = (string) part.GetXDocument().Descendants(DC.title).FirstOrDefault() ?? destName;
					}

					HtmlConverterSettings settings = new HtmlConverterSettings()
					{
						AdditionalCss = "body { margin: 1cm auto; max-width: 20cm; padding: 0; }",
						PageTitle = pageTitle,
						FabricateCssClasses = true,
						CssClassPrefix = "pt-",
						RestrictToSupportedLanguages = false,
						RestrictToSupportedNumberingFormats = false,
						ImageHandler = imageInfo =>
						{
							DirectoryInfo localDirInfo = new DirectoryInfo(destName);
							++imageCounter;
							string extension = imageInfo.ContentType.Split('/')[1].ToLower();
							ImageFormat imageFormat = null;
							if (extension == "png")
								imageFormat = ImageFormat.Png;
							else if (extension == "gif")
								imageFormat = ImageFormat.Gif;
							else if (extension == "bmp")
								imageFormat = ImageFormat.Bmp;
							else if (extension == "jpeg")
								imageFormat = ImageFormat.Jpeg;
							else if (extension == "tiff") {
								// Convert tiff to gif.
								extension = "gif";
								imageFormat = ImageFormat.Gif;
							}
							else if (extension == "x-wmf")
							{
								extension = "wmf";
								imageFormat = ImageFormat.Wmf;
							}

							// If the image format isn't one that we expect, ignore it,
							// and don't return markup for the link.
							if (imageFormat == null)
								return null;

							string imageFileName = imageCounter.ToString() + "." + extension;
							try {
								imageInfo.Bitmap.Save(
									Path.Combine(destName, imageFileName), imageFormat);
							} catch (Exception ex) {
								Console.WriteLine("Cannot save image {0}: {1}", imageInfo.ContentType, ex.Message);
								return null;
							}
							XElement img = new XElement(Xhtml.img,
								new XAttribute(NoNamespace.src, imageFileName),
								imageInfo.ImgStyleAttribute,
								imageInfo.AltText != null ?
									new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
							return img;
						}
					};
					XElement htmlElement = HtmlConverter.ConvertToHtml(wDoc, settings);

					// Produce HTML document with <!DOCTYPE html > declaration to tell the browser
					// we are using HTML5.
					var html = new XDocument(
						new XDocumentType("html", null, null, null),
						htmlElement);

					// Note: the xhtml returned by ConvertToHtmlTransform contains objects of type
					// XEntity.  PtOpenXmlUtil.cs define the XEntity class.  See
					// http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
					// for detailed explanation.
					//
					// If you further transform the XML tree returned by ConvertToHtmlTransform, you
					// must do it correctly, or entities will not be serialized properly.

					var htmlString = html.ToString(SaveOptions.DisableFormatting);
					var destHtmlPath = Path.Combine(destName, "index.html"); 
					File.WriteAllText(destHtmlPath, htmlString, Encoding.UTF8);
					return destHtmlPath;
				}


			}
		}
	}
}
