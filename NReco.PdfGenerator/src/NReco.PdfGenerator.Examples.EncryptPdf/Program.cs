using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NReco.PdfGenerator;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace NReco.PdfGenerator.Examples.EncryptPdf {
	class Program {
		static void Main(string[] args) {

			Console.WriteLine("Generate PDF document and protect it (encrypt) with background image...");

			var pdfMemStream = new MemoryStream();
			var htmlToPdf = new HtmlToPdfConverter();
			htmlToPdf.GeneratePdfFromFile("doc.html", null, pdfMemStream);

			// lets encrypt generated PDF with iTextSharp library (LGPL version -- can be used for FREE)
			using (var fs = new FileStream("secured_result.pdf", FileMode.OpenOrCreate, FileAccess.Write)) {
				EncryptPdf(pdfMemStream.ToArray(), "123456", fs);
			}

			Console.WriteLine("Generated secured_result.pdf. Press any key to exit...");
			Console.ReadKey();

		}

		static void EncryptPdf(byte[] sourcePdf, string pwd, Stream output) {
			var pwdBytes = Encoding.ASCII.GetBytes(pwd);

			var pdfRdr = new PdfReader(sourcePdf);

			var stamper = new PdfStamper(pdfRdr, output);
			stamper.SetEncryption(
				pwdBytes,
				pwdBytes,
				PdfWriter.ALLOW_COPY | PdfWriter.ALLOW_PRINTING,
				PdfWriter.ENCRYPTION_AES_128);

			stamper.Close();
			pdfRdr.Close();
		}

	}
}
