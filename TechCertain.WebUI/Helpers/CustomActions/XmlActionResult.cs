using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TechCertain.WebUI.Helpers.CustomActions
{
	public class XmlActionResult : ActionResult
	{
		protected XDocument _document;

		public Formatting Formatting { get; set; }
		public string MimeType { get; set; }

		public XmlActionResult (XDocument document)
		{
			if (document == null)
				throw new ArgumentNullException(nameof(document));

			_document = document;

			// Default values
			MimeType = "text/xml";
			Formatting = Formatting.None;
		}

		public override void ExecuteResult (ActionContext context)
		{
            var syncIOFeature = context.HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            context.HttpContext.Response.Headers.Clear();
            context.HttpContext.Response.ContentType = MimeType;

           
            using (var writer = new XmlTextWriter(context.HttpContext.Response.Body, Encoding.UTF8) { Formatting = Formatting })
            	_document.WriteTo(writer);


                //context.HttpContext.Response.ContentType = MimeType;

                //using (var writer = new XmlTextWriter(context.HttpContext.Response.OutputStream, Encoding.UTF8) { Formatting = Formatting })
                //	_document.WriteTo(writer);
        }
    }
}

