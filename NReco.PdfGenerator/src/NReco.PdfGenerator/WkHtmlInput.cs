/*
 *  Copyright 2017 Vitaliy Fedorchenko
 *
 *  Licensed under PdfGenerator Source Code Licence (see LICENSE file).
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS 
 *  OF ANY KIND, either express or implied.
 */ 
using System;
using System.Collections.Generic;
using System.Text;

namespace NReco.PdfGenerator {

	/// <summary>
	/// Represents one wkhtmltopdf input.
	/// </summary>
	public class WkHtmlInput {

		/// <summary>
		/// HTML file name or URL.
		/// </summary>
		public string Input { get; set; }

		/// <summary>
		/// Custom WkHtmlToPdf page options for this input.
		/// </summary>
		public string CustomWkHtmlPageArgs { get; set; }

		/// <summary>
		/// Get or set custom page header HTML for this input.
		/// </summary>
		public string PageHeaderHtml { get; set; }

		/// <summary>
		/// Get or set custom page footer HTML for this input.
		/// </summary>
		public string PageFooterHtml { get; set; }

		public WkHtmlInput(string inputFileOrUrl) {
			Input = inputFileOrUrl;
		}

		internal string HeaderFilePath { get; set; }
		internal string FooterFilePath { get; set; }
	}
}
