using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using NReco.PdfGenerator;

namespace NReco.PdfGenerator.Examples.DataSetReport
{
    public class Program 
    {
		
		
		static int Main(string[] args) {
			
			var ds = CreateSampleDataSet();
			var dsXml = ds.GetXml();

			var xslTransformer = new XslCompiledTransform();
			using (var stylesheetReader = XmlTextReader.Create(new StreamReader("report.xslt"))) {
				xslTransformer.Load(stylesheetReader);
			}

			var resultWriter = new StringWriter();
			using (var xmlDataReader = XmlTextReader.Create(new StringReader(dsXml))) {
				var xmlDoc = new XPathDocument(xmlDataReader);
				xslTransformer.Transform(xmlDoc, null, resultWriter);
			}

			var reportHtml = resultWriter.ToString();


			var pdfGen = new HtmlToPdfConverter();
			
			// it is possible to handle all console output provided by wkhtmltopdf. Useful for debug purposes.
			pdfGen.LogReceived += (sender, e) => {
				Console.WriteLine("WkHtmlToPdf Log: {0}", e.Data);
			};
			var pdfBytes = pdfGen.GeneratePdf(reportHtml);

			using (var pdfFile = new FileStream("report.pdf", FileMode.OpenOrCreate, FileAccess.Write)) {
				pdfFile.Write(pdfBytes, 0, pdfBytes.Length);
			}

			return 0;
		}


		static DataSet CreateSampleDataSet() {
			var ds = new DataSet();
			ds.DataSetName = "data";
			var t = ds.Tables.Add("order_items");

			t.Columns.Add("id", typeof(int));
			t.Columns.Add("title", typeof(string));
			t.Columns.Add("quantity", typeof(int));
			t.Columns.Add("price", typeof(decimal));

			for (int i = 1; i <= 10; i++) {
				var r = t.NewRow();
				r["id"] = i;
				r["title"] = String.Format("Item #{0}", i);
				r["quantity"] = i;
				r["price"] = i * 5;
				t.Rows.Add(r);
			}

			return ds;
		}
    }
}
