using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NReco.PdfGenerator;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace NReco.PdfGenerator.Examples.MergePdf {

	/// <summary>
	/// MergePdf example illustrates how to generate 2 PDF files with PdfGenerator 
	/// and merge them into one resulting PDF with iTextSharp (LGPL build - doesn't require commercial license).
	/// </summary>
	public class Program {

		static void Main(string[] args) {

			var pdfGen = new HtmlToPdfConverter();

			// lets generate 1st pdf with landscape orientation
			pdfGen.Orientation = PageOrientation.Portrait;
			var firstPdfBytes = pdfGen.GeneratePdf("<h1>PDF #1</h1>");

			// lets generate 2st pdf with portrait orientation
			pdfGen.Orientation = PageOrientation.Landscape;
			var secondPdfBytes = pdfGen.GeneratePdf("<h1>PDF #2</h1>");

			var sourcePdfs = new byte[][] {
				firstPdfBytes, secondPdfBytes
			};

			// merge PDF documents with iTextSharp
			using (var stream = new FileStream("merge_result.pdf", FileMode.OpenOrCreate)) {
				var doc = new Document();
				var pdf  = new PdfCopy(doc, stream);
				doc.Open();

				foreach (var pdfBytes in sourcePdfs) {
					var reader = new PdfReader( new MemoryStream(pdfBytes) );
					
					for (int i = 0; i < reader.NumberOfPages; i++) {
						var page = pdf.GetImportedPage(reader, i + 1);
						pdf.AddPage(page);
					}

					pdf.FreeReader(reader);
					reader.Close();	
				}
				pdf.Close();
				doc.Close();
			}

			Console.WriteLine("Merged 2 PDF files into merge_result.pdf");
		}

	}
}
