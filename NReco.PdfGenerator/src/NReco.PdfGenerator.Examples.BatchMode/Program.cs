using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NReco.PdfGenerator.Examples.BatchMode {
	class Program {
		static void Main(string[] args) {

			Console.WriteLine("Batch mode uses one wkhtmltopdf.exe process to improve PDF generation performance and better CPU resources utilization.");

			var sw = new Stopwatch();
			var htmlToPdf = new HtmlToPdfConverter();

			// batch feature requires license key
			htmlToPdf.License.SetLicenseKey("DEMO", DemoLicenseKey);  // use your own license info from order's page

			// if you don't want to call SetLicenseKey each time:
			// licensing information may be specified in the web.config (or app.config):
			// <appSettings>
			//	<add key="NReco.PdfGenerator.LicenseOwner" value="YOUR_LICENCE_ID"/>
			//	<add key="NReco.PdfGenerator.LicenseKey" value="YOUR_LICENCE_KEY"/>
			// </appSettings>

			htmlToPdf.Quiet = true;
			htmlToPdf.LogReceived += (sender,a) => {
				Console.WriteLine("---> {0}", a.Data);
			};

			Console.WriteLine("Generate 10 PDF document in usual way...");
			
			sw.Start();
			for (int i = 0; i < 10; i++) {
				var f = String.Format("_res{0}.pdf", i);
				Console.WriteLine("Generating: {0}", f );
				htmlToPdf.GeneratePdfFromFile("doc.html", null, f);
			}
			sw.Stop();

			Console.WriteLine("Generation time: {0}", sw.Elapsed);

			Console.WriteLine("Generate 10 PDF document in the batch mode...");
			htmlToPdf.BeginBatch();
			sw.Restart();
			try {

				for (int i = 0; i < 10; i++) {
					var f = String.Format("_res{0}_batch.pdf", i);
					Console.WriteLine("Generating: {0}", f );
					htmlToPdf.GeneratePdfFromFile("doc.html", null, f);
				}

			} finally {
				// always call EndBatch at the end!
				htmlToPdf.EndBatch();
			}

			sw.Stop();
			Console.WriteLine("Generation time: {0}", sw.Elapsed);

		}

		static string DemoLicenseKey = "pjfsL9eBhU5mER7ULRNO/pgqeXBsJF15ea8d+vpzJ/ja8LpELgrs2FoaZwNLRmsJpgwKahfzSvyCZDHZsI0Bs9KCaby6CXo02YxpA3iiwJPaDdfO+vTK/JCsjH/d9l6118KUVjegNFAraGSKA3Q6tMTg4/injiZ9wZ3kcjoXPjs=";

	}


}
