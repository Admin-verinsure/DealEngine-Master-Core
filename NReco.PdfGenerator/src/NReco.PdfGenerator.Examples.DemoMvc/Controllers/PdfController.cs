using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.IO;
using System.Net;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

using System.Web.Security;

using NReco.PdfGenerator;

namespace Controllers {
	
	public class PdfController : Controller {

		public ActionResult DemoPage() {
			return View();
		}

		[PdfResult]
		public ActionResult GetPdf(string id) {
			return PartialView(id);
		}

		[PdfResult(GenerateToc=true)]
		public ActionResult CoverAndToc() {
			return PartialView();
		}


		public class PdfResultAttribute : ActionFilterAttribute {
			HttpWriter Output;
			StringBuilder HtmlContent;

			public bool GenerateToc { get; set; }

			public PdfResultAttribute() {
				GenerateToc = false;
			}

			public override void OnActionExecuting(ActionExecutingContext filterContext) {
				HtmlContent = new StringBuilder();
				Output = (HttpWriter)filterContext.RequestContext.HttpContext.Response.Output;
				filterContext.RequestContext.HttpContext.Response.Output = new HtmlTextWriter( new StringWriter(HtmlContent) );
			}

			public override void OnResultExecuted(ResultExecutedContext filterContext) {
				string responseHtml = HtmlContent.ToString();
				filterContext.RequestContext.HttpContext.Response.Output = Output;

				string coverHtml = null;
				var coverHtmlMatch = Regex.Match(responseHtml, "[<]cover[>](?<cover>.*?)[<][/]cover[>]", RegexOptions.Singleline | RegexOptions.IgnoreCase);
				if (coverHtmlMatch.Success) {
					coverHtml = coverHtmlMatch.Groups["cover"].Value;
					responseHtml = responseHtml.Remove(coverHtmlMatch.Index, coverHtmlMatch.Length);
				}

				var htmlToPdf = new HtmlToPdfConverter();
				// in some asp.net hosting environments system TEMP/bin folders may be not accessible for write. 
				// App_Data may be used in this case.
				var appDataPdfGen = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/PdfGenerator");
				htmlToPdf.TempFilesPath = appDataPdfGen;
				htmlToPdf.PdfToolPath = appDataPdfGen;

				var headerHtmlMatch = Regex.Match(responseHtml, "[<]header[>](?<header>.*?)[<][/]header[>]", RegexOptions.Singleline | RegexOptions.IgnoreCase);
				if (headerHtmlMatch.Success) {
					htmlToPdf.PageHeaderHtml = headerHtmlMatch.Groups["header"].Value;
					responseHtml = responseHtml.Remove(headerHtmlMatch.Index, headerHtmlMatch.Length);
				}
				var footerHtmlMatch = Regex.Match(responseHtml, "[<]footer[>](?<footer>.*?)[<][/]footer[>]", RegexOptions.Singleline | RegexOptions.IgnoreCase);
				if (footerHtmlMatch.Success) {
					htmlToPdf.PageFooterHtml = footerHtmlMatch.Groups["footer"].Value;
					responseHtml = responseHtml.Remove(footerHtmlMatch.Index, footerHtmlMatch.Length);
				}

				if (GenerateToc)
					htmlToPdf.GenerateToc = true;

				filterContext.HttpContext.Response.ContentType = "application/pdf";
				htmlToPdf.GeneratePdf(responseHtml, coverHtml, filterContext.HttpContext.Response.OutputStream);
			}
		}



	}
}
