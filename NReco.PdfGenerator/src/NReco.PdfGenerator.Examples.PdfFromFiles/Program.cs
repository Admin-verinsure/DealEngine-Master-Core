using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NReco.PdfGenerator.Examples.PdfFromFiles {
	class Program {
		static void Main(string[] args) {

			Console.WriteLine("Generating PDF from several HTML documents...");

			// ensure that resulting file does not exist
			if (File.Exists("result.pdf"))
				File.Delete("result.pdf");

			var htmlDocuments = new List<string>();

			// lets use wiki pages as HTML documents
			// note: it is possible to specify full path to the local file as well 
			htmlDocuments.Add("https://en.wikipedia.org/w/index.php?title=Report&printable=yes");
			htmlDocuments.Add("https://en.wikipedia.org/w/index.php?title=Portable_Document_Format&printable=yes");

			var htmlToPdf = new HtmlToPdfConverter();
			// GeneratePdfFromFiles requires a license key
			htmlToPdf.License.SetLicenseKey("DEMO", DemoLicenseKey);  // use your own license info from order's page

			// lets use simple common header
			htmlToPdf.PageHeaderHtml = "<div style='background-color:silver;padding:5px;'>Page <span class='page'></span> of <span class='topage'></span></div>";
			htmlToPdf.Margins.Top = 15; //mm

			htmlToPdf.GeneratePdfFromFiles(
				htmlDocuments.ToArray(),
				null, // optional cover page html
				"result.pdf" // overload for output to Stream also available
			);

			Console.WriteLine("Generated result.pdf. Press any key to exit...");
			Console.ReadKey();
		}

		static string DemoLicenseKey = "pjfsL9eBhU5mER7ULRNO/pgqeXBsJF15ea8d+vpzJ/ja8LpELgrs2FoaZwNLRmsJpgwKahfzSvyCZDHZsI0Bs9KCaby6CXo02YxpA3iiwJPaDdfO+vTK/JCsjH/d9l6118KUVjegNFAraGSKA3Q6tMTg4/injiZ9wZ3kcjoXPjs=";

	}
}
