﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Xml;
using System.Xml.Linq;

namespace TechCertain.WebUI
{
	public class XmlActionResult : ActionResult
	{
		protected XDocument _document;

		public Formatting Formatting { get; set; }
		public string MimeType { get; set; }

		public XmlActionResult (XDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			_document = document;

			// Default values
			MimeType = "text/xml";
			Formatting = Formatting.None;
		}

		public void ExecuteResult (ControllerContext context)
		{
            throw new Exception("This method needs to be re-written");
			//context.HttpContext.Response.Clear();
			//context.HttpContext.Response.ContentType = MimeType;

			//using (var writer = new XmlTextWriter(context.HttpContext.Response.OutputStream, Encoding.UTF8) { Formatting = Formatting })
			//	_document.WriteTo(writer);
		}
	}
}

