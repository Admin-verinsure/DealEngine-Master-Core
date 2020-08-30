using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NReco.PdfGenerator;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace NReco.PdfGenerator.Examples.WatermarkPdf {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Generate PDF document and watermark it with background image...");

			// this is simplest variant if you want just add some watermark text over PDF content
			Console.WriteLine("\nVariant 1: add transparent header text that overlaps page content");
			WatermarkWithHeaderText();
			Console.WriteLine("Generated watermarked_result_variant1.pdf");
			
			// this variant is suitable if you can provide watermark as background image that matches page height
			Console.WriteLine("\nVariant 2: add watermark with repeated CSS background... ");
			WatermarkWithCssBackground();
			Console.WriteLine("Generated watermarked_result_variant2.pdf");

			// use iTextSharp for adding watermark layer if variants listed above are not suitable
			Console.WriteLine("\nVariant 3: add watermark layer with iTextSharp library. PDF may be encrypted to prevent layer removal.");
			WatermarkWithITextSharp();
			Console.WriteLine("Generated watermarked_result_variant3.pdf");

			Console.WriteLine("\nPress any key to exit...");
			Console.ReadKey();
		}

		static void WatermarkWithHeaderText() {
			var htmlToPdf = new HtmlToPdfConverter();
			var watermarkText = "Draft";
			htmlToPdf.PageHeaderHtml = String.Format("<div><br/><div style='position:fixed;color:rgba(0,0,0,0.2);background-color:transparent;font-size:250px;text-align:center;width:100%;margin-top:200px;'>{0}</div></div>", 
				watermarkText);
			htmlToPdf.GeneratePdfFromFile("doc.html", null, "watermarked_result_variant1.pdf");
		}

		static void WatermarkWithCssBackground() {
			var htmlToPdf = new HtmlToPdfConverter();
			htmlToPdf.CustomWkHtmlArgs = "  --user-style-sheet doc_add_watermark.css ";
			htmlToPdf.GeneratePdfFromFile("doc.html", null, "watermarked_result_variant2.pdf");
		}

		static void WatermarkWithITextSharp() {
			var htmlToPdf = new HtmlToPdfConverter();
			var pdfMemStream = new MemoryStream();
			htmlToPdf.GeneratePdfFromFile("doc.html", null, pdfMemStream);
			// lets apply watermark (background image) for every PDF page with iTextSharp library (LGPL version -- can be used for FREE)
			using (var fs = new FileStream("watermarked_result_variant3.pdf", FileMode.OpenOrCreate, FileAccess.Write)) {
				ApplyWatermark(pdfMemStream.ToArray(), fs);
			}
		}

		static void ApplyWatermark(byte[] sourcePdf, Stream output) {
			var pdfRdr = new PdfReader(sourcePdf);

			var pdfStamper = new PdfStamper(pdfRdr, output);

			// the following line encrypts resulting PDF to prevent watermark removal
			pdfStamper.SetEncryption(true, null, 
				Guid.NewGuid().ToString(), // unique "owner" password
				PdfWriter.ALLOW_COPY | PdfWriter.ALLOW_PRINTING);

			for (var pIdx = 1; pIdx <= pdfRdr.NumberOfPages; pIdx++) {
				var pdfData = pdfStamper.GetUnderContent(pIdx);

				pdfData.BeginText();

				var pageRectangle = pdfRdr.GetPageSizeWithRotation(pIdx);

				var iTextSharpImage = iTextSharp.text.Image.GetInstance( Path.Combine(Environment.CurrentDirectory, "Watermark.png") );
				iTextSharpImage.ScaleAbsolute(pageRectangle.Width, pageRectangle.Height);
				iTextSharpImage.SetAbsolutePosition(0, 0);
				iTextSharpImage.Alignment = iTextSharp.text.Image.UNDERLYING;
				//iTextSharpImage.Transparency =  (new int[] { 0, 0}); 

				pdfData.AddImage(iTextSharpImage);

                pdfData.EndText();
			}

			pdfStamper.Close();
		}
	}
}
